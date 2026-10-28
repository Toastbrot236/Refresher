using Refresher.Core.Patching.Steps.Common;

namespace Refresher.Core.Patching.Pipelines.Common;

public class AutoDiscoverPipeline : Pipeline
{
    public override string Name => "AutoDiscover";

    protected override List<Type> StepTypes =>
    [
        typeof(ExecuteAutoDiscoverStep),
    ];
}