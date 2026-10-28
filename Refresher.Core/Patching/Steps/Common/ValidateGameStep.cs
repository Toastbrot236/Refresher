using Refresher.Core.Patching;
using Refresher.Core.Patching.Pipelines;

namespace Refresher.Core.Patching.Steps.Common;

public class ValidateGameStep : Step
{
    public ValidateGameStep(Pipeline pipeline) : base(pipeline)
    {}

    public override string Name { get; } = "Validating your entered game data";
    public override float Progress { get; protected set; }

    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        string? titleId = this.UserInputs.TitleID?.Trim();
        if (titleId == null)
            return this.Fail("No game was selected!");

        string gamePath = $"game/{titleId}";

        // sanity check. UI will not allow this to happen, but CLI will
        if (titleId.Length != "NPUA80662".Length)
            this.Fail("Title ID does not match expected length. Did you type the ID in correctly?");

        if (!this.Pipeline.Accessor!.DirectoryExists(gamePath))
            this.Fail("The game directory does not exist. This usually means you haven't installed any updates for your game.");
        
        this.Pipeline.GameInformation = new GameInformation
        {
            TitleId = titleId,
        };

        return Task.CompletedTask;
    }
}