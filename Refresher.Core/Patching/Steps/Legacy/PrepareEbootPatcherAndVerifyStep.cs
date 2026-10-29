using Refresher.Core.Patching;
using Refresher.Core.Patching.Pipelines;
using Refresher.Core.Verification;

namespace Refresher.Core.Patching.Steps.Legacy;

public class PrepareEbootPatcherAndVerifyStep : Step
{
    public PrepareEbootPatcherAndVerifyStep(Pipeline pipeline) : base(pipeline)
    {}

    public override string Name { get; } = "Preparing PS3 EBOOT patcher";
    public override float Progress { get; protected set; }
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        string? url = this.Pipeline.Context?.GetServerUrl();
        if (string.IsNullOrWhiteSpace(url))
        {
            return this.Fail("Server URL was not specified! Cancelling operation.");
        }

        EbootPatcher patcher = new(File.Open(this.Game.DecryptedEbootPath!, FileMode.Open, FileAccess.ReadWrite));

        this.Pipeline.Patcher = patcher;

        // TODO maybe it'd be a good idea to split the url/digest key verification into a separate step, and to also use that step in the
        // server input verification pipelines that correspond to the pipelines which already use this step.
        List<Message> messages = patcher.Verify(url, true);
        foreach (Message message in messages)
        {
            State.Logger.LogInfo(LogType.Patcher, message.ToString());
        }

        if (messages.Any(m => m.Level == MessageLevel.Error))
        {
            return this.Fail("There were errors while verifying the patch details against the EBOOT. Check the log for more information.");
        }
        
        return Task.CompletedTask;
    }
}