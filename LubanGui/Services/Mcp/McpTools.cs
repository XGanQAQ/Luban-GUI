using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LubanGui.Infrastructure;
using LubanGui.LubanAdapter.Interfaces;
using LubanGui.Models;
using LubanGui.Services.Luban;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace LubanGui.Services.Mcp;

[McpServerToolType]
public class McpTools
{
    private static readonly SemaphoreSlim ExportLock = new(1, 1);

    private readonly IProjectManager _projectManager;
    private readonly ISchemaService _schemaService;
    private readonly ITablePreviewService _tablePreviewService;
    private readonly IExportService _exportService;
    private readonly ProjectConfigManager _projectConfigManager;
    private readonly ILubanConfAdapter _lubanConfAdapter;
    private readonly ILogger<McpTools> _logger;

    public McpTools(
        IProjectManager projectManager,
        ISchemaService schemaService,
        ITablePreviewService tablePreviewService,
        IExportService exportService,
        ProjectConfigManager projectConfigManager,
        ILubanConfAdapter lubanConfAdapter,
        ILogger<McpTools> logger)
    {
        _projectManager = projectManager;
        _schemaService = schemaService;
        _tablePreviewService = tablePreviewService;
        _exportService = exportService;
        _projectConfigManager = projectConfigManager;
        _lubanConfAdapter = lubanConfAdapter;
        _logger = logger;
    }

    // ── 项目管理 ──────────────────────────────────────────────────────────────

    [McpServerTool, Description("列出所有已注册的 Luban 项目")]
    public object[] ListProjects()
    {
        return _projectManager.Projects.Select(p => new
        {
            p.Name,
            p.WorkspaceRoot,
            p.ProjectPath,
            p.LastOpenedAt
        }).ToArray();
    }

    [McpServerTool, Description("获取当前打开的项目信息；无项目时返回 null")]
    public object? GetCurrentProject()
    {
        var p = _projectManager.CurrentProject;
        return p == null ? null : new
        {
            p.Name,
            p.WorkspaceRoot,
            p.ProjectPath,
            p.LastOpenedAt
        };
    }

    [McpServerTool, Description("按项目路径打开一个已有的 Luban 项目")]
    public async Task<string> OpenProject(string projectPath)
    {
        if (!Directory.Exists(projectPath))
        {
            return $"项目目录不存在: {projectPath}";
        }

        try
        {
            await _projectManager.OpenProjectAsync(projectPath);
            return $"已打开项目: {_projectManager.CurrentProject?.Name ?? projectPath}";
        }
        catch (Exception ex)
        {
            return $"打开项目失败: {ex.Message}";
        }
    }

    [McpServerTool, Description("在工作区目录下创建新的 Luban 项目")]
    public async Task<object?> CreateProject(string name, string workspacePath)
    {
        try
        {
            var info = await _projectManager.CreateProjectAsync(name, workspacePath);
            return new
            {
                info.Name,
                info.WorkspaceRoot,
                info.ProjectPath,
                info.LastOpenedAt
            };
        }
        catch (Exception ex)
        {
            return $"创建项目失败: {ex.Message}";
        }
    }

    // ── Schema 检查 ───────────────────────────────────────────────────────────

    [McpServerTool, Description("列出当前项目中的所有表格")]
    public async Task<object[]?> ListTables()
    {
        var project = _projectManager.CurrentProject;
        if (project == null) return null;

        var metas = await _schemaService.LoadTablesAsync(project.ProjectPath);
        return metas.Select(m => new
        {
            m.FullName,
            m.Input,
            m.ValueType,
            m.Index
        }).ToArray();
    }

    [McpServerTool, Description("获取指定表格的字段定义")]
    public async Task<object?[]> GetTableSchema(string tableFullName)
    {
        var project = _projectManager.CurrentProject;
        if (project == null) return Array.Empty<object>();

        var metas = await _schemaService.LoadTablesAsync(project.ProjectPath);
        var meta = metas.FirstOrDefault(m =>
            string.Equals(m.FullName, tableFullName, StringComparison.OrdinalIgnoreCase));
        if (meta == null) return Array.Empty<object>();

        var xlsxPath = Path.Combine(project.ProjectPath, "Datas", meta.Input);
        if (!File.Exists(xlsxPath)) return Array.Empty<object>();

        var fields = ExcelWriter.ReadDataXlsxSchema(xlsxPath);
        return fields.Select(f => new
        {
            f.Name,
            f.Type,
            f.Comment
        }).ToArray();
    }

    [McpServerTool, Description("列出当前项目中所有可用的自定义类型名称（枚举和 Bean）")]
    public async Task<string[]?> ListEnums()
    {
        var project = _projectManager.CurrentProject;
        if (project == null) return null;

        var names = await _schemaService.GetAvailableTypeNamesAsync(project.ProjectPath);
        return names.ToArray();
    }

