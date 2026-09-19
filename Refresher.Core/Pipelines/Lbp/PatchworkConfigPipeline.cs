using Refresher.Core.Pipelines.Steps;
using Refresher.Core.Pipelines.Steps.Common;

namespace Refresher.Core.Pipelines.Lbp;

public abstract class PatchworkConfigPipeline : Pipeline
{
    public override string Name => $"Patchwork {this.ConsoleName} Config";

    public override string? ShorthandUrlId => this.ConsoleName.ToLower();

    protected abstract string ConsoleName { get; }

    protected abstract override Type? SetupAccessorStepType { get; }

    protected override List<Type> StepTypes =>
    [
        typeof(UploadPatchworkSprxStep),
        typeof(UploadPatchworkConfigurationStep),
    ];
}