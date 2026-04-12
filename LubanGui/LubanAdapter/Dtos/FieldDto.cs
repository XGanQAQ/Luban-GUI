using System.Collections.Generic;

namespace LubanGui.LubanAdapter.Dtos;

/// <summary>表格/Bean 中的单个字段定义，对应 Luban RawField。</summary>
public record FieldDto(
    string Name,
    string Alias,
    string Type,
    string Comment,
    IReadOnlyList<string> Groups,
    IReadOnlyDictionary<string, string> Tags
);
