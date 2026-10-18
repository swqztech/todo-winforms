namespace MojiTodo.Ui;

/// <summary>与 Rust 版 moji-qingdan 一致的浅色主题</summary>
internal static class MojiTheme
{
    public static readonly Color PageBg = Color.FromArgb(232, 238, 246);
    public static readonly Color TopBar = Color.FromArgb(245, 248, 252);
    public static readonly Color Accent = Color.FromArgb(72, 120, 200);
    public static readonly Color Title = Color.FromArgb(30, 40, 52);
    public static readonly Color Sub = Color.FromArgb(90, 102, 118);
    public static readonly Color Muted = Color.FromArgb(120, 130, 145);
    public static readonly Color Border = Color.FromArgb(210, 218, 230);
    public static readonly Color BtnBlue = Color.FromArgb(72, 130, 220);
    public static readonly Color ChipOn = Color.FromArgb(40, 48, 58);
    public static readonly Color ChipOff = Color.FromArgb(248, 250, 253);

    public static void StyleInput(TextBox tb)
    {
        tb.BackColor = Color.White;
        tb.ForeColor = Title;
        tb.BorderStyle = BorderStyle.FixedSingle;
    }

    public static void StyleChip(Button btn, bool active)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.BorderColor = active ? Accent : Border;
        btn.BackColor = active ? ChipOn : ChipOff;
        btn.ForeColor = active ? Color.White : Title;
        btn.Font = new Font(btn.Font, active ? FontStyle.Bold : FontStyle.Regular);
        btn.Cursor = Cursors.Hand;
    }

    public static Button MakeBlueButton(string text, Size size)
    {
        return new Button
        {
            Text = text,
            Size = size,
            FlatStyle = FlatStyle.Flat,
            BackColor = BtnBlue,
            ForeColor = Color.White,
            FlatAppearance = { BorderSize = 0 },
            Font = new Font("Microsoft YaHei UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
    }

    public static Button MakeChipButton(string text, Size size)
    {
        var b = new Button
        {
            Text = text,
            Size = size,
            Margin = new Padding(0, 0, 6, 0)
        };
        StyleChip(b, false);
        return b;
    }
}
