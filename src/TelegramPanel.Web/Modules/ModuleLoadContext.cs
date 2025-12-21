using System.Reflection;
using System.Runtime.Loader;

namespace TelegramPanel.Web.Modules;

public sealed class ModuleLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;
    private readonly string _baseDir;

    public ModuleLoadContext(string mainAssemblyPath) : base(isCollectible: false)
    {
        if (string.IsNullOrWhiteSpace(mainAssemblyPath))
            throw new ArgumentException("mainAssemblyPath 不能为空", nameof(mainAssemblyPath));

        _resolver = new AssemblyDependencyResolver(mainAssemblyPath);
        _baseDir = Path.GetDirectoryName(mainAssemblyPath) ?? "";
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var path = _resolver.ResolveAssemblyToPath(assemblyName);
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            return LoadFromAssemblyPath(path);

        // fallback：如果模块未提供 deps.json，则在 lib 目录按名称寻找
        var name = (assemblyName.Name ?? "").Trim();
        if (name.Length == 0 || string.IsNullOrWhiteSpace(_baseDir))
            return null;

        var candidate = Path.Combine(_baseDir, name + ".dll");
        if (File.Exists(candidate))
            return LoadFromAssemblyPath(candidate);

        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            return LoadUnmanagedDllFromPath(path);

        return base.LoadUnmanagedDll(unmanagedDllName);
    }
}
