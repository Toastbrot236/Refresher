using Eto.Drawing;
using Eto.Forms;

namespace Refresher.UI.Items;

public class LabelledRow : TableRow, ITableRowWrapper
{
    public TableRow Row { get; set; }
    public Label Label { get; set; }
    public Control Content { get; set; }
    public Button? SideButton { get; set; }

    public LabelledRow(Control content, string labelText, string tooltipText, Button? sideButton = null)
    {
        this.Label = new()
        {
            // line break above label
            Text = '\n' + labelText + ':',
            VerticalAlignment = VerticalAlignment.Center,
            ToolTip = tooltipText,
        };

        this.Content = content;
        this.SideButton = sideButton;

        this.Row = new(new TableLayout([
            this.Label,

            // Just content if no button, else button to the right of content
            sideButton == null
            ? content
            : new DynamicLayout([content, sideButton])
                {
                    Spacing = new Size(5, 0)
                }
        ]));
    }
}