using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using LubanGui.Infrastructure;
using LubanGui.LubanAdapter;
using LubanGui.LubanAdapter.Interfaces;
using LubanGui.Services;
using LubanGui.Services.Luban;
using LubanGui.ViewModels;
using LubanGui.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LubanGui;

public partial class App : Application
{
    internal IServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // 初始化 Serilog：输出到控制台 + 滚动日志文件
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/luban-gui-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("Luban GUI 启动中...");

        // 一次性初始化 Luban 内部 Manager（SchemaManager, DataLoaderManager 等）
        LubanAdapterInitializer.Initialize();

        // 配置依赖注入容器
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // 桥接 Serilog 到 Microsoft.Extensions.Logging.ILogger<T>
        services.AddLogging(builder => builder.AddSerilog(dispose: true));

        // 基础设施层
        services.AddSingleton<AppConfigManager>();
        services.AddSingleton<ProjectConfigManager>();
        services.AddSingleton<FileOpenService>();

        // 业务逻辑层
        services.AddSingleton<IProjectManager, ProjectManager>();
        services.AddSingleton<ISchemaService, SchemaService>();
        services.AddSingleton<ITablePreviewService, TablePreviewService>();
        services.AddSingleton<ILubanExecutor, LubanExecutor>();
        services.AddSingleton<IExportService, ExportService>();

        // Luban 源适配层
        services.AddSingleton<ILubanSchemaReader, LubanSchemaReader>();
        services.AddSingleton<ILubanConfAdapter, LubanConfAdapter>();
        services.AddSingleton<ILubanTypeMapper, LubanTypeMapper>();

        // 注册 ViewModel（手动注入所有依赖）
        services.AddSingleton<MainWindowViewModel>(sp => new MainWindowViewModel(
            sp.GetRequiredService<ILogger<MainWindowViewModel>>(),
            sp.GetRequiredService<IProjectManager>(),
            sp.GetRequiredService<ISchemaService>(),
            sp.GetRequiredService<ITablePreviewService>(),
            sp.GetRequiredService<FileOpenService>(),
            sp.GetRequiredService<IExportService>(),
            sp.GetRequiredService<ProjectConfigManager>()));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            var viewModel = _serviceProvider!.GetRequiredService<MainWindowViewModel>();
            var projectManager = _serviceProvider!.GetRequiredService<IProjectManager>();

            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel,
            };

            // 异步初始化：加载持久化的项目列表并恢复上次打开的项目
            desktop.MainWindow.Opened += async (_, _) =>
            {
                await projectManager.InitializeAsync();
                viewModel.SyncProjectsFromManager();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}