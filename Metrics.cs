using System.Globalization;
using System.Text;

public static class Metrics
{
    public const double DefaultNominalBitsPerSecond = 1_000_000_000.0;

    public static double IdealSeconds(long bytes, double nominalBitsPerSecond)
        => nominalBitsPerSecond <= 0 ? 0 : bytes * 8.0 / nominalBitsPerSecond;

    public static double MegaBytesPerSecond(long bytes, double seconds)
        => seconds <= 0 ? 0 : bytes / (seconds * 1_000_000.0);

    public static double MegabitsPerSecond(long bytes, double seconds)
        => MegaBytesPerSecond(bytes, seconds) * 8.0;

    public static double PercentOfNominal(long bytes, double seconds, double nominalBitsPerSecond)
        => nominalBitsPerSecond <= 0 || seconds <= 0
            ? 0
            : (bytes * 8.0 / seconds) / nominalBitsPerSecond * 100.0;

    public static string FormatBytes(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB" };
        double size = bytes;
        int unit = 0;
        while (size >= 1000 && unit < units.Length - 1)
        {
            size /= 1000;
            unit++;
        }

        return unit == 0
            ? $"{bytes} B"
            : $"{size.ToString("F2", CultureInfo.InvariantCulture)} {units[unit]}";
    }

    public static double ParseLinkSpeedToBitsPerSecond(string linkSpeed)
    {
        if (string.IsNullOrWhiteSpace(linkSpeed))
            return 0;

        var text = linkSpeed.Trim();
        double multiplier;
        if (text.Contains("Gbps", StringComparison.OrdinalIgnoreCase))
            multiplier = 1_000_000_000.0;
        else if (text.Contains("Mbps", StringComparison.OrdinalIgnoreCase))
            multiplier = 1_000_000.0;
        else if (text.Contains("Kbps", StringComparison.OrdinalIgnoreCase))
            multiplier = 1_000.0;
        else
            multiplier = 1.0;

        var number = new StringBuilder();
        foreach (var ch in text)
        {
            if (char.IsDigit(ch) || ch == '.' || ch == ',')
                number.Append(ch);
            else if (number.Length > 0)
                break;
        }

        var token = number.ToString().Replace(',', '.');
        if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            return value * multiplier;

        return 0;
    }
}
