public static class TransferPaths
{
    // Compone la carpeta final de destino: base [\ estudiante] [\ subcarpeta].
    // Se extrae a un helper estático y sin dependencias de UI para poder probarlo.
    public static string BuildDestinationDirectory(string destinoBase, string student, string subcarpeta, bool createFolders)
    {
        var directory = (destinoBase ?? "").Trim();
        if (!createFolders)
            return directory;

        student = (student ?? "").Trim();
        if (student.Length > 0)
        {
            if (!student.StartsWith("FAST-", StringComparison.OrdinalIgnoreCase))
                student = "FAST-" + student;
            directory = Path.Combine(directory, student);
        }

        subcarpeta = (subcarpeta ?? "").Trim();
        return subcarpeta.Length == 0 ? directory : Path.Combine(directory, subcarpeta);
    }
}
