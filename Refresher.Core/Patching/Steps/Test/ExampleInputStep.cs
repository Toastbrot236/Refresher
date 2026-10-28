using Refresher.Core.Patching.Pipelines;
using Refresher.Core.Platform;

namespace Refresher.Core.Patching.Steps.Test;

public class ExampleInputStep : Step
{
    public ExampleInputStep(Pipeline pipeline) : base(pipeline)
    {
    }

    public override string Name { get; } = "Example input";
    public override float Progress { get; protected set; }

    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        this.Platform.InfoPrompt("Info");
        this.Platform.WarnPrompt("Warn");
        this.Platform.ErrorPrompt("Error");
        QuestionResult result = this.Platform.Ask("Question");
        this.Platform.InfoPrompt(result.ToString());
        
        return Task.CompletedTask;
    }
}