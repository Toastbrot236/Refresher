using Eto;
using Eto.Drawing;
using Eto.Forms;
using Refresher.Core.Patching.Pipelines.Lbp;
using Refresher.Core.Patching.Pipelines.Legacy;
using Refresher.Core.Patching.Steps;
using Refresher.UI.Items;

namespace Refresher.UI;

public abstract class LegacyElfPatchForm : PipelineForm
{
    // TODO replace with PipelineController once we move the hardcoded steps from the base Pipeline class (e.g. AutoDiscover, eboot revert),
    // and once we make PipelineController (and superclasses of it) manage all the pipelines we need per form
    protected override Type PipelineType => typeof(ElfToElfPatchPipeline);

    // these are not in their own classes because they're only used by this form, and nowhere else
    protected LabelledRow InputElfRow { get; set; }
    protected LabelledRow OutputElfRow { get; set; }

    protected FilePicker InputElfPicker { get; set; }
    protected FilePicker OutputElfPicker { get; set; }

    public LegacyElfPatchForm() : this(null)
    {
        
    }
    
    public LegacyElfPatchForm(AutoApplyInformation? autoApply) : base(autoApply)
    {
        // Init and add the GUI components
        this.Content = new Splitter
        {
            Orientation = Orientation.Vertical,
            FixedPanel = SplitterFixedPanel.Panel1,
            Panel1 = this.FormLayout = new TableLayout([
                (this.InputElfRow = new LabelledRow(this.InputElfPicker = new FilePicker()
                    {
                        FilePath = "",
                        FileAction = FileAction.OpenFile,
                    }, "Input .ELF", "Select the ELF file you want to patch")).Row,

                (this.ServerUrlInput = new ServerUrlInput(this.OnAutoDiscoverClick)).Row,

                (this.OutputElfRow = new LabelledRow(this.OutputElfPicker = new FilePicker()
                    {
                        FilePath = "",
                        FileAction = FileAction.SaveFile,
                    }, "Output .ELF", "Select the path where you want to save the patched ELF file")).Row,
            ])
            {
                Spacing = new Size(5, 5),
                Padding = new Padding(0, 0, 0, 10),
            },
            Panel2 = new TableLayout([
                // these are already initted by base class, but do it explicitly here anyway
                (this.LogDisplay = new()).Row,
                (this.ProgressBars = new()).Row,

                new Label(), // line break

                this.PatchButton = new Button(this.OnPatchButtonClick) { Text = "Apply Patch!" },

                new Label(), // line break

                new Button(this.OnViewGuideClick) { Text = "View Guide" },
                new Button(this.OnCopyLogClick) { Text = "Copy Log (for support)" },
            ])
            {
                Padding = new Padding(0, 10, 0, 10),
                Spacing = new Size(5, 5),
                Size = new Size(-1, -1),
            },
        };
    }

    // TODO adjust this when we replace StepInputs with the UserEnteredInputs class,
    // which will store all inputs using their own props, instead of a dictionary.
    protected override void AddFormInputsToPipeline()
    {
        if (this.Pipeline == null) return;

        this.Pipeline.Inputs.Add(CommonStepInputs.ElfInput.Id, this.InputElfPicker.FilePath);
        this.Pipeline.Inputs.Add(CommonStepInputs.ElfOutput.Id, this.OutputElfPicker.FilePath);
        this.Pipeline.Inputs.Add(CommonStepInputs.ServerUrl.Id, this.ServerUrlInput!.Input.Text);
    }
}