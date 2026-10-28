using Refresher.Core.Platform;
using Refresher.Core.Verification.AutoDiscover;

namespace Refresher.Core.Patching.Pipelines;

/// <summary>
/// Pipeline-independent data used and set by pipeline steps, or the GUI controller
/// </summary>
public class PipelineContext
{
    /// <summary>
    /// Platform specific stuff for GUI operations, like showing prompts. Set by PipelineController.
    /// </summary>
    public IPlatformInterface Platform { get; set; } = null!;

    /// <summary>
    /// List of game data downloaded by a pipeline using a platform accessor. Null = not downloaded yet;
    /// empty = user has no games.
    /// </summary>
    internal List<GameInformation>? GameList { get; set; }

    /// <summary>
    /// Server-specific patch data downloaded from the user's specified server by a pipeline.
    /// </summary>
    public AutoDiscoverResponse? AutoDiscover { get; set; }

    /// <summary>
    /// Everything the user has entered via the GUI. Initialized by the PipelineController by either
    /// loading from file, or creating a new object.
    /// </summary>
    public UserEnteredInputs UserInputs { get; set; } = null!;

    /// <summary>
    /// Allow GUI controller to specify whitelisted words in game names if wanted. Empty = Don't do any filtering
    /// </summary>
    // TODO also allow blacklisting using a separate array
    public string[] GameNameWhitelist { get; set; } = [];

    /// <summary>
    /// Prefers the URL from AutoDiscover, but will fall back to the user-entered one.
    /// </summary>
    public string GetServerUrl()
        => this.AutoDiscover?.Url ?? this.UserInputs.EnteredServerUrl ?? "";
}