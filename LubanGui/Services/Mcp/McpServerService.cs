using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LubanGui.Infrastructure;
using LubanGui.LubanAdapter.Interfaces;
using LubanGui.Services.Luban;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace LubanGui.Services.Mcp;

public class McpServerService
{
    private readonly ILogger<McpServerService> _logger;
    private readonly AppConfigManager _configManager;
    private readonly IProjectManager _projectManager;
    private readonly ISchemaService _schemaService;
    private readonly ITablePreviewService _tablePreviewService;
    private readonly IExportService _exportService;
    private readonly ProjectConfigManager _projectConfigManager;
    private readonly ILubanConfAdapter _lubanConfAdapter;

    private WebApplication? _app;
    private Task? _runTask;
    private CancellationTokenSource? _cts;

    public bool IsRunning { get; private set; }
    public int Port { get; private set; }
    public string Url => IsRunning ? $"http://localhost:{Port}/mcp" : string.Empty;

    public event Action<bool, int>? StatusChanged;

    public McpServerService(
        ILogger<McpServerService> logger,
        AppConfigManager configManager,
        IProjectManager projectManager,
        ISchemaService schemaService,
        ITablePreviewService tablePreviewService,
        IExportService exportService,
        ProjectConfigManager projectConfigManager,
        ILubanConfAdapter lubanConfAdapter)
    {
        _logger = logger;
        _configManager = configManager;
        _projectManager = projectManager;
        _schemaService = schemaService;
        _tablePreviewService = tablePreviewService;
        _exportService = exportService;
        _projectConfigManager = projectConfigManager;
        _lubanConfAdapter = lubanConfAdapter;
    }

    public async Task StartAsync(int? preferredPort = null)
    {
        if (IsRunning)
        {
            _logger.LogWarning("MCP 服务器已在运行中 ({Url})", Url);
            return;
        }

        var port = preferredPort ?? _configManager.GetMcpPort();

        port = await FindAvailablePortAsync(port);
        if (port < 0)
        {
            _logger.LogError("无法找到可用端口，MCP 服务器启动失败");
            return;
        }

        var builder = WebApplication.CreateBuilder();

        builder.Logging.ClearProviders();
        builder.Logging.AddFilter(_ => false);

        builder.Services.AddSingleton(_projectManager);
        builder.Services.AddSingleton(_schemaService);
        builder.Services.AddSingleton(_tablePreviewService);
        builder.Services.AddSingleton(_exportService);
        builder.Services.AddSingleton(_projectConfigManager);
        builder.Services.AddSingleton(_lubanConfAdapter);
        builder.Services.AddSingleton(_logger);

        builder.Services.AddSingleton<McpTools>();

        builder.Services.AddMcpServer()
            .WithHttpTransport(options =>
            {
                options.Stateless = true;
            })
            .WithToolsFromAssembly();

        _app = builder.Build();
        _app.MapMcp("/mcp");

        _cts = new CancellationTokenSource();
        var url = $"http://localhost:{port}";

        try
        {
            _runTask = _app.RunAsync(url);
            IsRunning = true;
            Port = port;
            _logger.LogInformation("MCP 服务器已启动: {Url}", Url);
            StatusChanged?.Invoke(true, port);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MCP 服务器启动失败 (端口 {Port})", port);
            await _app.DisposeAsync();
            _app = null;
        }
    }

    public async Task StopAsync()
    {
        if (!IsRunning || _app == null)
        {
            return;
        }

        try
        {
            _cts?.Cancel();
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "停止 MCP 服务器时出现异常");
        }
        finally
        {
            _app = null;
            _cts = null;
            IsRunning = false;
            _logger.LogInformation("MCP 服务器已停止");
            StatusChanged?.Invoke(false, 0);
        }
    }

    private static async Task<int> FindAvailablePortAsync(int startPort, int maxAttempts = 10)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            var port = startPort + i;
            if (await IsPortAvailableAsync(port))
            {
                return port;
            }
        }

        return -1;
    }

    private static async Task<bool> IsPortAvailableAsync(int port)
    {
        try
        {
            using var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            listener.Stop();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
