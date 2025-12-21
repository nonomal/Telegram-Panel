using Microsoft.AspNetCore.Routing;
using TelegramPanel.Modules;

namespace TelegramPanel.Web.Modules;

public sealed class ModuleRegistry
{
    private readonly List<LoadedModule> _modules = new();

    public IReadOnlyList<LoadedModule> Modules => _modules;

    public void Add(LoadedModule module) => _modules.Add(module);

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        foreach (var m in _modules)
        {
            try
            {
                m.Instance.MapEndpoints(endpoints, m.Context);
            }
            catch
            {
                // 模块出错不应影响主站启动；日志在上层打
            }
        }
    }
}

public sealed record LoadedModule(
    string Id,
    string Version,
    bool BuiltIn,
    ITelegramPanelModule Instance,
    ModuleHostContext Context,
    ModuleManifest Manifest,
    string? ModuleRootPath);
