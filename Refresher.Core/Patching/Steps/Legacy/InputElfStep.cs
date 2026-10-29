using Refresher.Core.Patching.Pipelines;

namespace Refresher.Core.Patching.Steps.Legacy;

public class InputElfStep : Step
{
    public InputElfStep(Pipeline pipeline) : base(pipeline)
    {}

    public override string Name { get; } = "Opening an EBOOT.ELF";
    public override float Progress { get; protected set; }
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        string? elfInput = this.UserInputs?.InputElfPath;
        if (string.IsNullOrWhiteSpace(elfInput))
        {
            return this.Fail("Input path of the EBOOT.ELF to patch is missing! Please report this issue! Cancelling operation.");
        }

        if (!File.Exists(elfInput))
            return this.Fail("The Input .ELF could not be found.");

        string temp = Path.GetTempFileName();

        {
            using FileStream write = File.OpenWrite(temp);
            using FileStream read = File.OpenRead(elfInput);
            read.CopyTo(write);
        }

        this.Pipeline.GameInformation = new GameInformation
        {
            DecryptedEbootPath = temp,
            Name = Path.GetFileName(elfInput),
            TitleId = "UNKN12345",
        };

        return Task.CompletedTask;
    }
}