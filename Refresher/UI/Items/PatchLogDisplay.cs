using Eto.Forms;

namespace Refresher.UI.Items;

public class PatchLogDisplay : ITableRowWrapper
{
    public TableRow Row => this.LabelRow.Row;
    public LabelledRow LabelRow { get; set; }
    public ListBox Display { get; set; }

    public PatchLogDisplay()
    {
        this.Display = new ListBox
        {
            Height = -1,
        };

        /*
        new StackLayoutItem(this.LogMessageDisplay = new ListBox
                {
                    Height = -1,
                }, VerticalAlignment.Top, true),
        */
        
        this.LabelRow = new(this.Display, "Patch log", "Displays various messages related to patch progress");
    }
}