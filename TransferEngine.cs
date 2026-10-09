using System.Diagnostics;
using System.Security.Cryptography;

public class ProgressInfo
{
    public string FileName { get; set; } = "";
    public long BytesCopied { get; set; }
    public long TotalBytes { get; set; }
    public double InstantMegaBytesPerSecond { get; set; }
    public double ElapsedSeconds { get; set; }
    public int PercentComplete => TotalBytes <= 0 ? 0 : (int)(BytesCopied * 100L / TotalBytes);
}

public class TransferEngine
{
    public int BufferSize { get; set; } = 4 * 1024 * 1024;
    public bool VerifyHash { get; set; }
    public double NominalBitsPerSecond { get; set; } = Metrics.DefaultNominalBitsPerSecond;

    public async Task<TransferResult> CopyAsync(string sourcePath, string destinationPath,
        IProgress<ProgressInfo>? progress, CancellationToken cancellationToken = default)
    {
        var result = new TransferResult
        {
            FileName = Path.GetFileName(sourcePath),
            SourcePath = sourcePath,
            DestinationPath = destinationPath
        };

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var destinationDirectory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            long totalBytes;
            double elapsedSeconds;
            string? sourceHash = null;

            using (var src = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                       BufferSize, FileOptions.SequentialScan | FileOptions.Asynchronous))
            using (var dst = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None,
                       BufferSize, FileOptions.Asynchronous))
            using (var incrementalHash = VerifyHash
                       ? IncrementalHash.CreateHash(HashAlgorithmName.SHA256)
                       : null)
            {
                totalBytes = src.Length;

                var buffer = new byte[BufferSize];
                long bytesCopied = 0;

                var windowStart = stopwatch.Elapsed;
                long windowBytes = 0;
                double instantMegaBytesPerSecond = 0;

                int read;
                while ((read = await src.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
                {
                    await dst.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                    incrementalHash?.AppendData(buffer, 0, read);
                    bytesCopied += read;

                    var now = stopwatch.Elapsed;
                    var windowSeconds = (now - windowStart).TotalSeconds;
                    if (windowSeconds >= 0.5)
                    {
                        instantMegaBytesPerSecond = (bytesCopied - windowBytes) / (windowSeconds * 1_000_000.0);
                        windowStart = now;
                        windowBytes = bytesCopied;
                    }

                    progress?.Report(new ProgressInfo
                    {
                        FileName = result.FileName,
                        BytesCopied = bytesCopied,
                        TotalBytes = totalBytes,
                        InstantMegaBytesPerSecond = instantMegaBytesPerSecond,
                        ElapsedSeconds = now.TotalSeconds
                    });
                }

                await dst.FlushAsync(cancellationToken);
                stopwatch.Stop();
                elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

                if (incrementalHash != null)
                    sourceHash = Convert.ToHexString(incrementalHash.GetHashAndReset());
            }

            result.SizeBytes = totalBytes;
            result.ElapsedSeconds = elapsedSeconds;
            result.IdealSeconds = Metrics.IdealSeconds(totalBytes, NominalBitsPerSecond);
            result.PercentOfNominal = Metrics.PercentOfNominal(totalBytes, elapsedSeconds, NominalBitsPerSecond);

            if (sourceHash != null)
            {
                var destinationHash = ComputeFileHash(destinationPath);
                result.HashVerified = string.Equals(sourceHash, destinationHash, StringComparison.OrdinalIgnoreCase);
                result.Hash = sourceHash;
            }

            result.Success = true;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            result.ElapsedSeconds = stopwatch.Elapsed.TotalSeconds;
            result.Success = false;
            result.Error = "Operación cancelada";
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.ElapsedSeconds = stopwatch.Elapsed.TotalSeconds;
            result.Success = false;
            result.Error = ex.Message;
        }

        return result;
    }

    private static string ComputeFileHash(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            4 * 1024 * 1024, FileOptions.SequentialScan);
        using var sha256 = SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(stream));
    }
}
