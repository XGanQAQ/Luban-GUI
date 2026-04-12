using System;
using System.Threading;
using System.Threading.Tasks;

namespace LubanGui.Services.Luban;

/// <summary>
/// 启动并管理 Luban CLI 子进程。
/// </summary>
public interface ILubanExecutor
{
    /// <summary>
    /// 以异步方式运行 Luban CLI，实时通过 <paramref name="progress"/> 推送每行输出。
    /// </summary>
    /// <param name="lubanExePath">Luban 可执行文件路径（.exe 或 .dll）。</param>
    /// <param name="args">CLI 参数列表，不含可执行文件名。</param>
    /// <param name="progress">进度回调，每收到一行输出就调用一次。</param>
    /// <param name="ct">取消令牌；触发时强制终止子进程。</param>
    /// <returns>子进程退出码（0 = 成功）。</returns>
    Task<int> RunAsync(
        string lubanExePath,
        string[] args,
        IProgress<string> progress,
        CancellationToken ct);
}
