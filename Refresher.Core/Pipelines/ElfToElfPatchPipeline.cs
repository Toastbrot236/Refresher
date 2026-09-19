using Refresher.Core.Pipelines.Steps.Legacy;

namespace Refresher.Core.Pipelines;

public class ElfToElfPatchPipeline : Pipeline
{
    public override string Name => ".elf->.elf Patch";
    
    protected override List<Type> StepTypes =>
    [
        typeof(InputElfStep),

        typeof(PrepareEbootPatcherAndVerifyStep),
        typeof(ApplyPatchToEbootStep),
        
        typeof(OutputElfStep),
    ];
}