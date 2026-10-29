using Refresher.Core.Patching.Steps.Common;
using Refresher.Core.Patching.Steps.RPCS3;

namespace Refresher.Core.Patching.Pipelines.Lbp;

// TODO this pipeline will be removed when we rework LBP RPCS3 patching.
public class LbpRPCS3PatchPipeline : Pipeline
{
    public override string Name => "LBP RPCS3 Patch";
    public override bool NeedsAccessor => true;

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
        typeof(ApplySprxPatchToEbootStep),
        
        // Encryption and upload stage
        typeof(FakeEncryptGameEbootStep),
        typeof(PrintInfoForEncryptedGameEbootStep),
        typeof(BackupGameEbootBeforeReplaceStep),
        typeof(UploadPatchworkSprxStep),
        typeof(UploadPatchworkConfigurationStep),
        typeof(UploadGameEbootStep),
    ];
}