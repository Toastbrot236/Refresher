using Refresher.Core.Patching.Steps.Common;

namespace Refresher.Core.Patching.Pipelines.Common;

public class DownloadGameListPipeline : Pipeline
{
    public override string Name => "Downloading your Game list";

    public override bool ReplacesEboot => true;

    protected override List<Type> StepTypes =>
    [
        typeof(DownloadGameListStep),
    ];

    protected override Type? SetupAccessorStepType { get; }

    public DownloadGameListPipeline(Type setupAccessorStepType)
    {
        this.SetupAccessorStepType = setupAccessorStepType;
    }
}