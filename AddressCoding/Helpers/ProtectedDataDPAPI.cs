using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AddressCoding.Helpers
{
    /// <summary>
    /// Класс для реализации шифрования и дешифровки данных
    /// </summary>
    public static class ProtectedDataDPAPI
    {
        /// <summary>
        /// Строка для поучения энтропии
        /// </summary>
        private static readonly string _entropyString = $"{Environment.MachineName}{Environment.UserName}";

        public static Entities.EntityResult<string> EncryptData(string data)
        {
            Entities.EntityResult<string> result = new Entities.EntityResult<string>();

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    byte[] d = Encoding.UTF8.GetBytes(data);
                    byte[] crypted = ProtectedData.Protect(d, GetEntropy(), DataProtectionScope.CurrentUser);
                    result.Object = Convert.ToBase64String(crypted);
                }
                catch (Exception ex)
                {
                    result.Error = ex;
                }
            }

            return result;
        }

        public static Entities.EntityResult<string> DecryptData(string data)
        {
            Entities.EntityResult<string> result = new Entities.EntityResult<string>();

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    byte[] d = Convert.FromBase64String(data);
                    byte[] decrypted = ProtectedData.Unprotect(d, GetEntropy(), DataProtectionScope.CurrentUser);
                    result.Object = Encoding.UTF8.GetString(decrypted);
                }
                catch (Exception ex)
                {
                    result.Error = ex;
                }
            }

            return result;
        }

        /// <summary>
        /// Метод для получения энтропии
        /// </summary>
        /// <returns>Энтропию</returns>
        private static byte[] GetEntropy()
        {
            MD5 md5 = MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(_entropyString));
        }
    }
}