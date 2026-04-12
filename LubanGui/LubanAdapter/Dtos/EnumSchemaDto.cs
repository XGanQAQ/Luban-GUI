using System.Collections.Generic;

namespace LubanGui.LubanAdapter.Dtos;

/// <summary>枚举项，对应 Luban EnumItem。</summary>
public record EnumItemDto(
    string Name,
    string Alias,
    string Value,
    string Comment
);

/// <summary>枚举 Schema，对应 Luban RawEnum。</summary>
public record EnumSchemaDto(
    string FullName,
    bool IsFlags,
    string Comment,
    IReadOnlyList<EnumItemDto> Items
);
