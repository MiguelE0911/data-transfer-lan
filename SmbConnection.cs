using System.Runtime.InteropServices;

public static class SmbConnection
{
    public static bool Connect(string uncPath, string user, string password, out string message)
    {
        if (string.IsNullOrWhiteSpace(uncPath))
        {
            message = "La ruta de destino está vacía.";
            return false;
        }

        var resource = new NetResource
        {
            dwType = ResourceTypeDisk,
            lpRemoteName = uncPath.Trim()
        };

        string? username = string.IsNullOrWhiteSpace(user) ? null : user.Trim();
        string? secret = string.IsNullOrEmpty(password) ? null : password;

        int code = WNetAddConnection2(ref resource, secret, username, ConnectTemporary);
        if (code == 0)
        {
            message = "Conexión establecida.";
            return true;
        }

        message = DescribeError(code);
        return false;
    }

    public static bool Disconnect(string uncPath, out string message)
    {
        if (string.IsNullOrWhiteSpace(uncPath))
        {
            message = "La ruta de destino está vacía.";
            return false;
        }

        int code = WNetCancelConnection2(uncPath.Trim(), 0, true);
        if (code == 0 || code == ErrorNotConnected)
        {
            message = "Conexión eliminada.";
            return true;
        }

        message = DescribeError(code);
        return false;
    }

    private static string DescribeError(int code)
    {
        string detail = code switch
        {
            5 => "Acceso denegado (revise usuario y contraseña).",
            53 => "No se encontró el equipo o el recurso compartido (BOSGAME apagada o fuera de la red).",
            67 => "No se encontró el recurso de red.",
            85 => "Ya existe una conexión con ese nombre.",
            86 => "Contraseña incorrecta.",
            1202 => "El recurso ya está recordado con otras credenciales.",
            1208 => "No se pudo abrir el perfil de usuario.",
            1219 => "Conflicto de credenciales: la sesión ya está conectada con otro usuario. Desconecte e intente de nuevo.",
            1222 => "No hay red disponible.",
            1223 => "Operación cancelada.",
            1317 => "El usuario no existe.",
            1326 => "Nombre de usuario o contraseña incorrectos.",
            _ => "Error de red al conectar el recurso compartido."
        };
        return $"{detail} (código {code})";
    }

    private const uint ResourceTypeDisk = 0x00000001;
    private const uint ConnectTemporary = 0x00000004;
    private const int ErrorNotConnected = 2250;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NetResource
    {
        public uint dwScope;
        public uint dwType;
        public uint dwDisplayType;
        public uint dwUsage;
        public string? lpLocalName;
        public string? lpRemoteName;
        public string? lpComment;
        public string? lpProvider;
    }

    [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
    private static extern int WNetAddConnection2(ref NetResource lpNetResource, string? lpPassword, string? lpUsername, uint dwFlags);

    [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
    private static extern int WNetCancelConnection2(string lpName, uint dwFlags, bool fForce);
}
