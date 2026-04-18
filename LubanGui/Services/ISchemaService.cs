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

    /// <summary>
    /// 创建新枚举，并将条目写入 __enums__.xlsx。
    /// </summary>
    /// <param name="isFlags">是否为标志位枚举（可多值按位组合）。</param>
    /// <param name="isUnique">枚举项整数值是否唯一，建议为 true。</param>
    Task CreateEnumAsync(
        string projectPath,
        string fullName,
        bool isFlags,
        bool isUnique,
        IReadOnlyList<EnumItemDefinition> items);

    /// <summary>创建新 Bean，并将条目写入 __beans__.xlsx。</summary>
    Task CreateBeanAsync(
        string projectPath,
        string fullName,
        IReadOnlyList<FieldDefinition> fields);

    /// <summary>
    /// 从项目 Schema 中读取所有已注册的枚举和 Bean 的 full_name，
    /// 供字段类型输入时提供自动补全候选列表。
    /// </summary>
    Task<IReadOnlyList<string>> GetAvailableTypeNamesAsync(string projectPath);

    /// <summary>
    /// 读取统一类型列表：内置默认类型 + 自定义类型（enum/bean）。
    /// </summary>
    Task<IReadOnlyList<DataTypeListItem>> GetUnifiedTypeListAsync(string projectPath);

    /// <summary>
    /// 删除表格：从 __tables__.xlsx 中移除注册条目，并可选地删除对应的物理 xlsx 数据文件。
    /// </summary>
    /// <param name="projectPath">项目根目录。</param>
    /// <param name="fullName">表格的 full_name（即 __tables__.xlsx 中 B 列的值，例如 "cfg.TbItem"）。</param>
    /// <param name="inputRelPath">数据文件相对于 Datas/ 目录的路径（例如 "cfg/Item.xlsx"）。</param>
    /// <param name="deletePhysicalFile">true 则同时删除物理数据文件；false 则仅移除注册条目。</param>
    Task DeleteTableAsync(string projectPath, string fullName, string inputRelPath, bool deletePhysicalFile);
}
