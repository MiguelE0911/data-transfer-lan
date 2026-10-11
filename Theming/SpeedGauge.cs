using System.Drawing.Drawing2D;

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
