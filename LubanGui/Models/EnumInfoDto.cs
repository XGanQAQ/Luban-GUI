using System.Collections.Generic;

namespace LubanGui.Models;

public record EnumInfoDto(
    string FullName,
    bool IsFlags,
    bool IsUnique,
    string Comment,
    IReadOnlyList<EnumItemDefinition> Items
);
