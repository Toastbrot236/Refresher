using Refresher.Core.Patching.Steps.Common;

namespace Refresher.Core.Patching.Pipelines.Common;

public class AutoDiscoverPipeline : Pipeline
{
    public override string Name => "AutoDiscover";

    protected override List<Type> StepTypes =>
    [
        typeof(ExecuteAutoDiscoverStep),
    ];

    public override string GetStateText()
    {
        return this.State switch
        {
            // Most pipelines are patch pipelines, so default to terms related to patching
            PipelineState.NotStarted => "AutoDiscover",
            PipelineState.Running => "Searching for server... (click to cancel)",
            PipelineState.Finished => $"Server found! (Connected to '{this.Context.AutoDiscover?.ServerBrand ?? this.Context.GetServerUrl()}')",
            PipelineState.Cancelled => "AutoDiscover",
            PipelineState.Error => "Retry (AutoDiscover failed)",
            _ => "AutoDiscover",
        };
    }
}