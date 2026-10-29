using Refresher.Core.Patching.Steps.Common;
using Refresher.Core.Patching.Steps.Legacy;
using Refresher.Core.Patching.Steps.RPCS3;

namespace Refresher.Core.Patching.Pipelines.Legacy;

public class RPCS3PatchPipeline : Pipeline
{
    public override string Name => "Create RPCS3 patch for non-LBP game";

    protected override Type SetupAccessorStepType => typeof(SetupEmulatorAccessorStep);

    protected override List<Type> StepTypes =>
    [
        // Info gathering stage
        typeof(ValidateGameStep),
        typeof(DownloadParamSfoStep),
        typeof(DownloadGameEbootStep),
        typeof(ReadEbootContentIdStep),
        typeof(DownloadGameLicenseStep),
        
        // Decryption and patch stage
        typeof(PrepareSceToolStep),
        typeof(DecryptGameEbootStep),
        typeof(PrepareEbootPatchCreatorAndVerifyStep),
        typeof(ApplyPatchToEbootStep),
        // The patch creator will automatically write to the patch file. No upload steps are required.
    ];
}