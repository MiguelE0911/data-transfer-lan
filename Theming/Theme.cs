using System.Drawing.Drawing2D;

public static class Theme
{
    public static readonly Color Bg = Color.FromArgb(0x0E, 0x14, 0x20);
    public static readonly Color Surface = Color.FromArgb(0x1A, 0x22, 0x33);
    public static readonly Color SurfaceAlt = Color.FromArgb(0x14, 0x1B, 0x29);
    public static readonly Color Border = Color.FromArgb(0x2A, 0x35, 0x48);
    public static readonly Color Text = Color.FromArgb(0xE6, 0xEC, 0xF5);
    public static readonly Color Muted = Color.FromArgb(0x93, 0xA3, 0xBD);
    public static readonly Color RowAlt = Color.FromArgb(0x14, 0x1B, 0x29);
    public static readonly Color InputBg = Color.FromArgb(0x11, 0x18, 0x28);

    public static readonly Color Accent = Color.FromArgb(0x4C, 0x8D, 0xF6);
    public static readonly Color AccentDark = Color.FromArgb(0x3B, 0x6F, 0xD6);
    public static readonly Color Success = Color.FromArgb(0x22, 0xB2, 0x5F);
    public static readonly Color SuccessDark = Color.FromArgb(0x1C, 0x91, 0x50);
    public static readonly Color Danger = Color.FromArgb(0xE5, 0x48, 0x4D);
    public static readonly Color DangerDark = Color.FromArgb(0xC4, 0x3B, 0x40);

    public static readonly Color Secondary = Color.FromArgb(0x23, 0x2E, 0x44);
    public static readonly Color SecondaryHover = Color.FromArgb(0x2C, 0x39, 0x52);
    public static readonly Color SecondaryPress = Color.FromArgb(0x35, 0x42, 0x5E);

    public static readonly Color Header = Color.FromArgb(0x0A, 0x0F, 0x1A);
    public static readonly Color HeaderAccent = Color.FromArgb(0x4C, 0xC2, 0xF7);
    public static readonly Color HeaderText = Color.FromArgb(0xEA, 0xF1, 0xFB);
    public static readonly Color HeaderMuted = Color.FromArgb(0x8F, 0xA1, 0xBC);

    public static readonly Color LogBg = Color.FromArgb(0x0A, 0x0F, 0x18);
    public static readonly Color LogText = Color.FromArgb(0xCF, 0xD8, 0xE6);
    public static readonly Color Selection = Color.FromArgb(0x22, 0x34, 0x57);
    public static readonly Color Grid = Color.FromArgb(0x28, 0x34, 0x4C);

    public static readonly Font BaseFont = new("Segoe UI", 9.5F);
    public static readonly Font SmallFont = new("Segoe UI", 8.5F);
    public static readonly Font BoldFont = new("Segoe UI Semibold", 9.5F);
    public static readonly Font CardTitleFont = new("Segoe UI Semibold", 10.5F);
    public static readonly Font ListHeaderFont = new("Segoe UI Semibold", 9F);
    public static readonly Font AppTitleFont = new("Segoe UI Semibold", 15F);
    public static readonly Font MonoFont = new("Consolas", 9.5F);
    public static readonly Font GaugeValueFont = new("Segoe UI Semibold", 20F);

    public static Color Mix(Color a, Color b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return Color.FromArgb(
            (int)(a.A + (b.A - a.A) * t),
            (int)(a.R + (b.R - a.R) * t),
            (int)(a.G + (b.G - a.G) * t),
            (int)(a.B + (b.B - a.B) * t));
    }

    public static GraphicsPath Rounded(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        int d = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
        if (d <= 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        int x = bounds.X, y = bounds.Y, w = bounds.Width, h = bounds.Height;
        path.AddArc(x, y, d, d, 180, 90);
        path.AddArc(x + w - d, y, d, d, 270, 90);
        path.AddArc(x + w - d, y + h - d, d, d, 0, 90);
        path.AddArc(x, y + h - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    public static void StyleInput(Control control)
    {
        control.BackColor = InputBg;
        control.ForeColor = Text;
        if (control is TextBoxBase textBox)
            textBox.BorderStyle = BorderStyle.FixedSingle;
        if (control is NumericUpDown numeric)
        {
            numeric.BorderStyle = BorderStyle.FixedSingle;
            numeric.BackColor = InputBg;
        }
    }
}
