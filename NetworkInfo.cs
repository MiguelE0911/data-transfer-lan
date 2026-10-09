using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;

public class AdapterInfo
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string LinkSpeed { get; set; } = "";
    public string Status { get; set; } = "";
    public long SpeedBitsPerSecond { get; set; }
}

public static class NetworkInfo
{
    private const string AdapterScript =
        "Get-NetAdapter | Where-Object {$_.Status -eq 'Up'} | " +
        "Select-Object Name,InterfaceDescription,LinkSpeed,Status,Speed | ConvertTo-Json -Compress";

    public static List<AdapterInfo> GetActiveAdapters()
    {
        var adapters = GetFromPowerShell();
        if (adapters.Count == 0)
            adapters = GetFromDotNet();
        return adapters;
    }

    public static AdapterInfo? GetAdapterForTarget(string target)
    {
        var localAddress = ResolveLocalAddress(target);
        if (localAddress == null)
            return null;

        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            try
            {
                foreach (var unicast in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicast.Address.Equals(localAddress))
                        return FromNetworkInterface(networkInterface);
                }
            }
            catch
            {
                // Adaptadores virtuales pueden fallar al consultar propiedades.
            }
        }

        return null;
    }

    private static List<AdapterInfo> GetFromPowerShell()
    {
        var adapters = new List<AdapterInfo>();
        var (ok, output) = RunPowerShell(AdapterScript);
        if (!ok || string.IsNullOrWhiteSpace(output))
            return adapters;

        try
        {
            using var document = JsonDocument.Parse(output.Trim());
            var root = document.RootElement;
            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in root.EnumerateArray())
                    AddAdapter(adapters, element);
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                AddAdapter(adapters, root);
            }
        }
        catch
        {
            // Si el JSON no es válido se intenta el respaldo administrado.
        }

        return adapters;
    }

    private static void AddAdapter(List<AdapterInfo> adapters, JsonElement element)
    {
        var linkSpeed = GetString(element, "LinkSpeed");
        long bitsPerSecond = GetLong(element, "Speed");
        if (bitsPerSecond <= 0)
            bitsPerSecond = (long)Metrics.ParseLinkSpeedToBitsPerSecond(linkSpeed);

        adapters.Add(new AdapterInfo
        {
            Name = GetString(element, "Name"),
            Description = GetString(element, "InterfaceDescription"),
            LinkSpeed = string.IsNullOrWhiteSpace(linkSpeed) ? FormatLinkSpeed(bitsPerSecond) : linkSpeed,
            Status = GetString(element, "Status"),
            SpeedBitsPerSecond = bitsPerSecond
        });
    }

    private static List<AdapterInfo> GetFromDotNet()
    {
        var adapters = new List<AdapterInfo>();
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus != OperationalStatus.Up)
                continue;

            if (networkInterface.NetworkInterfaceType is not (NetworkInterfaceType.Ethernet
                or NetworkInterfaceType.GigabitEthernet or NetworkInterfaceType.Wireless80211))
                continue;

            adapters.Add(FromNetworkInterface(networkInterface));
        }

        return adapters;
    }

    private static AdapterInfo FromNetworkInterface(NetworkInterface networkInterface)
    {
        long bitsPerSecond = 0;
        try
        {
            bitsPerSecond = networkInterface.Speed;
            if (bitsPerSecond < 0)
                bitsPerSecond = 0;
        }
        catch
        {
            bitsPerSecond = 0;
        }

        return new AdapterInfo
        {
            Name = networkInterface.Name,
            Description = networkInterface.Description,
            LinkSpeed = FormatLinkSpeed(bitsPerSecond),
            Status = networkInterface.OperationalStatus.ToString(),
            SpeedBitsPerSecond = bitsPerSecond
        };
    }

    private static IPAddress? ResolveLocalAddress(string target)
    {
        if (string.IsNullOrWhiteSpace(target))
            return null;

        var host = target.Trim();
        if (host.StartsWith(@"\\", StringComparison.Ordinal))
        {
            var withoutPrefix = host[2..];
            var separator = withoutPrefix.IndexOfAny(new[] { '\\', '/' });
            host = separator >= 0 ? withoutPrefix[..separator] : withoutPrefix;
        }

        if (host.Length == 0)
            return null;

        try
        {
            using var udp = new UdpClient();
            udp.Connect(host, 445);
            return (udp.Client.LocalEndPoint as IPEndPoint)?.Address;
        }
        catch
        {
            return null;
        }
    }

    internal static string FormatLinkSpeed(long bitsPerSecond)
    {
        if (bitsPerSecond <= 0)
            return "0 bps";
        if (bitsPerSecond >= 1_000_000_000)
            return (bitsPerSecond / 1_000_000_000.0).ToString("0.##", CultureInfo.InvariantCulture) + " Gbps";
        if (bitsPerSecond >= 1_000_000)
            return (bitsPerSecond / 1_000_000.0).ToString("0.##", CultureInfo.InvariantCulture) + " Mbps";
        if (bitsPerSecond >= 1_000)
            return (bitsPerSecond / 1_000.0).ToString("0.##", CultureInfo.InvariantCulture) + " Kbps";
        return bitsPerSecond.ToString(CultureInfo.InvariantCulture) + " bps";
    }

    private static string GetString(JsonElement element, string propertyName)
        => element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? ""
            : "";

    private static long GetLong(JsonElement element, string propertyName)
        => element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.Number
            && value.TryGetInt64(out var result)
            ? result
            : 0;

    private static (bool ok, string output) RunPowerShell(string script)
    {
        foreach (var executable in new[] { "pwsh", "powershell" })
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = executable,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                startInfo.ArgumentList.Add("-NoProfile");
                startInfo.ArgumentList.Add("-NonInteractive");
                startInfo.ArgumentList.Add("-ExecutionPolicy");
                startInfo.ArgumentList.Add("Bypass");
                startInfo.ArgumentList.Add("-Command");
                startInfo.ArgumentList.Add(script);

                using var process = new Process { StartInfo = startInfo };
                process.Start();
                var stdout = process.StandardOutput.ReadToEnd();
                var stderr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                    return (true, stdout);

                if (!string.IsNullOrWhiteSpace(stdout))
                    return (true, stdout);

                return (false, stderr);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // Ejecutable no disponible; intenta con el siguiente.
            }
            catch
            {
                // Error inesperado; intenta con el siguiente.
            }
        }

        return (false, "");
    }
}
