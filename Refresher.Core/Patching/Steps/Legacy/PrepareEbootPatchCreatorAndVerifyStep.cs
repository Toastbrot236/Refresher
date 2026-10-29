using Refresher.Core.Patching;
using Refresher.Core.Patching.Pipelines;
using Refresher.Core.Verification;

namespace Refresher.Core.Patching.Steps.Legacy;

public class PrepareEbootPatchCreatorAndVerifyStep : Step
{
    public PrepareEbootPatchCreatorAndVerifyStep(Pipeline pipeline) : base(pipeline)
    {}

    public override string Name { get; } = "Preparing RPCS3 patch creator";
    public override float Progress { get; protected set; }
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        string? url = this.Pipeline.Context?.GetServerUrl();
        string? devHdd0Path = this.UserInputs?.OutputElfPath;
        if (string.IsNullOrWhiteSpace(devHdd0Path))
        {
            return this.Fail("RPCS3 dev_hdd0 folder path was not specified! Cancelling operation.");
        }
        if (string.IsNullOrWhiteSpace(url))
        {
            return this.Fail("Server URL was not specified! Cancelling operation.");
        }

        EbootPatcher patcher = new(File.Open(this.Game.DecryptedEbootPath!, FileMode.Open, FileAccess.ReadWrite));
        patcher.GenerateRpcs3Patch = true;

        patcher.Rpcs3PatchFolder = Path.GetFullPath(Path.Combine(devHdd0Path, "..", "patches")); // lol
        patcher.TitleId = this.Game.TitleId;
        patcher.GameName = this.Game.Name;
        patcher.GameVersion = this.Game.Version;
        
        RefresherState.Logger.LogDebug(LogType.RPCS3, $"RPCS3 patches folder: {patcher.Rpcs3PatchFolder}");

        this.Pipeline.Patcher = patcher;

        List<Message> messages = patcher.Verify(url, this.AutoDiscover?.UsesCustomDigestKey ?? false);
        foreach (Message message in messages)
        {
            RefresherState.Logger.LogInfo(LogType.Patcher, message.ToString());
        }

        if (messages.Any(m => m.Level == MessageLevel.Error))
        {
            return this.Fail("There were errors while verifying the patch details against the EBOOT. Check the log for more information.");
        }
        
        return Task.CompletedTask;
    }
}