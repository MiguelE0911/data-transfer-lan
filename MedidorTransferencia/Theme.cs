using System.Drawing.Drawing2D;
using System.Drawing.Text;

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

public class Card : Panel
{
    public string Title { get; set; } = "";
    public Color Accent { get; set; } = Theme.Accent;
    public int CornerRadius { get; set; } = 12;

    public Card()
    {
        DoubleBuffered = true;
        BackColor = Theme.Surface;
        Padding = new Padding(16, 44, 16, 16);
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (Width > 0 && Height > 0)
        {
            using var path = Theme.Rounded(new Rectangle(0, 0, Width, Height), CornerRadius);
            Region = new Region(path);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using (var brush = new SolidBrush(BackColor))
            g.FillRectangle(brush, ClientRectangle);

        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using (var pen = new Pen(Theme.Border))
        using (var path = Theme.Rounded(rect, CornerRadius))
            g.DrawPath(pen, path);

        if (!string.IsNullOrEmpty(Title))
        {
            using var bar = new SolidBrush(Accent);
            g.FillRectangle(bar, new Rectangle(16, 16, 4, 18));
            TextRenderer.DrawText(g, Title, Theme.CardTitleFont, new Point(28, 15), Theme.Text);
        }
    }
}

public class RoundedProgress : Control
{
    private int _minimum;
    private int _maximum = 100;
    private int _value;

    public int Minimum { get => _minimum; set { _minimum = value; Invalidate(); } }
    public int Maximum { get => _maximum; set { _maximum = value; Invalidate(); } }
    public int Value { get => _value; set { _value = value; Invalidate(); } }
    public Color TrackColor { get; set; } = Color.FromArgb(0x24, 0x2E, 0x42);
    public Color BarColor { get; set; } = Theme.Accent;

    public RoundedProgress()
    {
        DoubleBuffered = true;
        Height = 20;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var track = new Rectangle(0, (Height - 12) / 2, Width - 1, 12);
        using (var path = Theme.Rounded(track, 6))
        using (var brush = new SolidBrush(TrackColor))
            g.FillPath(brush, path);

        double span = _maximum - _minimum;
        double fraction = span <= 0 ? 0 : (_value - _minimum) / span;
        fraction = Math.Clamp(fraction, 0, 1);
        int fillWidth = (int)(track.Width * fraction);
        if (fillWidth >= 3)
        {
            var fill = new Rectangle(track.X, track.Y, fillWidth, track.Height);
            using var path = Theme.Rounded(fill, 6);
            using var brush = new LinearGradientBrush(fill, Theme.Mix(BarColor, Color.White, 0.15f), BarColor, 0f);
            g.FillPath(brush, path);
        }
    }
}

public class SpeedChart : Control
{
    private readonly List<(double Seconds, double MegaBytes)> _points = new();

    public string Subtitle { get; set; } = "";
    public Color LineColor { get; set; } = Theme.Accent;

