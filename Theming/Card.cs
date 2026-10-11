using System.Drawing.Drawing2D;

public class Card : Panel
{
    private int _cornerRadius = 12;

    public string Title { get; set; } = "";
    public Color Accent { get; set; } = Theme.Accent;

    public int CornerRadius
    {
        get => _cornerRadius;
        set
        {
            _cornerRadius = value;
            UpdateRegion();
            Invalidate();
        }
    }

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
        UpdateRegion();
    }

    private void UpdateRegion()
    {
        if (Width <= 0 || Height <= 0)
            return;
        using var path = Theme.Rounded(new Rectangle(0, 0, Width, Height), _cornerRadius);
        Region = new Region(path);
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
