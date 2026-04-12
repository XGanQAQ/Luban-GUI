using System;
using System.Collections.Generic;

namespace LubanGui.LubanAdapter;

/// <summary>
/// 在应用启动时一次性初始化 Luban 的所有内部 Manager 和 Behaviour 注册。
/// 幂等：多次调用只执行一次。
/// </summary>
public static class LubanAdapterInitializer
{
    private static bool _initialized = false;
    private static readonly object _lock = new();

    public static void Initialize()
    {
        if (_initialized) return;
        lock (_lock)
        {
            if (_initialized) return;

            // SimpleLauncher 完成：
            //   1. 创建空 EnvManager
            //   2. 初始化 SchemaManager、DataLoaderManager、CustomBehaviourManager 等
            //   3. 扫描带 [assembly:RegisterBehaviour] 的所有 Luban DLL 并注册行为
            new Luban.SimpleLauncher().Start(new Dictionary<string, string>());

            _initialized = true;
        }
    }
}
