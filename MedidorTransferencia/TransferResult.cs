public class TransferResult
{
    public string FileName { get; set; } = "";
    public string SourcePath { get; set; } = "";
    public string DestinationPath { get; set; } = "";
    public long SizeBytes { get; set; }
    public double ElapsedSeconds { get; set; }
    public double IdealSeconds { get; set; }
    public double PercentOfNominal { get; set; }
    public bool HashVerified { get; set; }
    public string? Hash { get; set; }
    public bool Success { get; set; } = true;
    public string? Error { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public double MegaBytesPerSecond => ElapsedSeconds <= 0 ? 0 : SizeBytes / (ElapsedSeconds * 1_000_000.0);
    public double MegabitsPerSecond => MegaBytesPerSecond * 8.0;
}
