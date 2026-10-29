namespace HotelMS.GUI;

// Các hàm tiện ích dựng giao diện WinForms bằng code
public static class Ui
{
    public static Label AddLabel(Control p, string text, int x, int y)
    {
        var c = new Label { Text = text, Left = x, Top = y + 3, AutoSize = true };
        p.Controls.Add(c); return c;
    }
    public static TextBox AddText(Control p, int x, int y, int w = 150)
    {
        var c = new TextBox { Left = x, Top = y, Width = w };
        p.Controls.Add(c); return c;
    }
    public static ComboBox AddCombo(Control p, int x, int y, int w = 150)
    {
        var c = new ComboBox { Left = x, Top = y, Width = w, DropDownStyle = ComboBoxStyle.DropDownList };
        p.Controls.Add(c); return c;
    }
    public static NumericUpDown AddNum(Control p, int x, int y, int w, decimal value, decimal min, decimal max, int decimals)
    {
        var c = new NumericUpDown { Left = x, Top = y, Width = w, DecimalPlaces = decimals, Minimum = min, Maximum = max, Value = value };
        p.Controls.Add(c); return c;
    }
    public static Button AddButton(Control p, string text, int x, int y, int w, EventHandler onClick)
    {
        var c = new Button { Text = text, Left = x, Top = y, Width = w, Height = 30 };
        c.Click += onClick;
        p.Controls.Add(c); return c;
    }
    public static DataGridView AddGrid(Control p, int x, int y, int w, int h)
    {
        var g = new DataGridView
        {
            Left = x, Top = y, Width = w, Height = h,
            ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false,
            BackgroundColor = Color.White
        };
        p.Controls.Add(g); return g;
    }
}
