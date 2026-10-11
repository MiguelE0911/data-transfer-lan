using System.Drawing.Drawing2D;

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
