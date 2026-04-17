using System;
using System.Collections.Generic;

namespace LubanGui.Services;

/// <summary>
/// v0.2 容器类型合法性校验器。
/// 定义了合法的容器语法白名单与嵌套层级限制（最多 1 级容器）。
/// </summary>
internal static class ContainerTypeValidator
{
    /// <summary>Luban 内置基础类型白名单。</summary>
    private static readonly HashSet<string> s_primitiveTypes = new(StringComparer.Ordinal)
    {
        "bool", "byte", "short", "int", "long", "sbyte", "ushort", "uint", "ulong",
        "float", "double", "string", "bytes", "text",
    };

    /// <summary>v0.2 支持的容器关键字白名单（最多 1 级嵌套）。</summary>
    private static readonly HashSet<string> s_containerKeywords = new(StringComparer.Ordinal)
    {
        "array", "list", "set", "map",
    };

    /// <summary>
    /// 校验字段类型表达式。
    /// </summary>
    /// <returns>
    /// <c>null</c> 表示合法；否则返回可读的错误描述。
    /// </returns>
    public static string? Validate(string typeExpr)
    {
        if (string.IsNullOrWhiteSpace(typeExpr))
            return "类型不能为空";

        var parts = typeExpr.Trim().Split(',');
        var head = parts[0].Trim();

        if (!s_containerKeywords.Contains(head))
        {
            // 非容器关键字：直接判断为简单类型
            return IsValidSimpleType(head)
                ? null
                : $"未知类型 '{head}'，应为内置基础类型或合法的自定义类型名（如 cfg.MyEnum）";
        }

        return head switch
        {
            "array" or "list" or "set" => ValidateListLike(head, parts),
            "map"                       => ValidateMap(parts),
            _                           => $"不支持的容器类型 '{head}'",
        };
    }

    /// <summary>
    /// 构建供 AutoCompleteBox 使用的类型建议列表：
    /// 内置基础类型 + 常用容器模板 + 传入的自定义类型名。
    /// </summary>
    public static IReadOnlyList<string> BuildTypeSuggestions(IReadOnlyList<string> customTypes)
    {
        var list = new List<string>
        {
            // 基础类型（高频在前）
            "int", "long", "float", "double", "bool", "string",
            "byte", "short", "sbyte", "ushort", "uint", "ulong", "bytes", "text",
            // 常用容器模板
            "list,int", "list,long", "list,float", "list,string",
            "array,int", "array,string",
            "set,int",   "set,string",
            "map,int,int", "map,int,string", "map,string,string",
        };
        list.AddRange(customTypes);
        return list;
    }

    // ──────────────────────────────────────────────────────────────
    // 内部校验辅助
    // ──────────────────────────────────────────────────────────────

    private static string? ValidateListLike(string keyword, string[] parts)
    {
        if (parts.Length != 2)
            return $"'{keyword}' 需要恰好 1 个元素类型，写法：{keyword},<元素类型>（收到 {parts.Length - 1} 个参数）";

        var elem = parts[1].Trim();
        if (s_containerKeywords.Contains(elem))
            return $"v0.2 不支持嵌套容器（{keyword} 的元素不能是容器类型 '{elem}'）";

        if (!IsValidSimpleType(elem))
            return $"'{keyword}' 的元素类型 '{elem}' 不合法，应为内置基础类型或自定义类型名";

        return null;
    }

    private static string? ValidateMap(string[] parts)
    {
        if (parts.Length != 3)
            return $"'map' 需要恰好 2 个类型参数，写法：map,<键类型>,<值类型>（收到 {parts.Length - 1} 个参数）";

        var key = parts[1].Trim();
        var val = parts[2].Trim();

        if (s_containerKeywords.Contains(key))
            return $"map 的键类型不能是容器类型 '{key}'";
        if (!IsValidSimpleType(key))
            return $"map 的键类型 '{key}' 不合法，应为内置基础类型或自定义类型名";

        if (s_containerKeywords.Contains(val))
            return $"v0.2 不支持嵌套容器，map 的值类型不能是容器 '{val}'";
        if (!IsValidSimpleType(val))
            return $"map 的值类型 '{val}' 不合法，应为内置基础类型或自定义类型名";

        return null;
    }

    private static bool IsValidSimpleType(string name) =>
        s_primitiveTypes.Contains(name) || IsCustomTypeName(name);

    /// <summary>
    /// 判断名称是否为合法的自定义类型标识符（支持命名空间点分格式，如 cfg.Item）。
    /// </summary>
    private static bool IsCustomTypeName(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        var parts = name.Split('.');
        foreach (var p in parts)
        {
            if (string.IsNullOrEmpty(p)) return false;
            if (!char.IsLetter(p[0])) return false;
            foreach (var c in p)
                if (!char.IsLetterOrDigit(c) && c != '_') return false;
        }
        return true;
    }
}
