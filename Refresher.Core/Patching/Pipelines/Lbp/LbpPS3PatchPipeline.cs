using Refresher.Core.Patching.Steps.Common;
using Refresher.Core.Patching.Steps.PS3;

namespace Refresher.Core.Patching.Pipelines.Lbp;

public class LbpPS3PatchPipeline : Pipeline
{
    public override string Name => "LBP PS3 Patch";

    protected override Type SetupAccessorStepType => typeof(SetupPS3AccessorStep);

    protected override List<Type> StepTypes =>
    [
        // Info gathering stage
        typeof(ValidateGameStep),
        typeof(DownloadParamSfoStep),
        typeof(DownloadGameEbootStep),
        typeof(ReadEbootContentIdStep),
        typeof(DownloadGameLicenseStep),
        typeof(GetConsoleIdpsStep),
        
        // Decryption and patch stage
        typeof(PrepareSceToolStep),
        typeof(DecryptGameEbootStep),
        typeof(ApplySprxPatchToEbootStep),
        
        // Encryption and upload stage
        typeof(EncryptGameEbootStep),
        typeof(BackupGameEbootBeforeReplaceStep),
        typeof(UploadPatchworkSprxStep),
        typeof(UploadPatchworkConfigurationStep),
        typeof(UploadGameEbootStep),
    ];
}