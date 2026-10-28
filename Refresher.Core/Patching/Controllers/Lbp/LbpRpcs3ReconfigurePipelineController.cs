using Refresher.Core.Patching.Pipelines.Common;
using Refresher.Core.Patching.Pipelines.Lbp;

namespace Refresher.Core.Patching.Controllers.Lbp;

public class LbpRpcs3ReconfigurePipelineController() : PipelineController()
{
    public override string Name => "Reconfigure existing patches for LBP1/2/3 on RPCS3";
    public override string ShorthandUrlId => "rpcs3";

    // This controller doesn't revert Eboots, and also doesn't show game metadata
    public override Type MainPipelineType => typeof(PatchworkRPCS3ConfigPipeline);
    public override Type VerifyServerInputPipelineType => typeof(AutoDiscoverPipeline);
}