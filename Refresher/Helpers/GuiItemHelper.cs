using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace Refresher.Helpers;

public static class GuiItemHelper
{
    /// <summary>
    /// Puts a label to the left, the child to the middle, and optionally a button to the right of the row.
    /// </summary>
    /// <param name="onButtonClick">
    /// If not null, we will add a button to the right of the TableRow which will execute this callback and contain the text in 'buttonText'.
    /// </param>
    /// <param name="buttonText">
    /// If this isn't set, but 'onButtonClick' is, we will add a button regardless, but with default text.
    /// </param>
    public static TableRow AsLabelledChild(Control child, string labelText, string tooltipText, Button? button = null)
    {
        Label label = new()
        {
            // line break above label
            Text = '\n' + labelText + ':',
            VerticalAlignment = VerticalAlignment.Center,
            ToolTip = tooltipText,
        };

        if (button == null)
        {
            return new(new TableLayout([
                label,
                child
            ]));
        }

        return new(new TableLayout([
            label,
            new DynamicLayout([child, button])
            {
                Spacing = new Size(5, 0)
            }
        ]));
    }

    /// <summary>
    /// Puts a label to the left, a TextBox (1 row text input) to the middle, and optionally a button to the right of the row.
    /// </summary>
    public static TableRow AsLabelledTextInput(string labelText, string tooltipText, string defaultText, string placeholder, Button? button = null)
    {
        TextBox textBox = new()
        {
            Text = defaultText,
            PlaceholderText = placeholder
        };
        return AsLabelledChild(textBox, labelText, tooltipText, button);
    }

    /// <summary>
    /// Puts a label to the left, a file (or folder) picker to the middle, and optionally sets a callback for when a file is selected.
    /// </summary>
    public static TableRow AsLabelledFilePicker(string labelText, string tooltipText, FileAction fileAction, string defaultPath, EventHandler<EventArgs>? onFileSelected = null)
    {
        FilePicker filePicker = new()
        {
            FilePath = defaultPath,
            FileAction = fileAction
        };
        if (onFileSelected != null)
        {
            filePicker.FilePathChanged += onFileSelected;
        }
        return AsLabelledChild(filePicker, labelText, tooltipText);
    }
}