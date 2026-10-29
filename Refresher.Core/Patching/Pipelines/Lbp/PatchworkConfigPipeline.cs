using Refresher.Core.Patching.Steps.Common;

namespace Refresher.Core.Patching.Pipelines.Lbp;

public abstract class PatchworkConfigPipeline : Pipeline
{
    public override string Name => $"Reconfigure Patchwork";
    public override bool NeedsAccessor => true;

    protected override List<Type> StepTypes =>
    [
        typeof(UploadPatchworkSprxStep),
        typeof(UploadPatchworkConfigurationStep),
    ];

    public override string GetStateText()
    {
        return this.State switch
        {
            PipelineState.NotStarted => "Apply changes!",
            PipelineState.Running => "Applying changes... (click to cancel)",
            PipelineState.Finished => $"Changes applied! (click to re-apply)",
            PipelineState.Cancelled => "Apply changes!",
            PipelineState.Error => "Retry (Failed to apply changes)",
            _ => "Apply changes!",
        };
    }
}