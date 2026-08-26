using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TelegramPanel.Core.BatchTasks;
using TelegramPanel.Core.Services;
using TelegramPanel.Modules;

namespace TelegramPanel.Web.Services;

/// <summary>
/// 账号数据同步任务处理器。用于恢复容器重启后遗留的 account_auto_sync 待执行任务。
/// </summary>
public sealed class AccountAutoSyncTaskHandler : IModuleTaskHandler
{
    public string TaskType => BatchTaskTypes.AccountAutoSync;

    public async Task ExecuteAsync(IModuleTaskExecutionHost host, CancellationToken cancellationToken)
    {
        var logger = host.Services.GetRequiredService<ILogger<AccountAutoSyncTaskHandler>>();
        var dataSync = host.Services.GetRequiredService<DataSyncService>();
        var trigger = ReadTrigger(host.Config);

        try
        {
            await dataSync.ExecuteTrackedSyncAsync(host.TaskId, trigger, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "Account auto sync task interrupted by shutdown and will be recovered on next start: {TaskId}",
                host.TaskId);
            throw;
        }
    }

    private static string ReadTrigger(string? rawConfig)
    {
        var raw = (rawConfig ?? string.Empty).Trim();
        if (raw.Length == 0)
            return "background";

        try
        {
            var root = JsonNode.Parse(raw);
            var trigger = root?["trigger"]?.GetValue<string>()?.Trim();
            return string.IsNullOrWhiteSpace(trigger) ? "background" : trigger;
        }
        catch (JsonException)
        {
            return "background";
        }
    }
}
