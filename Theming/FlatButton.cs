using System.Drawing.Drawing2D;

public class FlatButton : Button
{
    public Color BaseColor { get; set; } = Theme.Accent;
    public Color HoverColor { get; set; } = Theme.AccentDark;
    public Color PressColor { get; set; } = Theme.AccentDark;
    public Color BorderColor { get; set; } = Color.Transparent;
    public int CornerRadius { get; set; } = 8;
    public string Glyph { get; set; } = "";
    public int GlyphSize { get; set; } = 16;

    private bool _hover;
    private bool _pressed;

    public FlatButton()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Font = Theme.BaseFont;
        ForeColor = Color.White;
        Cursor = Cursors.Hand;
        AutoSize = false;
        Height = 34;
    }

    public static FlatButton Primary(string text, string glyph, int width) => new()
    {
        Text = text, Glyph = glyph, Width = width,
        BaseColor = Theme.Accent, HoverColor = Theme.AccentDark, PressColor = Theme.AccentDark
    };

    public static FlatButton Success(string text, string glyph, int width) => new()
    {
        Text = text, Glyph = glyph, Width = width, Font = Theme.BoldFont, Height = 40,
        BaseColor = Theme.Success, HoverColor = Theme.SuccessDark, PressColor = Theme.SuccessDark
    };

    public static FlatButton Danger(string text, string glyph, int width) => new()
    {
        Text = text, Glyph = glyph, Width = width, Font = Theme.BoldFont, Height = 40,
        BaseColor = Theme.Danger, HoverColor = Theme.DangerDark, PressColor = Theme.DangerDark
    };

    public static FlatButton Secondary(string text, string glyph, int width) => new()
    {
        Text = text, Glyph = glyph, Width = width, ForeColor = Theme.Text,
        BaseColor = Theme.Secondary, HoverColor = Theme.SecondaryHover, PressColor = Theme.SecondaryPress,
        BorderColor = Theme.Border
    };

    public void ApplyPrimary()
    {
        BaseColor = Theme.Accent;
        HoverColor = Theme.AccentDark;
        PressColor = Theme.AccentDark;
        ForeColor = Color.White;
        BorderColor = Color.Transparent;
        Invalidate();
    }

    public void ApplySecondary()
    {
        BaseColor = Theme.Secondary;
        HoverColor = Theme.SecondaryHover;
        PressColor = Theme.SecondaryPress;
        ForeColor = Theme.Text;
        BorderColor = Theme.Border;
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs e) { _pressed = true; Invalidate(); base.OnMouseDown(e); }
    protected override void OnMouseUp(MouseEventArgs e) { _pressed = false; Invalidate(); base.OnMouseUp(e); }
    protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Theme.Surface);

        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        Color fill = BaseColor;
        if (!Enabled)
            fill = Theme.Mix(BaseColor, Theme.Bg, 0.6f);
        else if (_pressed)
            fill = PressColor;
        else if (_hover)
            fill = HoverColor;

        using (var path = Theme.Rounded(rect, CornerRadius))
        using (var brush = new SolidBrush(fill))
            g.FillPath(brush, path);

        if (BorderColor != Color.Transparent)
        {
            using var pen = new Pen(BorderColor);
            using var path = Theme.Rounded(rect, CornerRadius);
            g.DrawPath(pen, path);
        }

        Color fore = Enabled ? ForeColor : Theme.Mix(ForeColor, fill, 0.55f);
        DrawContent(g, fore);
    }

    private void DrawContent(Graphics g, Color fore)
    {
        string text = Text;
        Image? icon = string.IsNullOrEmpty(Glyph) ? null : IconFactory.Glyph(Glyph, GlyphSize, fore);
        if (icon == null)
        {
            TextRenderer.DrawText(g, text, Font, ClientRectangle, fore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            return;
        }

        // Botón solo-icono: centrar el glifo sin reservar espacio para texto.
        if (string.IsNullOrEmpty(text))
        {
            g.DrawImage(icon, (Width - icon.Width) / 2, (Height - icon.Height) / 2, icon.Width, icon.Height);
            return;
        }

        int gap = 8;
        Size textSize = TextRenderer.MeasureText(text, Font, Size.Empty, TextFormatFlags.NoPadding);
        int total = icon.Width + gap + textSize.Width;
        int x = Math.Max(8, (Width - total) / 2);
        int iconY = (Height - icon.Height) / 2;
        g.DrawImage(icon, x, iconY, icon.Width, icon.Height);
        TextRenderer.DrawText(g, text, Font,
            new Rectangle(x + icon.Width + gap, 0, textSize.Width + 4, Height), fore,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }
}
