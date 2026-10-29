using Refresher.Core.Patching.Pipelines.Common;
using Refresher.Core.Patching.Pipelines.Lbp;

namespace Refresher.Core.Patching.Controllers.Lbp;

public class LbpRpcs3PatchPipelineController() : PipelineController()
{
    public override string Name => "Patch unpatched LBP1/2/3 on RPCS3";
    public override string ShorthandUrlId => "rpcs3";

    public override bool UsesPatchwork => true;
    public override TargetPlatform TargetPlatform => TargetPlatform.RPCS3;

    public override Type MainPipelineType => typeof(LbpRPCS3PatchPipeline);
    public override Type RevertPatchPipelineType => typeof(RevertEbootPatchPipeline);
    public override Type VerifyServerInputPipelineType => typeof(AutoDiscoverPipeline);
    public override Type DownloadGameDataPipelineType => typeof(DownloadGameListPipeline);
}