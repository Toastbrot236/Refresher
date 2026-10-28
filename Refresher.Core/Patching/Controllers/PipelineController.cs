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
    private Dictionary<Type, Pipeline> _pipelines { get; } = [];

    /// <summary>
    /// The current pipeline being executed (or the last one that was executed), for stuff like progress bars.
    /// </summary>
    public Pipeline CurrentPipeline { get; protected set; } = null!;

    /// <summary>
    /// Contains data like user-entered inputs, AutoDiscover results, and other data shared and used by the pipelines.
    /// </summary>
    public PipelineContext Context { get; protected init; }
    public IPlatformInterface Platform => this.Context.Platform;
    public bool EnableFormControls => this.CurrentPipeline.State != PipelineState.Running;

    public PipelineController(IPlatformInterface platform) // TODO blacklist
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
        this.AddPipeline(this.MainPipelineType);
        this.AddPipeline(this.RevertPatchPipelineType);
        this.AddPipeline(this.VerifyServerInputPipelineType);
        this.AddPipeline(this.DownloadGameDataPipelineType);

        // Set current pipeline to the main one by default.
        Pipeline? defaultPipeline = this._pipelines.GetValueOrDefault(this.MainPipelineType);
        Debug.Assert(defaultPipeline != null);

        this.CurrentPipeline = defaultPipeline!;
    }

    private void AddPipeline(Type? type)
    {
        if (type == null) return;

        Debug.Assert(type.IsAssignableTo(typeof(Pipeline)));
        if (this._pipelines.Any(p => p.Key == type))
        {
            State.Logger.LogTrace(LogType.PipelineController, $"Pipeline of type {type.Name} is already initialized, skipping addition.");
            return;
        }

        Pipeline pipeline = (Pipeline)Activator.CreateInstance(type, this)!;
        pipeline.Initialize(this.Context);

        this._pipelines.Add(type, pipeline);
    }

    public async Task ExecutePipeline(Type type)
    {
        Pipeline? pipeline = this.GetPipeline(type);
        if (pipeline == null)
        {
            this.Platform.ErrorPrompt($"Pipeline of type {type} was not defined, so we can't execute it!! Please report this as an issue!");
            return;
        }

        this.CurrentPipeline = pipeline;
        await pipeline.ExecuteAsync();
    }

    public Pipeline? GetPipeline(Type type)
    {
        return this._pipelines.GetValueOrDefault(type);
    }
}