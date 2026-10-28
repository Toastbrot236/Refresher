using Refresher.Core.Patching.Steps.Common;

namespace Refresher.Core.Patching.Pipelines.Lbp;

public abstract class PatchworkConfigPipeline : Pipeline
{
    public override string Name => $"Patchwork {this.ConsoleName} Config";

    public override string? ShorthandUrlId => this.ConsoleName.ToLower();

    protected abstract string ConsoleName { get; }

    protected abstract override Type? SetupAccessorStepType { get; }

    protected override List<Type> StepTypes =>
    [
        typeof(UploadPatchworkSprxStep),
        typeof(UploadPatchworkConfigurationStep),
    ];

    public override string GetStateText()
    {
        return this.State switch
        {
            // Most pipelines are patch pipelines, so default to terms related to patching
            PipelineState.NotStarted => "Apply changes!",
            PipelineState.Running => "Applying changes... (click to cancel)",
            PipelineState.Finished => $"Changes applied! (click to re-apply)",
            PipelineState.Cancelled => "Apply changes!",
            PipelineState.Error => "Retry (Failed to apply changes)",
            _ => "Apply changes!",
        };
    }
}