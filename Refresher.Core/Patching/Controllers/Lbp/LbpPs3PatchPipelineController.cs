using Refresher.Core.Patching.Pipelines.Common;
using Refresher.Core.Patching.Pipelines.Lbp;

namespace Refresher.Core.Patching.Controllers.Lbp;

public class LbpPs3PatchPipelineController() : PipelineController()
{
    public override string Name => "Patch unpatched LBP1/2/3 on PS3";
    public override string ShorthandUrlId => "ps3";

    public override bool UsesPatchwork => true;
    public override TargetPlatform TargetPlatform => TargetPlatform.PS3;

    public override Type MainPipelineType => typeof(LbpPS3PatchPipeline);
    public override Type RevertPatchPipelineType => typeof(RevertEbootPatchPipeline);
    public override Type AutoDiscoverPipelineType => typeof(AutoDiscoverPipeline);
    public override Type DownloadGameDataPipelineType => typeof(DownloadGameListPipeline);
}