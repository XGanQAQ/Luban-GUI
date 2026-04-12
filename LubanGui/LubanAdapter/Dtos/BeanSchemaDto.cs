using System.Collections.Generic;

namespace LubanGui.LubanAdapter.Dtos;

/// <summary>结构体 Schema，对应 Luban RawBean。</summary>
public record BeanSchemaDto(
    string FullName,
    string Parent,
    bool IsValueType,
    string Comment,
    string Sep,
    IReadOnlyList<FieldDto> Fields
);
