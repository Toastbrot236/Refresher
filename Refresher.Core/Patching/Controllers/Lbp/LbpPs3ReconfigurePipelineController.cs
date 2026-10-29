using Refresher.Core.Patching.Pipelines.Common;
using Refresher.Core.Patching.Pipelines.Lbp;

namespace Refresher.Core.Patching.Controllers.Lbp;

public class LbpPs3ReconfigurePipelineController() : PipelineController()
{
    public override string Name => "Reconfigure existing patches for LBP1/2/3 on PS3";
    public override string ShorthandUrlId => "ps3";

    public virtual bool UsesPatchwork => true;
    public abstract TargetPlatform TargetPlatform => TargetPlatform.PS3;

    // This controller doesn't revert Eboots, and also doesn't show game metadata
    public override Type MainPipelineType => typeof(PatchworkPS3ConfigPipeline);
    public override Type VerifyServerInputPipelineType => typeof(AutoDiscoverPipeline);
}