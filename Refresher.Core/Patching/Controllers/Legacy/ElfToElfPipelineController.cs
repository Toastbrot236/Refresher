
using Refresher.Core.Patching.Pipelines.Common;
using Refresher.Core.Patching.Pipelines.Lbp;
using Refresher.Core.Patching.Pipelines.Legacy;
using Refresher.Core.Platform;

namespace Refresher.Core.Patching.Controllers.Legacy;

public class ElfToElfPipelineController(IPlatformInterface platform) : PipelineController(platform)
{
    public override string Name => ".elf -> .elf Patch (Advanced)";
    public override string ShorthandUrlId => "";

    public override Type MainPipelineType => typeof(ElfToElfPatchPipeline);
    public override Type RevertPatchPipelineType => typeof(RevertEbootPatchPipeline);
    public override Type VerifyServerInputPipelineType => typeof(AutoDiscoverPipeline);
    public override Type DownloadGameDataPipelineType => typeof(DownloadGameListPipeline);
}