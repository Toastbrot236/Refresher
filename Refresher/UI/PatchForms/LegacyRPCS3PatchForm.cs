using Eto.Drawing;
using Eto.Forms;
using Refresher.Core.Patching.Pipelines.Legacy;
using Refresher.Core.Patching.Steps;
using Refresher.UI.Items;

namespace Refresher.UI.PatchForms;

public class LegacyRPCS3PatchForm : PipelineForm
{
    // TODO replace with PipelineController once we move the hardcoded steps from the base Pipeline class (e.g. AutoDiscover, eboot revert),
    // and once we make PipelineController (and superclasses of it) manage all the pipelines we need per form
    protected override Type PipelineType => typeof(RPCS3PatchPipeline);
    protected Rpcs3ConnectionInput ConnectionInput { get; set; }

    public LegacyRPCS3PatchForm() : this(null)
    {
        
    }
    
    public LegacyRPCS3PatchForm(AutoApplyInformation? autoApply) : base(autoApply)
    {
        // Init and add the GUI components
        this.Content = new Splitter
        {
            Orientation = Orientation.Vertical,
            FixedPanel = SplitterFixedPanel.Panel1,
            Panel1 = this.FormLayout = new TableLayout([
                (this.ConnectionInput = new Rpcs3ConnectionInput(this.OnConnect)).Row,
                (this.GamesDropDown = new GameListDropdown()).Row,
                (this.ServerUrlInput = new ServerUrlInput(this.OnAutoDiscoverClick)).Row,
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
                this.RevertPatchButton = new Button(this.OnRevertPatchClick) { Text = "Revert Patch" },

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

        this.Pipeline.Inputs.Add(CommonStepInputs.TitleId.Id, this.GamesDropDown!.Dropdown.SelectedKey);
        this.Pipeline.Inputs.Add(CommonStepInputs.RPCS3Folder.Id, this.ConnectionInput.DevHdd0Picker.FilePath);
        this.Pipeline.Inputs.Add(CommonStepInputs.ServerUrl.Id, this.ServerUrlInput!.Input.Text);
    }
}