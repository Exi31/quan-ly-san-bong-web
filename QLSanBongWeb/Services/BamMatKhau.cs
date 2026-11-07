using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace QLSanBongWeb.Services;

public static class BamMatKhau
{
    public static string Tao(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        const int rounds = 600000;
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, rounds, HashAlgorithmName.SHA256, 32);
        return $"PBKDF2-SHA256${rounds}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool KiemTra(string password, string stored)
    {
        if (password == null || string.IsNullOrWhiteSpace(stored)) return false;
        var parts = stored.Split('$');
        if (parts.Length != 4 || parts[0] != "PBKDF2-SHA256" ||
            !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out int rounds) ||
            rounds < 100000 || rounds > 2000000) return false;
        try
        {
            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expected = Convert.FromBase64String(parts[3]);
            if (salt.Length != 16 || expected.Length != 32) return false;
            byte[] actual = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, rounds, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
    }
}
