using Refresher.Core.Patching.Steps.Common;

namespace Refresher.Core.Patching.Pipelines.Common;

public class DownloadGameListPipeline : Pipeline
{
    public override string Name => "Downloading your Game list";
    public override bool NeedsAccessor => true;

    protected override List<Type> StepTypes =>
    [
        typeof(DownloadGameListStep),
        // Might want to probably execute more steps here in the future
        // (e.g. download patchwork config before/after game list so the user could choose whether to 
        // fill in options from there instead of our local inputs.ini), so having this be in its own pipelines
        // might get more usecases than just simplicity soon.
    ];

    public override string GetStateText()
    {
        return this.State switch
        {
            PipelineState.NotStarted => "Download game data",
            PipelineState.Running => "Downloading game data... (click to cancel)",
            PipelineState.Finished => $"Game data download finished",
            PipelineState.Cancelled => "Download game data",
            PipelineState.Error => "Retry (Game data download failed)",
            _ => "Download game data",
        };
    }
}