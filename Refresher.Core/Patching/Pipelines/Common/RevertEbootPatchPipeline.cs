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
}