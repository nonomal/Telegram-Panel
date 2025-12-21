using System.Text.Json.Serialization;

namespace TelegramPanel.Modules;

public interface IModuleUiProvider
{
    IEnumerable<ModuleNavItem> GetNavItems(ModuleHostContext context);
    IEnumerable<ModulePageDefinition> GetPages(ModuleHostContext context);
}

public sealed class ModuleNavItem
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("href")]
    public string Href { get; set; } = "";

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = "";

    [JsonPropertyName("group")]
    public string? Group { get; set; }

    [JsonPropertyName("order")]
    public int Order { get; set; } = 0;
}

public sealed class ModulePageDefinition
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = "";

    /// <summary>
    /// 组件类型的 AssemblyQualifiedName（用于 DynamicComponent 渲染）。
    /// </summary>
    [JsonPropertyName("componentType")]
    public string ComponentType { get; set; } = "";

    [JsonPropertyName("group")]
    public string? Group { get; set; }

    [JsonPropertyName("order")]
    public int Order { get; set; } = 0;
}

public interface IModuleTaskProvider
{
    IEnumerable<ModuleTaskDefinition> GetTasks(ModuleHostContext context);
}

public sealed class ModuleTaskDefinition
{
    /// <summary>
    /// 任务分类：例如 user / bot / system（建议使用小写）。
    /// </summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    /// <summary>
    /// 任务类型常量（数据库 BatchTask.TaskType）。
    /// </summary>
    [JsonPropertyName("taskType")]
    public string TaskType { get; set; } = "";

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = "";

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = "";

    /// <summary>
    /// 如果提供了创建页面路由，则任务中心“新建任务”会跳转到该页面创建。
    /// </summary>
    [JsonPropertyName("createRoute")]
    public string? CreateRoute { get; set; }

    /// <summary>
    /// 任务创建编辑器组件类型 AssemblyQualifiedName（可选）。\n    /// 该组件需要支持参数：\n    /// - Draft (ModuleTaskDraft)\n    /// - DraftChanged (EventCallback&lt;ModuleTaskDraft&gt;)\n    /// </summary>
    [JsonPropertyName("editorComponentType")]
    public string? EditorComponentType { get; set; }

    [JsonPropertyName("order")]
    public int Order { get; set; } = 0;
}

public readonly record struct ModuleTaskDraft(int Total, string? Config, bool CanSubmit, string? ValidationError);

public interface IModuleApiProvider
{
    IEnumerable<ModuleApiTypeDefinition> GetApis(ModuleHostContext context);
}

public sealed class ModuleApiTypeDefinition
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = "";

    [JsonPropertyName("route")]
    public string Route { get; set; } = "";

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("order")]
    public int Order { get; set; } = 0;
}

public interface IModuleTaskHandler
{
    string TaskType { get; }
    Task ExecuteAsync(IModuleTaskExecutionHost host, CancellationToken cancellationToken);
}

public interface IModuleTaskExecutionHost
{
    int TaskId { get; }
    string TaskType { get; }
    int Total { get; }
    string? Config { get; }

    IServiceProvider Services { get; }

    Task<bool> IsStillRunningAsync(CancellationToken cancellationToken);
    Task UpdateProgressAsync(int completed, int failed, CancellationToken cancellationToken);
}

