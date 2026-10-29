using Refresher.Core.Patching.Pipelines.Common;
using Refresher.Core.Patching.Pipelines.Legacy;

namespace Refresher.Core.Patching.Controllers.Legacy;

public class LegacyRpcs3PipelineController() : PipelineController()
{
    public override string Name => "Patch non-LBP game on RPCS3 (Legacy)";
    public override string ShorthandUrlId => "";

    public override bool UsesPatchwork => false;
    public override TargetPlatform TargetPlatform => TargetPlatform.RPCS3;

    // This one doesn't have any revert pipeline
    public override Type MainPipelineType => typeof(RPCS3PatchPipeline);
    public override Type VerifyServerInputPipelineType => typeof(AutoDiscoverPipeline);
    public override Type DownloadGameDataPipelineType => typeof(DownloadGameListPipeline);
}