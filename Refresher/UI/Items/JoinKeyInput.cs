using Eto.Forms;

namespace Refresher.UI.Items;

public class JoinKeyInput
{
    public LabelledRow Row { get; set; }
    public TextBox Input { get; set; }

    public JoinKeyInput(EventHandler<EventArgs> onCopyUrlClick)
    {
        this.Input = new TextBox
        {
            Text = "",
            PlaceholderText = "(leave empty to randomize)"
        };
        
        this.Row = new(this.Input, "Join Key", "Enter the Join Key you want to use",
            new Button(onCopyUrlClick) { Text = "Copy as URL" });
    }
}