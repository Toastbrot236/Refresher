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
        AutoDiscoverResponse? autoDiscover = AutoDiscoverClient.InvokeAutoDiscoverAsync(this.UserEntered.EnteredServerUrl, this.Platform, cancellationToken).Result;
        if (autoDiscover != null)
        {
            this.Pipeline.AutoDiscover = autoDiscover;
        }
        
        return Task.CompletedTask;
    }
}