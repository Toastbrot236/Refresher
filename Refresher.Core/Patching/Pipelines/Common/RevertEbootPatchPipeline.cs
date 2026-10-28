using Refresher.Core.Patching.Steps.Common;

namespace Refresher.Core.Patching.Pipelines.Common;

public class RevertEbootPatchPipeline : Pipeline
{
    public override string Name => "Revert EBOOT Patch";

    public override bool ReplacesEboot => true;

    protected override List<Type> StepTypes =>
    [
        // Info gathering stage
        typeof(ValidateGameStep),
        typeof(RevertGameEbootFromBackupStep),
    ];

    protected override Type? SetupAccessorStepType { get; }

    public RevertEbootPatchPipeline(Type setupAccessorStepType)
    {
        this.SetupAccessorStepType = setupAccessorStepType;
    }

    public override string GetStateText()
    {
        return this.State switch
        {
            // Most pipelines are patch pipelines, so default to terms related to patching
            PipelineState.NotStarted => "Revert Patch",
            PipelineState.Running => "Reverting Patch... (click to cancel)",
            PipelineState.Finished => $"Patch was reverted!",
            PipelineState.Cancelled => "Revert Patch",
            PipelineState.Error => "Retry (Revert failed)",
            _ => "Revert Patch",
        };
    }
}