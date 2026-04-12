using System.Collections.Generic;

namespace LubanGui.LubanAdapter.Interfaces;

/// <summary>Luban 内置类型与 GUI 显示名称的双向映射。</summary>
public interface ILubanTypeMapper
{
    IReadOnlyList<LubanTypeDescriptor> GetBuiltinTypes();
    string ToDisplayName(string lubanType);
    string ToLubanType(string displayName);
}

/// <summary>单个类型的描述信息。</summary>
public record LubanTypeDescriptor(
    string LubanType,
    string DisplayName,
    string Category
);
