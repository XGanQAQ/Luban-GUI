using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LubanGui.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;

namespace LubanGui.ViewModels;

public partial class AppSettingsViewModel : ViewModelBase
{
    private readonly AppConfigManager _configManager;

    /// <summary>设计时构造。</summary>
    public AppSettingsViewModel() : this(new AppConfigManager(NullLogger<AppConfigManager>.Instance)) { }

    [ObservableProperty]
    private bool _mcpAutoStart;

    [ObservableProperty]
    private int _mcpPort = 62930;

    [ObservableProperty]
    private bool _deleteTablePhysicalFileByDefault;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public event EventHandler<bool?>? CloseRequested;

    public AppSettingsViewModel(AppConfigManager configManager)
    {
        _configManager = configManager;
    }

    public void LoadFromConfig()
    {
        McpAutoStart = _configManager.GetMcpAutoStart();
        McpPort = _configManager.GetMcpPort();
        DeleteTablePhysicalFileByDefault = _configManager.GetDeleteTablePhysicalFileByDefault();
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private async Task Save()
    {
        ErrorMessage = string.Empty;

        if (McpPort is < 1024 or > 65535)
        {
            ErrorMessage = "MCP 端口必须在 1024-65535 之间";
            return;
        }

        await _configManager.SetMcpAutoStartAsync(McpAutoStart);
        await _configManager.SetMcpPortAsync(McpPort);
        await _configManager.SetDeleteTablePhysicalFileByDefaultAsync(DeleteTablePhysicalFileByDefault);

        CloseRequested?.Invoke(this, true);
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseRequested?.Invoke(this, null);
    }
}
