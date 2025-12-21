using System.Text.Json;

namespace TelegramPanel.Web.Modules;

public sealed class ModuleStateStore
{
    private readonly ModuleLayout _layout;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public ModuleStateStore(ModuleLayout layout)
    {
        _layout = layout;
    }

    public async Task<ModuleState> LoadAsync()
    {
        EnsureDirectories();

        if (!File.Exists(_layout.StateFile))
            return new ModuleState();

        var json = await File.ReadAllTextAsync(_layout.StateFile);
        if (string.IsNullOrWhiteSpace(json))
            return new ModuleState();

        return JsonSerializer.Deserialize<ModuleState>(json) ?? new ModuleState();
    }

    public async Task SaveAsync(ModuleState state)
    {
        EnsureDirectories();

        state.SchemaVersion = 1;
        state.Modules ??= new List<ModuleStateItem>();

        var json = JsonSerializer.Serialize(state, _jsonOptions);
        var target = _layout.StateFile;
        var dir = Path.GetDirectoryName(target) ?? _layout.Root;
        Directory.CreateDirectory(dir);

        var temp = target + ".tmp";
        await File.WriteAllTextAsync(temp, json, new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        if (File.Exists(target))
            File.Replace(temp, target, destinationBackupFileName: null);
        else
            File.Move(temp, target);
    }

    private void EnsureDirectories()
    {
        Directory.CreateDirectory(_layout.Root);
        Directory.CreateDirectory(_layout.PackagesDir);
        Directory.CreateDirectory(_layout.InstalledDir);
        Directory.CreateDirectory(_layout.ActiveDir);
        Directory.CreateDirectory(_layout.StagingDir);
        Directory.CreateDirectory(_layout.TrashDir);
    }
}

public sealed class ModuleState
{
    public int SchemaVersion { get; set; } = 1;
    public List<ModuleStateItem> Modules { get; set; } = new();
}

public sealed class ModuleStateItem
{
    public string Id { get; set; } = "";
    public bool Enabled { get; set; }
    public string? ActiveVersion { get; set; }
    public string? LastGoodVersion { get; set; }
    public List<string> InstalledVersions { get; set; } = new();

    public bool BuiltIn { get; set; }
}

