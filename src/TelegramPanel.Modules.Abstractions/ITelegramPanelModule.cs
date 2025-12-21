using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace TelegramPanel.Modules;

public interface ITelegramPanelModule
{
    ModuleManifest Manifest { get; }

    /// <summary>
    /// 可选：模块注入自身服务（注意：启用/停用通常需要重启才能生效）。
    /// </summary>
    void ConfigureServices(IServiceCollection services, ModuleHostContext context);

    /// <summary>
    /// 可选：模块映射 API endpoints。
    /// </summary>
    void MapEndpoints(IEndpointRouteBuilder endpoints, ModuleHostContext context);
}

public sealed class ModuleHostContext
{
    public ModuleHostContext(string hostVersion, string modulesRootPath)
    {
        HostVersion = hostVersion;
        ModulesRootPath = modulesRootPath;
    }

    public string HostVersion { get; }
    public string ModulesRootPath { get; }
}

