using Refresher.Core.Patching.Steps.RPCS3;

namespace Refresher.Core.Patching.Pipelines.Lbp;

// TODO this pipeline will be removed when we rework LBP RPCS3 patching.
public class PatchworkRPCS3ConfigPipeline : PatchworkConfigPipeline
{
    public override string Name => $"Reconfigure Patchwork on RPCS3";
    protected override Type? SetupAccessorStepType => typeof(SetupEmulatorAccessorStep);
}