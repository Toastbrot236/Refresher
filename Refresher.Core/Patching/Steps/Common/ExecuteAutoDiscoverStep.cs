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
        string? enteredUrl = this.UserEntered.EnteredServerUrl;
        if (string.IsNullOrWhiteSpace(enteredUrl))
            return this.Fail("Can't use AutoDiscover if no server URL was given!");

        AutoDiscoverResponse? autoDiscover = AutoDiscoverClient.InvokeAutoDiscoverAsync(enteredUrl, this.Platform, cancellationToken).Result;
        if (autoDiscover != null)
        {
            this.Pipeline.AutoDiscover = autoDiscover;
        }
        
        return Task.CompletedTask;
    }
}