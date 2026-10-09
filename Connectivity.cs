using System.Net.NetworkInformation;
using System.Net.Sockets;

public static class Connectivity
{
    public static (bool Ok, string Message) Check(string pathOrUnc)
    {
        var target = pathOrUnc.Trim();
        if (string.IsNullOrWhiteSpace(target))
            return (false, "La ruta de destino está vacía.");

        var host = ExtractHost(target);
        if (string.IsNullOrEmpty(host))
            return (false, "No se pudo determinar el equipo de destino a partir de la ruta.");

        bool pingOk = TryPing(host);
        bool smbOk = TryTcp(host, 445);

        bool directoryOk = Directory.Exists(target);
        string existing = FindExistingDirectory(target) ?? "";

        bool writeOk = false;
        string writeDetail = "carpeta no accesible";
        if (!string.IsNullOrEmpty(existing))
        {
            try
            {
                var probe = Path.Combine(existing, $".verif_{Guid.NewGuid():N}.tmp");
                File.WriteAllText(probe, "ok");
                File.Delete(probe);
                writeOk = true;
                writeDetail = "escritura correcta";
            }
            catch (Exception ex)
            {
                writeDetail = "sin permiso de escritura (" + ex.Message + ")";
            }
        }

        bool ok = directoryOk && writeOk;
        string message = string.Join(" | ", new[]
        {
            $"ping: {(pingOk ? "OK" : "sin respuesta")}",
            $"SMB 445: {(smbOk ? "OK" : "cerrado")}",
            $"carpeta: {(directoryOk ? "OK" : "no accesible")}",
            $"escritura: {writeDetail}"
        });

        return (ok, message);
    }

    private static string ExtractHost(string path)
    {
        var value = path;
        if (value.StartsWith(@"\\", StringComparison.Ordinal))
            value = value[2..];
        else if (value.Length >= 2 && char.IsLetter(value[0]) && value[1] == ':')
            return "";

        int separator = value.IndexOfAny(new[] { '\\', '/' });
        var host = separator >= 0 ? value[..separator] : value;
        int colon = host.IndexOf(':');
        if (colon >= 0)
            host = host[..colon];
        return host;
    }

    private static string? FindExistingDirectory(string path)
    {
        var directory = path;
        while (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            var parent = Path.GetDirectoryName(directory);
            if (string.IsNullOrEmpty(parent) || parent == directory)
                return null;
            directory = parent;
        }
        return string.IsNullOrEmpty(directory) ? null : directory;
    }

    private static bool TryPing(string host)
    {
        try
        {
            using var ping = new Ping();
            var reply = ping.Send(host, 1500);
            return reply.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryTcp(string host, int port)
    {
        try
        {
            using var client = new TcpClient();
            var task = client.ConnectAsync(host, port);
            return task.Wait(1500) && client.Connected;
        }
        catch
        {
            return false;
        }
    }
}
