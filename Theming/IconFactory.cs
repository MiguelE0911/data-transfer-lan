using System.Drawing.Drawing2D;
using System.Drawing.Text;

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
