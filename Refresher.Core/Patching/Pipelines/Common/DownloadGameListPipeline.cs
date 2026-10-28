using Refresher.Core.Patching.Steps.Common;

namespace Refresher.Core.Patching.Pipelines.Common;

public class DownloadGameListPipeline : Pipeline
{
    public override string Name => "Downloading your Game list";

    public override bool ReplacesEboot => true;

    protected override List<Type> StepTypes =>
    [
        typeof(DownloadGameListStep),
    ];

    protected override Type? SetupAccessorStepType { get; }

    public DownloadGameListPipeline(Type setupAccessorStepType)
    {
        this.SetupAccessorStepType = setupAccessorStepType;
    }

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