    [McpServerTool, Description("获取指定枚举的完整定义")]
    public async Task<object?> GetEnumSchema(string fullName)
    {
        var project = _projectManager.CurrentProject;
        if (project == null) return null;

        var dto = await _schemaService.GetEnumAsync(project.ProjectPath, fullName);
        if (dto == null) return null;

        return new
        {
            dto.FullName,
            dto.IsFlags,
            dto.IsUnique,
            Items = dto.Items.Select(i => new { i.Name, i.Value, i.Comment }).ToArray()
        };
    }

    // ── 数据预览 ──────────────────────────────────────────────────────────────

    [McpServerTool, Description("预览指定表格的数据内容（最多返回指定行数）")]
    public async Task<object?> PreviewTable(string tableFullName, int maxRows = 100)
    {
        var project = _projectManager.CurrentProject;
        if (project == null) return null;

        var metas = await _schemaService.LoadTablesAsync(project.ProjectPath);
        var meta = metas.FirstOrDefault(m =>
            string.Equals(m.FullName, tableFullName, StringComparison.OrdinalIgnoreCase));
        if (meta == null) return null;

        var absPath = Path.Combine(project.ProjectPath, "Datas", meta.Input);
        if (!File.Exists(absPath)) return null;

        var data = await _tablePreviewService.LoadPreviewAsync(absPath, project.ProjectPath);
        return new
        {
            Columns = data.Columns.ToArray(),
            Rows = data.Rows.Take(maxRows).Select(r => r.ToArray()).ToArray(),
            TotalRows = data.Rows.Count
        };
    }

    // ── 导表操作 ──────────────────────────────────────────────────────────────

