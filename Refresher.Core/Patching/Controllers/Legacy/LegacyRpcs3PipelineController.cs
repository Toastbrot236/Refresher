
using Refresher.Core.Patching.Pipelines.Common;
using Refresher.Core.Patching.Pipelines.Lbp;
using Refresher.Core.Patching.Pipelines.Legacy;
using Refresher.Core.Platform;

namespace Refresher.Core.Patching.Controllers.Legacy;

public class LegacyRpcs3PipelineController(IPlatformInterface platform) : PipelineController(platform)
{
    public override string Name => "Patch non-LBP game on RPCS3 (Legacy)";
    public override string ShorthandUrlId => "";

    // This one doesn't have any revert pipeline
    public override Type MainPipelineType => typeof(RPCS3PatchPipeline);
    public override Type VerifyServerInputPipelineType => typeof(AutoDiscoverPipeline);
    public override Type DownloadGameDataPipelineType => typeof(DownloadGameListPipeline);
}