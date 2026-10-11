using System.Text.Json;

// Preferencias persistentes de la aplicación (recordar destino, usuario y opciones
// entre ejecuciones). Se guardan en %AppData%\MedidorTransferencia\settings.json.
public class AppSettings
{
    public string Destination { get; set; } = @"\\192.168.100.10\Velocidad";
    public string Student { get; set; } = "FAST-Miguel";
    public string User { get; set; } = "FAST";
    public string Password { get; set; } = "";
    public decimal LimitMbps { get; set; } = 1000M;
    public int BufferMb { get; set; } = 4;
    public int Retries { get; set; } = 1;
    public bool CreateFolders { get; set; } = true;
    public bool VerifyHash { get; set; }
    public bool ShowResult { get; set; } = true;

    private static string FilePath
    {
        get
        {
            var directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MedidorTransferencia");
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, "settings.json");
        }
    }

    public static AppSettings Load()
    {
        try
        {
            var path = FilePath;
            if (File.Exists(path))
            {
                var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(path));
                if (settings != null)
                    return settings;
            }
        }
        catch
        {
            // Si el archivo está corrupto se usan los valores por defecto.
        }

        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            File.WriteAllText(FilePath,
                JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch
        {
            // No es crítico si no se pueden guardar las preferencias.
        }
    }

    public static void Clear()
    {
        try
        {
            var path = FilePath;
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // Ignorado.
        }
    }
}
