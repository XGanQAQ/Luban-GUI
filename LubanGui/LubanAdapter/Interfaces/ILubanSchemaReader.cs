using System.Collections.Generic;
using System.Threading.Tasks;
using LubanGui.LubanAdapter.Dtos;

namespace LubanGui.LubanAdapter.Interfaces;

/// <summary>
/// 从 luban.conf 及其引用的 Schema 文件中读取表、枚举、结构体的定义。
/// 本接口是 GUI 访问 Luban Schema 的唯一入口。
/// </summary>
public interface ILubanSchemaReader
{
    /// <summary>读取所有表格 Schema。若配置不存在则返回空列表。</summary>
    Task<IReadOnlyList<TableSchemaDto>> ReadTablesAsync(string confPath);

    /// <summary>读取所有枚举 Schema。若配置不存在则返回空列表。</summary>
    Task<IReadOnlyList<EnumSchemaDto>> ReadEnumsAsync(string confPath);

    /// <summary>读取所有结构体 Schema。若配置不存在则返回空列表。</summary>
    Task<IReadOnlyList<BeanSchemaDto>> ReadBeansAsync(string confPath);
}
