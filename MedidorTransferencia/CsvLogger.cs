using System.Globalization;
using System.Text;

public static class CsvLogger
{
    public static void Export(string path, IEnumerable<TransferResult> results)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Archivo,TamanoBytes,Tiempo_s,MB_s,Mb_s,Ideal_s,Porcentaje,HASH_OK,Exito,Error,Fecha");

        foreach (var result in results)
        {
            builder.Append(Escape(result.FileName)).Append(',');
            builder.Append(result.SizeBytes.ToString(CultureInfo.InvariantCulture)).Append(',');
            builder.Append(result.ElapsedSeconds.ToString("F4", CultureInfo.InvariantCulture)).Append(',');
            builder.Append(result.MegaBytesPerSecond.ToString("F2", CultureInfo.InvariantCulture)).Append(',');
            builder.Append(result.MegabitsPerSecond.ToString("F2", CultureInfo.InvariantCulture)).Append(',');
            builder.Append(result.IdealSeconds.ToString("F4", CultureInfo.InvariantCulture)).Append(',');
            builder.Append(result.PercentOfNominal.ToString("F2", CultureInfo.InvariantCulture)).Append(',');
            builder.Append(result.HashVerified ? "OK" : "NO").Append(',');
            builder.Append(result.Success ? "SI" : "NO").Append(',');
            builder.Append(Escape(result.Error ?? "")).Append(',');
            builder.Append(Escape(result.Timestamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)));
            builder.AppendLine();
        }

        File.WriteAllText(path, builder.ToString(), new UTF8Encoding(true));
    }

    private static string Escape(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        var escaped = value.Replace("\"", "\"\"");
        var needsQuotes = value.Contains(',') || value.Contains('"') ||
                          value.Contains('\n') || value.Contains('\r');
        return needsQuotes ? $"\"{escaped}\"" : escaped;
    }
}
