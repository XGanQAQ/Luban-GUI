using System.Collections.Generic;
using System.Threading.Tasks;
using LubanGui.Models;

namespace LubanGui.Services;

/// <summary>
/// 负责读取和修改 Luban Schema 元数据（__tables__.xlsx 等）。
/// </summary>
public interface ISchemaService
{
    /// <summary>从项目的 __tables__.xlsx 加载所有表格元数据。</summary>
    Task<List<TableMeta>> LoadTablesAsync(string projectPath);

    /// <summary>
    /// 创建新表格：生成 xlsx 数据文件，并将条目写入 __tables__.xlsx。
    /// </summary>
    Task<TableMeta> CreateTableAsync(
        string projectPath,
        string fullName,
        string indexField,
        IReadOnlyList<FieldDefinition> fields);

    /// <summary>
    /// 导入已有 xlsx 文件：仅将其注册到 __tables__.xlsx，不修改文件本身。
    /// </summary>
    Task<TableMeta> ImportTableAsync(
        string projectPath,
        string xlsxAbsPath,
        string fullName,
        string indexField);

    /// <summary>创建新枚举：生成独立的枚举 xlsx 文件（MVP 阶段不修改 __enums__.xlsx）。</summary>
    Task CreateEnumAsync(
        string projectPath,
        string fullName,
        IReadOnlyList<EnumItemDefinition> items);

    /// <summary>创建新 Bean：生成独立的 Bean xlsx 文件（MVP 阶段不修改 __beans__.xlsx）。</summary>
    Task CreateBeanAsync(
        string projectPath,
        string fullName,
        IReadOnlyList<FieldDefinition> fields);
}
