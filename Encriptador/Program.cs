using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Encriptador
{
    class Program
    {
        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Número de parámetros incorrecto: uso Encriptador p1 p2");
                Console.WriteLine("p1 = 0 Encriptar / p1 = 1 Desencriptar");
                Console.WriteLine("p2 = Texto a encriptar o desencriptar");
                Console.WriteLine(" --> Pulse INTRO para terminar");
                Console.ReadLine();
                return;
            }
            int opcion = int.Parse(args[0]);
            if (opcion == 0)
            {
                string toEncrypt = args[1];
                string encrypted = Encrypt(toEncrypt);
                Console.WriteLine("El texto encriptado es ({0}) [sin paréntesis], cópielo en lugar seguro", encrypted);
                Console.WriteLine(" --> Pulse INTRO para terminar");
                Console.ReadLine();
                return;
            }
            else
            {
                string encrypted = args[1];
                string decrypted = Decrypt(encrypted);
                Console.WriteLine("Texto descifrado: ({0}) [sin paréntesis]", decrypted);
                Console.WriteLine("--> Pulse INTRO para finalizar");
                Console.ReadLine();
            }
        }

        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
    }
}