    [McpServerTool, Description("执行全量导表（如果没有正在进行的导表操作）")]
    public async Task<object> RunExport(string? target = null)
    {
        if (!await ExportLock.WaitAsync(0))
        {
            return new { Success = false, ErrorMessage = "已有导表任务正在进行，请稍后再试" };
        }

        try
        {
            var project = _projectManager.CurrentProject;
            if (project == null)
            {
                return new { Success = false, ErrorMessage = "当前没有打开的项目" };
            }

            var config = await _projectConfigManager.LoadAsync(project.ProjectPath);
            if (!string.IsNullOrEmpty(target))
            {
                config.Target = target;
            }

            var errors = _exportService.ValidateConfig(config);
            if (errors.Count > 0)
            {
                return new
                {
                    Success = false,
                    ErrorMessage = "配置校验失败",
                    Errors = errors.ToArray()
                };
            }

            var output = new List<string>();
            var progress = new Progress<string>(line => output.Add(line));

            var result = await _exportService.ExportAsync(config, progress, CancellationToken.None);

            return new
            {
                result.Success,
                result.ErrorMessage,
                result.ExitCode,
                Duration = result.Duration.TotalSeconds,
                Output = string.Join("\n", output)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MCP 导表异常");
            return new { Success = false, ErrorMessage = $"导表异常: {ex.Message}" };
        }
        finally
        {
            ExportLock.Release();
        }
    }

    // ── 配置管理 ──────────────────────────────────────────────────────────────

    [McpServerTool, Description("获取当前项目的导出配置")]
    public async Task<object?> GetExportConfig()
    {
        var project = _projectManager.CurrentProject;
        if (project == null) return null;

        var config = await _projectConfigManager.LoadAsync(project.ProjectPath);
        return new
        {
            DataOutput = new { config.DataOutput.Type, config.DataOutput.OutputPath },
            CodeOutput = new
            {
                config.CodeOutput.Enabled,
                config.CodeOutput.Type,
                config.CodeOutput.OutputPath,
                config.CodeOutput.TopModule
            },
            config.Target
        };
    }

    [McpServerTool, Description("更新当前项目的导出配置")]
    public async Task<string> UpdateExportConfig(
        string dataOutputType,
        string dataOutputPath,
        bool codeOutputEnabled = false,
        string codeOutputType = "cs-bin",
        string codeOutputPath = "",
        string codeOutputTopModule = "cfg",
        string target = "all")
    {
        var project = _projectManager.CurrentProject;
        if (project == null)
        {
            return "当前没有打开的项目";
        }

        var config = new ProjectConfig
        {
            DataOutput = new DataOutputConfig
            {
                Type = dataOutputType,
                OutputPath = dataOutputPath
            },
            CodeOutput = new CodeOutputConfig
            {
                Enabled = codeOutputEnabled,
                Type = codeOutputType,
                OutputPath = codeOutputPath,
                TopModule = codeOutputTopModule
            },
            Target = target
        };

        try
        {
            await _projectConfigManager.SaveAsync(project.ProjectPath, config);
            return "导出配置已更新";
        }
        catch (Exception ex)
        {
            return $"更新导出配置失败: {ex.Message}";
        }
    }

    // ── 表格 Schema 创建/修改 ──────────────────────────────────────────────────

    [McpServerTool, Description("创建新表格：定义表名、索引字段和字段列表，生成数据 xlsx 并注册到 __tables__.xlsx")]
    public async Task<object?> CreateTable(
        string fullName,
        string indexField,
        List<FieldDefinition> fields)
    {
        var project = _projectManager.CurrentProject;
        if (project == null)
        {
            return "当前没有打开的项目";
        }

        try
        {
            var meta = await _schemaService.CreateTableAsync(
                project.ProjectPath, fullName, indexField, fields);
            return new
            {
                Success = true,
                Message = $"表格已创建: {meta.FullName}",
                meta.FullName,
                meta.Input,
                meta.ValueType,
                meta.Index,
                FieldCount = fields.Count
            };
        }
        catch (Exception ex)
        {
            return $"创建表格失败: {ex.Message}";
        }
    }

    [McpServerTool, Description("修改表格字段定义：更新字段名、类型和注释，保留已有数据行")]
    public async Task<string> ModifyTableSchema(
        string tableFullName,
        List<FieldDefinition> fields)
    {
        var project = _projectManager.CurrentProject;
        if (project == null)
        {
            return "当前没有打开的项目";
        }

        try
        {
            var metas = await _schemaService.LoadTablesAsync(project.ProjectPath);
            var meta = metas.FirstOrDefault(m =>
                string.Equals(m.FullName, tableFullName, StringComparison.OrdinalIgnoreCase));
            if (meta == null)
            {
                return $"未找到表格: {tableFullName}";
            }

            await _schemaService.ModifyTableFieldsAsync(
                project.ProjectPath, meta.Input, fields);
            return $"表格字段已更新: {tableFullName}（{fields.Count} 个字段）";
        }
        catch (Exception ex)
        {
            return $"修改表格字段失败: {ex.Message}";
        }
    }

    [McpServerTool, Description("删除表格：从 __tables__.xlsx 中移除注册，并可选删除数据文件")]
    public async Task<string> DeleteTable(
        string tableFullName,
        bool deletePhysicalFile = false)
    {
        var project = _projectManager.CurrentProject;
        if (project == null)
        {
            return "当前没有打开的项目";
        }

        try
        {
            var metas = await _schemaService.LoadTablesAsync(project.ProjectPath);
            var meta = metas.FirstOrDefault(m =>
                string.Equals(m.FullName, tableFullName, StringComparison.OrdinalIgnoreCase));
            if (meta == null)
            {
                return $"未找到表格: {tableFullName}";
            }

            await _schemaService.DeleteTableAsync(
                project.ProjectPath, meta.FullName, meta.Input, deletePhysicalFile);
            return $"表格已删除: {tableFullName}";
        }
        catch (Exception ex)
        {
            return $"删除表格失败: {ex.Message}";
        }
    }

    // ── 枚举创建/修改 ──────────────────────────────────────────────────────────

    [McpServerTool, Description("创建新枚举：定义枚举名、各项和属性，注册到 __enums__.xlsx")]
    public async Task<string> CreateEnum(
        string fullName,
        List<EnumItemDefinition> items,
        bool isFlags = false,
        bool isUnique = true)
    {
        var project = _projectManager.CurrentProject;
        if (project == null)
        {
            return "当前没有打开的项目";
        }

        try
        {
            await _schemaService.CreateEnumAsync(
                project.ProjectPath, fullName, isFlags, isUnique, items);
            return $"枚举已创建: {fullName}（{items.Count} 项）";
        }
        catch (Exception ex)
        {
            return $"创建枚举失败: {ex.Message}";
        }
    }

    [McpServerTool, Description("更新枚举定义：替换枚举项列表和属性")]
    public async Task<string> UpdateEnum(
        string fullName,
        List<EnumItemDefinition> items,
        bool isFlags = false,
        bool isUnique = true)
    {
        var project = _projectManager.CurrentProject;
        if (project == null)
        {
            return "当前没有打开的项目";
        }

        try
        {
            await _schemaService.UpdateEnumAsync(
                project.ProjectPath, fullName, isFlags, isUnique, items);
            return $"枚举已更新: {fullName}（{items.Count} 项）";
        }
        catch (Exception ex)
        {
            return $"更新枚举失败: {ex.Message}";
        }
    }

    // ── Bean 创建 ──────────────────────────────────────────────────────────────

    [McpServerTool, Description("创建新 Bean 结构体：定义字段列表，注册到 __beans__.xlsx")]
    public async Task<string> CreateBean(
        string fullName,
        List<FieldDefinition> fields)
    {
        var project = _projectManager.CurrentProject;
        if (project == null)
        {
            return "当前没有打开的项目";
        }

        try
        {
            await _schemaService.CreateBeanAsync(
                project.ProjectPath, fullName, fields);
            return $"Bean 已创建: {fullName}（{fields.Count} 个字段）";
        }
        catch (Exception ex)
        {
            return $"创建 Bean 失败: {ex.Message}";
        }
    }
}
