using Eto.Forms;

namespace Refresher.UI.Items;

public class ServerUrlInput
{
    public TableRow OuterRow { get; set; }
    public LabelledRow InputRow { get; set; }
    public TextBox Input { get; set; }
    public Button? AutoDiscoverButton { get; set; }

    public ServerUrlInput(EventHandler<EventArgs> onAutoDiscoverClick)
    {
        this.InputRow = new(this.Input = new TextBox
        {
            Text = "",
            PlaceholderText = "http://example.com"
        }, "Server URL", "Enter the URL of the server you want to patch to");

        this.AutoDiscoverButton = new Button(onAutoDiscoverClick)
        {
            Text = "AutoDiscover" // should be updated by parent form anyway
        };
        
        this.OuterRow = new(new TableLayout([
            this.InputRow,
            this.AutoDiscoverButton,
        ]));
    }
}