public class MetricsTests
{
    [Fact]
    public void IdealSeconds_OneGigabyteAtOneGigabit_IsApproximatelyEightSeconds()
    {
        double result = Metrics.IdealSeconds(1_000_000_000, 1_000_000_000);
        Assert.Equal(8.0, result, 3);
    }

    [Fact]
    public void IdealSeconds_WithZeroLimit_ReturnsZero()
    {
        Assert.Equal(0, Metrics.IdealSeconds(1000, 0));
    }

    [Fact]
    public void PercentOfNominal_AtFullLinkSpeed_IsHundred()
    {
        // 125 MB en 1 s = 1000 Mbps = 100 % de 1 Gbps.
        double percent = Metrics.PercentOfNominal(125_000_000, 1.0, 1_000_000_000);
        Assert.Equal(100.0, percent, 2);
    }

    [Theory]
    [InlineData("1 Gbps", 1_000_000_000)]
    [InlineData("100 Mbps", 100_000_000)]
    [InlineData("2.5 Gbps", 2_500_000_000)]
    [InlineData("10 Kbps", 10_000)]
    public void ParseLinkSpeedToBitsPerSecond_ParsesCommonUnits(string text, double expected)
    {
        Assert.Equal(expected, Metrics.ParseLinkSpeedToBitsPerSecond(text), 0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("desconocido")]
    public void ParseLinkSpeedToBitsPerSecond_Invalid_ReturnsZero(string text)
    {
        Assert.Equal(0, Metrics.ParseLinkSpeedToBitsPerSecond(text));
    }

    [Theory]
    [InlineData(0, "0 B")]
    [InlineData(512, "512 B")]
    [InlineData(1000, "1.00 KB")]
    [InlineData(1_500_000, "1.50 MB")]
    public void FormatBytes_FormatsWithDecimalUnits(long bytes, string expected)
    {
        Assert.Equal(expected, Metrics.FormatBytes(bytes));
    }
}
