using System.IO;

public class CsvLoggerTests
{
    [Fact]
    public void Export_WritesHeaderAndEscapesSpecialCharacters()
    {
        var path = Path.Combine(Path.GetTempPath(), "csvtest_" + Guid.NewGuid().ToString("N") + ".csv");
        try
        {
            var results = new List<TransferResult>
            {
                new()
                {
                    FileName = "informe,2024.txt",
                    SizeBytes = 1000,
                    ElapsedSeconds = 1.0,
                    Success = true,
                    Timestamp = new DateTime(2024, 1, 1, 0, 0, 0)
                },
                new()
                {
                    FileName = "simple.bin",
                    SizeBytes = 2000,
                    ElapsedSeconds = 0.5,
                    Success = false,
                    Error = "fallo: \"x\"",
                    Timestamp = new DateTime(2024, 1, 1, 0, 0, 1)
                }
            };

            CsvLogger.Export(path, results);
            var lines = File.ReadAllLines(path);

            Assert.Equal("Archivo,TamanoBytes,Tiempo_s,MB_s,Mb_s,Ideal_s,Porcentaje,HASH_OK,Exito,Error,Fecha", lines[0]);
            Assert.Contains("\"informe,2024.txt\"", lines[1]);
            Assert.StartsWith("simple.bin,2000,", lines[2]);
            Assert.Contains("\"fallo: \"\"x\"\"\"", lines[2]);
            Assert.EndsWith("2024-01-01 00:00:01", lines[2]);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
