using System.Collections.Generic;
using System.Threading.Tasks;

namespace LubanGui.LubanAdapter.Interfaces;

/// <summary>读写 Luban 主配置文件（luban.conf）的适配接口。</summary>
public interface ILubanConfAdapter
{
    Task<LubanConfDto> ReadAsync(string confPath);
    Task WriteAsync(string confPath, LubanConfDto dto);
    Task CreateDefaultAsync(string confPath);
}

/// <summary>luban.conf 的 GUI 视图。</summary>
public record LubanConfDto(
    string Target,
    IReadOnlyList<string> CodeTargets,
    IReadOnlyList<string> DataTargets,
    string TopModule,
    IReadOnlyList<string> InputDataDirs,
    IReadOnlyDictionary<string, string> Groups,
    IReadOnlyList<string> TargetNames
);
