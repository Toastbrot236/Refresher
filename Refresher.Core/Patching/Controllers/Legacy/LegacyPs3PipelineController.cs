using Refresher.Core.Patching.Pipelines.Common;
using Refresher.Core.Patching.Pipelines.Legacy;

namespace Refresher.Core.Patching.Controllers.Legacy;

public class LegacyPs3PipelineController() : PipelineController()
{
    public override string Name => "Patch non-LBP game on PS3 (Legacy)";
    public override string ShorthandUrlId => "";

    public override Type MainPipelineType => typeof(PS3PatchPipeline);
    public override Type RevertPatchPipelineType => typeof(RevertEbootPatchPipeline);
    public override Type VerifyServerInputPipelineType => typeof(AutoDiscoverPipeline);
    public override Type DownloadGameDataPipelineType => typeof(DownloadGameListPipeline);
}