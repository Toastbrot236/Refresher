using Eto.Forms;

namespace Refresher.UI.Items;

public class PatchProgressBars
{
    public TableRow Row { get; set; }

    public LabelledRow PipelineProgressRow { get; set; }
    public LabelledRow StepProgressRow { get; set; }

    public ProgressBar PipelineProgressBar { get; set; }
    public ProgressBar StepProgressBar { get; set; }

    public PatchProgressBars()
    {
        // TODO use current pipeline name and step name in progress bar labels, and the pipeline state
        // as their tooltip.
        this.PipelineProgressRow = new(this.PipelineProgressBar = new(), "Pipeline progress", "");
        this.StepProgressRow = new(this.StepProgressBar = new(), "Step progress", "");

        this.Row = new(new TableLayout([
            this.PipelineProgressRow,
            this.StepProgressRow,
        ]));
    }
}