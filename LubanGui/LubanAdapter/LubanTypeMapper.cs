using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LubanGui.LubanAdapter.Interfaces;

namespace LubanGui.LubanAdapter;

/// <summary>
/// Luban 内置类型与 GUI 显示名称的双向映射表。
/// </summary>
public class LubanTypeMapper : ILubanTypeMapper
{
    private static readonly IReadOnlyList<LubanTypeDescriptor> _types = new ReadOnlyCollection<LubanTypeDescriptor>(
    [
        // 基础类型
        new("bool",   "布尔 (bool)",   "基础类型"),
        new("byte",   "字节 (byte)",   "基础类型"),
        new("short",  "短整数 (short)", "基础类型"),
        new("int",    "整数 (int)",    "基础类型"),
        new("long",   "长整数 (long)", "基础类型"),
        new("float",  "单精度 (float)", "基础类型"),
        new("double", "双精度 (double)", "基础类型"),
        new("string", "字符串 (string)", "基础类型"),
        new("text",   "本地化文本 (text)", "基础类型"),
        new("bytes",  "字节数组 (bytes)", "基础类型"),
        // 集合类型
        new("list,int",    "整数列表 (list,int)",    "集合类型"),
        new("list,string", "字符串列表 (list,string)", "集合类型"),
        new("set,int",     "整数集合 (set,int)",     "集合类型"),
        new("set,string",  "字符串集合 (set,string)",  "集合类型"),
        new("map,int,string", "映射 (map,int,string)", "集合类型"),
        // 特殊类型
        new("vector2", "二维向量 (vector2)", "特殊类型"),
        new("vector3", "三维向量 (vector3)", "特殊类型"),
        new("vector4", "四维向量 (vector4)", "特殊类型"),
        new("datetime", "日期时间 (datetime)", "特殊类型"),
    ]);

    private static readonly IReadOnlyDictionary<string, string> _toDisplay =
        _types.ToDictionary(d => d.LubanType, d => d.DisplayName);

    private static readonly IReadOnlyDictionary<string, string> _fromDisplay =
        _types.ToDictionary(d => d.DisplayName, d => d.LubanType);

    public IReadOnlyList<LubanTypeDescriptor> GetBuiltinTypes() => _types;

    public string ToDisplayName(string lubanType) =>
        _toDisplay.TryGetValue(lubanType, out var v) ? v : lubanType;

    public string ToLubanType(string displayName) =>
        _fromDisplay.TryGetValue(displayName, out var v) ? v : displayName;
}
