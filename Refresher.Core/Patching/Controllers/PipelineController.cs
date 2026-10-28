using System.Diagnostics;
using Refresher.Core.Patching.Pipelines;
using Refresher.Core.Platform;
using Refresher.Core.Storage;

namespace Refresher.Core.Patching.Controllers;

/// <summary>
/// Contains pipelines and other model stuff used by a particular form or page.
/// </summary>
public abstract class PipelineController : IAccessesPlatform
{
    public abstract string Name { get; }
    /// <summary>
    /// Empty = don't do any filtering, show all games regardless of their name
    /// </summary>
    public virtual string[] GameNameWhitelist { get; } = [];

    /// <summary>
    /// Used to get the guide page URL, aswell as the join key URL.
    /// </summary>
    public virtual string ShorthandUrlId { get; } = "";
    public string GuideLink => $"https://docs.littlebigrefresh.com/{this.ShorthandUrlId}";

    // This is only here for compatibility, may or may not get removed soon.
    public bool UsesPatchwork { get; protected init; }

    /// <summary>
    /// Used by the constructor to know what pipeline to default to as the current one.
    /// </summary>
    public abstract Type MainPipelineType { get; }

    /// <summary>
    /// Type of pipeline to use when reverting the patch done by the main pipeline.
    /// Not all our forms have a revert option, so this is nullable.
    /// </summary>
    public virtual Type? RevertPatchPipelineType { get; } = null;

    /// <summary>
    /// Type of pipeline to use when verifying the validity of the user's entered server data (usually just the URL).
    /// This will usually be AutoDiscover, but this way we can also use other pipelines for this if that's ever needed.
    /// </summary>
    public virtual Type? VerifyServerInputPipelineType { get; } = null;

    /// <summary>
    /// Type of pipeline to use when downloading data from the target platform, usually game metadata to show.
    /// </summary>
    public virtual Type? DownloadGameDataPipelineType { get; } = null;

    /// <summary>
    /// The pipelines managed by this controller. No hard-coded fields here to avoid duplicate execution code.
    /// </summary>
    private Dictionary<PipelineCategory, Pipeline> _pipelines { get; } = [];

    /// <summary>
    /// The current pipeline being executed (or the last one that was executed), for stuff like progress bars.
    /// </summary>
    public Pipeline CurrentPipeline { get; protected set; } = null!;

    /// <summary>
    /// Contains data like user-entered inputs, AutoDiscover results, and other data shared and used by the pipelines.
    /// </summary>
    public PipelineContext Context { get; protected set; } = null!;
    public IPlatformInterface Platform => this.Context.Platform;
    public bool EnableFormControls => this.CurrentPipeline.State != PipelineState.Running;

    public PipelineController()
    {
        
    }

    public void Initialize(IPlatformInterface platform)
    {
        this.Context = new()
        {
            Platform = platform,
            GameNameWhitelist = this.GameNameWhitelist,
            UserInputs = PreviousInputStorage.Inputs ?? new(), // load from file if available
        };

        // Build a dictionary mapping type -> pipeline for both the main type and the additional types.
        // This way we don't need to duplicate execution code, and forms can easily either use hardcoded types,
        // or use the controller's public type properties.
        this.AddPipeline(this.MainPipelineType, PipelineCategory.Main);
        this.AddPipeline(this.RevertPatchPipelineType, PipelineCategory.RevertPatch);
        this.AddPipeline(this.VerifyServerInputPipelineType, PipelineCategory.VerifyServerInput);
        this.AddPipeline(this.DownloadGameDataPipelineType, PipelineCategory.DownloadGameData);

        // Set current pipeline to the main one by default.
        Pipeline? defaultPipeline = this.GetPipeline(PipelineCategory.Main);
        Debug.Assert(defaultPipeline != null);

        this.CurrentPipeline = defaultPipeline!;
    }

    private void AddPipeline(Type? type, PipelineCategory category)
    {
        if (type == null) return;

        Debug.Assert(type.IsAssignableTo(typeof(Pipeline)));
        if (this._pipelines.Any(p => p.Key == category))
        {
            State.Logger.LogTrace(LogType.PipelineController, $"Pipeline of category {category} is already initialized, skipping addition.");
            return;
        }

        Pipeline pipeline = (Pipeline)Activator.CreateInstance(type, this)!;
        pipeline.Initialize(this.Context);

        this._pipelines.Add(category, pipeline);
    }

    public async Task ExecutePipeline(PipelineCategory category)
    {
        Pipeline? pipeline = this.GetPipeline(category);
        if (pipeline == null)
        {
            this.Platform.ErrorPrompt($"Pipeline of category {category} was not defined, so we can't execute it! Please report this as an issue!");
            return;
        }

        this.CurrentPipeline = pipeline;
        await pipeline.ExecuteAsync();
    }

    public Pipeline? GetPipeline(PipelineCategory category)
    {
        return this._pipelines.GetValueOrDefault(category);
    }
}