    public SpeedChart()
    {
        DoubleBuffered = true;
        BackColor = Theme.SurfaceAlt;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    public void Reset()
    {
        _points.Clear();
        Subtitle = "";
        Invalidate();
    }

    public void AddSample(double seconds, double megaBytes)
    {
        if (megaBytes < 0) megaBytes = 0;
        if (_points.Count >= 3000)
            _points.RemoveAt(0);
        _points.Add((seconds, megaBytes));
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(BackColor);

        int left = 46, right = 12, top = 12, bottom = 24;
        var plot = new Rectangle(left, top, Math.Max(10, Width - left - right), Math.Max(10, Height - top - bottom));

        using (var region = Theme.Rounded(new Rectangle(1, 1, Width - 3, Height - 3), 8))
        using (var bg = new SolidBrush(BackColor))
            g.FillPath(bg, region);

        double maxY = 5;
        foreach (var p in _points)
            if (p.MegaBytes > maxY) maxY = p.MegaBytes;
        maxY = NiceCeiling(maxY * 1.15);
        double maxX = _points.Count > 0 ? Math.Max(_points[^1].Seconds, 1e-3) : 1;

        using (var gridPen = new Pen(Theme.Grid))
        using (var axisPen = new Pen(Theme.Border))
        using (var labelBrush = new SolidBrush(Theme.Muted))
        {
            for (int i = 0; i <= 4; i++)
            {
                int y = plot.Bottom - (int)(plot.Height * i / 4.0);
                g.DrawLine(i == 0 ? axisPen : gridPen, plot.Left, y, plot.Right, y);
                double value = maxY * i / 4.0;
                string label = value.ToString("0.#");
                var size = g.MeasureString(label, Theme.SmallFont);
                g.DrawString(label, Theme.SmallFont, labelBrush, plot.Left - size.Width - 6, y - size.Height / 2);
            }

            for (int i = 0; i <= 4; i++)
            {
                int x = plot.Left + (int)(plot.Width * i / 4.0);
                g.DrawLine(gridPen, x, plot.Top, x, plot.Bottom);
                string label = (maxX * i / 4.0).ToString("0.#") + "s";
                var size = g.MeasureString(label, Theme.SmallFont);
                g.DrawString(label, Theme.SmallFont, labelBrush, x - size.Width / 2, plot.Bottom + 4);
            }
        }

        if (_points.Count >= 2)
        {
            var line = new PointF[_points.Count];
            for (int i = 0; i < _points.Count; i++)
            {
                float x = plot.Left + (float)(plot.Width * (_points[i].Seconds / maxX));
                float y = plot.Bottom - (float)(plot.Height * (_points[i].MegaBytes / maxY));
                line[i] = new PointF(x, Math.Clamp(y, plot.Top, plot.Bottom));
            }

            var area = new PointF[line.Length + 2];
            Array.Copy(line, area, line.Length);
            area[^2] = new PointF(line[^1].X, plot.Bottom);
            area[^1] = new PointF(line[0].X, plot.Bottom);

            using (var fillPath = new GraphicsPath())
            {
                fillPath.AddPolygon(area);
                using var gradient = new LinearGradientBrush(
                    plot, Color.FromArgb(120, LineColor), Color.FromArgb(10, LineColor), 90f);
                g.FillPath(gradient, fillPath);
            }

            using var linePen = new Pen(LineColor, 2.2f) { LineJoin = LineJoin.Round };
            g.DrawLines(linePen, line);
        }
        else
        {
            TextRenderer.DrawText(g, "Sin datos de transferencia", Theme.BaseFont,
                plot, Theme.Muted, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        using var titleBrush = new SolidBrush(Theme.Text);
        string header = string.IsNullOrEmpty(Subtitle) ? "Velocidad (MB/s) vs tiempo" : Subtitle;
        g.DrawString(header, Theme.SmallFont, titleBrush, 10, 6);
    }

    private static double NiceCeiling(double value)
    {
        if (value <= 5) return 5;
        double magnitude = Math.Pow(10, Math.Floor(Math.Log10(value)));
        double normalized = value / magnitude;
        double nice = normalized <= 1 ? 1 : normalized <= 2 ? 2 : normalized <= 5 ? 5 : 10;
        return nice * magnitude;
    }
}

public class SpeedGauge : Control
{
    private double _value;
    private double _maximum = 125;

    public double Value { get => _value; set { _value = value; if (value > _maximum) _maximum = value * 1.12; Invalidate(); } }
    public double Maximum { get => _maximum; set { _maximum = value <= 0 ? 1 : value; Invalidate(); } }
    public string Unit { get; set; } = "MB/s";
    public Color AccentColor { get; set; } = Theme.Accent;
    public string Caption { get; set; } = "Velocidad promedio";

    public SpeedGauge()
    {
        DoubleBuffered = true;
        BackColor = Theme.Surface;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(BackColor);

        int margin = 16;
        int cx = Width / 2;
        int cy = (int)(Height * 0.80);
        int radius = Math.Min(Width / 2 - margin, (int)(Height * 0.80) - margin);
        if (radius < 20)
            return;

        var arcs = new Rectangle(cx - radius, cy - radius, radius * 2, radius * 2);
        double fraction = _maximum <= 0 ? 0 : Math.Clamp(_value / _maximum, 0, 1);

        using (var trackPen = new Pen(Color.FromArgb(0x2A, 0x35, 0x48), 12f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            g.DrawArc(trackPen, arcs, 180, 180);

        using (var valuePen = new Pen(AccentColor, 12f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
        {
            if (fraction > 0.001)
                g.DrawArc(valuePen, arcs, 180, (float)(fraction * 180));
        }

        using (var tickPen = new Pen(Theme.Muted))
        using (var tickBrush = new SolidBrush(Theme.Muted))
        {
            for (int i = 0; i <= 10; i++)
            {
                double angle = Math.PI * (180 + i * 18) / 180.0;
                float outer = radius - 8;
                float inner = radius - 16;
                float x1 = cx + (float)(outer * Math.Cos(angle));
                float y1 = cy + (float)(outer * Math.Sin(angle));
                float x2 = cx + (float)(inner * Math.Cos(angle));
                float y2 = cy + (float)(inner * Math.Sin(angle));
                g.DrawLine(tickPen, x1, y1, x2, y2);
            }
        }

        double needleAngle = Math.PI * (180 + fraction * 180) / 180.0;
        float needleLength = radius - 22;
        float nx = cx + (float)(needleLength * Math.Cos(needleAngle));
        float ny = cy + (float)(needleLength * Math.Sin(needleAngle));
        using (var needlePen = new Pen(Theme.Text, 3f) { EndCap = LineCap.Round })
            g.DrawLine(needlePen, cx, cy, nx, ny);
        using (var hub = new SolidBrush(Theme.Text))
            g.FillEllipse(hub, cx - 6, cy - 6, 12, 12);
        using (var hubInner = new SolidBrush(AccentColor))
            g.FillEllipse(hubInner, cx - 3, cy - 3, 6, 6);

        string valueText = _value.ToString("0.0");
        var valueSize = g.MeasureString(valueText, Theme.GaugeValueFont);
        using (var valueBrush = new SolidBrush(Theme.Text))
            g.DrawString(valueText, Theme.GaugeValueFont, valueBrush, cx - valueSize.Width / 2, cy - radius * 0.72f);

        var unitSize = g.MeasureString(Unit, Theme.SmallFont);
        using (var unitBrush = new SolidBrush(Theme.Muted))
            g.DrawString(Unit, Theme.SmallFont, unitBrush, cx - unitSize.Width / 2, cy - radius * 0.72f + valueSize.Height - 4);

        var captionSize = g.MeasureString(Caption, Theme.SmallFont);
        using (var captionBrush = new SolidBrush(Theme.Muted))
            g.DrawString(Caption, Theme.SmallFont, captionBrush, cx - captionSize.Width / 2, cy + 6);
    }
}

public static class IconFactory
{
    private static readonly string Family;
    private static readonly Dictionary<(string, int, int), Image> Cache = new();

    static IconFactory()
    {
        var installed = new InstalledFontCollection();
        var names = installed.Families.Select(f => f.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        Family = new[] { "Segoe Fluent Icons", "Segoe MDL2 Assets" }.FirstOrDefault(names.Contains) ?? "Segoe UI Symbol";
    }

    public static Image Glyph(string glyph, int size, Color color)
    {
        var key = (glyph, size, color.ToArgb());
        if (Cache.TryGetValue(key, out var cached))
            return cached;

        var bitmap = new Bitmap(size, size);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.Clear(Color.Transparent);
            using var font = new Font(Family, size * 0.95f, GraphicsUnit.Pixel);
            TextRenderer.DrawText(g, glyph, font, new Rectangle(0, 0, size, size), color,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

        Cache[key] = bitmap;
        return bitmap;
    }
}
