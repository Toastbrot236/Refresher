using Refresher.Core.Pipelines.Steps;
using Refresher.Core.Pipelines.Steps.PS3;

namespace Refresher.Core.Pipelines.Lbp;

public class PatchworkPS3ConfigPipeline : PatchworkConfigPipeline
{
    protected override Type SetupAccessorStepType => typeof(SetupPS3AccessorStep);
    protected override string ConsoleName => "PS3";
}