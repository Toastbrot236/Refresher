using Refresher.Core.Accessors;
using Refresher.Core.Patching;
using Refresher.Core.Patching.Pipelines;
using Refresher.Core.Verification;
using Refresher.Core.Verification.AutoDiscover;

namespace Refresher.Core.Patching.Steps.Common;

public class ExecuteAutoDiscoverStep : Step
{
    public ExecuteAutoDiscoverStep(Pipeline pipeline) : base(pipeline)
    {}

    public override string Name { get; } = "Downloading server data via AutoDiscover";
    public override float Progress { get; protected set; }
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (this.Pipeline.UserContext == null)
        {
            throw new InvalidDataException("Cannot execute AutoDiscover if we don't know what server URL the user has entered!");
        }

        AutoDiscoverResponse? autoDiscover = AutoDiscoverClient.InvokeAutoDiscoverAsync(this.Pipeline.UserContext.EnteredServerUrl, this.Platform, cancellationToken).Result;
        if (autoDiscover != null)
        {
            this.Pipeline.AutoDiscover = autoDiscover;
        }
        
        return Task.CompletedTask;
    }
}