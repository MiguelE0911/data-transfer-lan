using System.Drawing.Drawing2D;

public class SpeedChart : Control
{
    private readonly List<(double Seconds, double MegaBytes)> _points = new();

    public string Subtitle { get; set; } = "";
    public Color LineColor { get; set; } = Theme.Accent;
    public double NominalMegaBytesPerSecond { get; set; }
    public double AverageMegaBytesPerSecond { get; set; }
    public bool ShowReferenceLines { get; set; } = true;

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
        AverageMegaBytesPerSecond = 0;
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
        if (ShowReferenceLines && NominalMegaBytesPerSecond > maxY)
            maxY = NominalMegaBytesPerSecond;
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

        if (ShowReferenceLines && NominalMegaBytesPerSecond > 0)
            DrawReferenceLine(g, plot, maxY, NominalMegaBytesPerSecond, Theme.Warning, DashStyle.Dash);

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

        if (ShowReferenceLines && AverageMegaBytesPerSecond > 0)
            DrawReferenceLine(g, plot, maxY, AverageMegaBytesPerSecond, Theme.Success, DashStyle.Dot);

        if (ShowReferenceLines && NominalMegaBytesPerSecond > 0)
        {
            string legend = $"límite {NominalMegaBytesPerSecond:0.#} MB/s";
            var size = g.MeasureString(legend, Theme.SmallFont);
            float lx = plot.Right - size.Width - 4;
            using (var pen = new Pen(Theme.Warning, 1.6f) { DashStyle = DashStyle.Dash })
                g.DrawLine(pen, lx - 22, plot.Top + size.Height / 2, lx - 4, plot.Top + size.Height / 2);
            using var brush = new SolidBrush(Theme.Muted);
            g.DrawString(legend, Theme.SmallFont, brush, lx, plot.Top);
        }
    }

    private static void DrawReferenceLine(Graphics g, Rectangle plot, double maxY, double value, Color color, DashStyle style)
    {
        if (value <= 0 || value > maxY || maxY <= 0)
            return;

        int y = plot.Bottom - (int)(plot.Height * (value / maxY));
        y = Math.Clamp(y, plot.Top, plot.Bottom);
        using var pen = new Pen(Color.FromArgb(210, color), 1.4f) { DashStyle = style };
        g.DrawLine(pen, plot.Left, y, plot.Right, y);
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
