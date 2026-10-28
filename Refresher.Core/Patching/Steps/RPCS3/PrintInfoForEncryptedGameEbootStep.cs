using Refresher.Core.Patching.Pipelines;
using SCEToolSharp;

namespace Refresher.Core.Patching.Steps.RPCS3;

public class PrintInfoForEncryptedGameEbootStep : Step
{
    public PrintInfoForEncryptedGameEbootStep(Pipeline pipeline) : base(pipeline)
    {}

    public override string Name { get; } = "Printing info on encrypted EBOOT";
    public override float Progress { get; protected set; }
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        LibSceToolSharp.PrintInfos(this.Game.EncryptedEbootPath ?? "Encrypted EBOOT path is null, it likely doesn't exist.");
        return Task.CompletedTask;
    }
}