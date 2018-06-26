using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Utility
{
    /// <summary>
    /// 加密安全类
    /// </summary>
    public static class Encryption
    {
        /// <summary>
        /// RgbKey
        /// </summary>
        private static byte[] RgbKey = { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 };
        /// <summary>
        /// RgbIV
        /// </summary>
        private static byte[] RgbIV = { 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <returns></returns>
        public static string DESEncrypt(this string encryptString)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(encryptString.NullThenEmpty());
                using (var des = new DESCryptoServiceProvider())
                {
                    using (var stream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(stream, des.CreateEncryptor(RgbKey, RgbIV), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(bytes, 0, bytes.Length);
                            cryptoStream.FlushFinalBlock();
                            return Convert.ToBase64String(stream.ToArray());
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>    
        /// <returns></returns>
        public static string DESDecrypt(this string decryptString)
        {
            try
            {
                var bytes = Convert.FromBase64String(decryptString);
                using (var des = new DESCryptoServiceProvider())
                {
                    using (var stream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(stream, des.CreateDecryptor(RgbKey, RgbIV), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(bytes, 0, bytes.Length);
                            cryptoStream.FlushFinalBlock();
                            return Encoding.UTF8.GetString(stream.ToArray());
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取Md5
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static string GetMD5(string source)
        {
            var data = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(source)).Select(item => item.ToString("X2")).ToArray();
            return string.Join(string.Empty, data);
        }

        /// <summary>       
        /// 密码MD5后反转再MD5
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static string GetReverseMd5(string source)
        {
            var reverseMd5 = Encryption.GetMD5(source).Reverse().ToArray();
            return Encryption.GetMD5(new string(reverseMd5));
        }

        /// <summary>
        /// HMACSHA1 加密
        /// </summary>
        /// <param name="source">待加密字符串</param>
        /// <param name="Key">秘钥</param>
        /// <returns></returns>
        public static string ToBase64hmac(string source, string Key)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(Key))
            {
                return string.Empty;
            }
            HMACSHA1 myHMACSHA1 = new HMACSHA1(Encoding.UTF8.GetBytes(Key));
            byte[] byteText = myHMACSHA1.ComputeHash(Encoding.UTF8.GetBytes(source));
            return System.Convert.ToBase64String(byteText);
        }

        #region RSA 不对称加解密/签名
        /// <summary>
        /// 生成私钥和公钥 arr对私钥[0]arr对公钥[1]
        /// </summary>
        /// <returns></returns>
        public static string[] GenerateKeys()
        {
            string[] sKeys = new String[2];
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            sKeys[0] = rsa.ToXmlString(true);
            sKeys[1] = rsa.ToXmlString(false);
            return sKeys;
        }

        /// <summary>
        /// RSA 加密
        /// </summary>
        /// <param name="sSource" >加密内容</param>
        /// <param name="sPublicKey" >公钥</param>
        /// <returns></returns>
        public static string RSAEncrypt(string sSource, string sPublicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string plaintext = sSource;
            rsa.FromXmlString(sPublicKey);
            byte[] cipherbytes;
            byte[] byteEn = rsa.Encrypt(Encoding.UTF8.GetBytes("a"), false);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(plaintext), false);

            StringBuilder sbString = new StringBuilder();
            for (int i = 0; i < cipherbytes.Length; i++)
            {
                sbString.Append(cipherbytes[i] + ",");
            }
            return sbString.ToString();
        }

        /// <summary>
        /// RSA 解密
        /// </summary>
        /// <param name="sSource">密文</param>
        /// <param name="sPrivateKey">私钥</param>
        /// <returns></returns>
        public static string RASDecrypt(String sSource, string sPrivateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(sPrivateKey);
            byte[] byteEn = rsa.Encrypt(Encoding.UTF8.GetBytes("a"), false);
            string[] sBytes = sSource.Split(',');

            for (int j = 0; j < sBytes.Length; j++)
            {
                if (sBytes[j] != "")
                {
                    byteEn[j] = Byte.Parse(sBytes[j]);
                }
            }
            byte[] plaintbytes = rsa.Decrypt(byteEn, false);
            return Encoding.UTF8.GetString(plaintbytes);
        }

        /// <summary>
        /// RAS 对数据签名
        /// </summary>
        /// <param name="str_DataToSign">需签名数据</param>
        /// <param name="str_Private_Key">私钥</param>
        /// <returns></returns>
        public static string RASHashAndSign(string str_DataToSign, string str_Private_Key)
        {
            ASCIIEncoding ByteConverter = new ASCIIEncoding();
            byte[] DataToSign = ByteConverter.GetBytes(str_DataToSign);
            try
            {
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
                RSAalg.FromXmlString(str_Private_Key);
                byte[] signedData = RSAalg.SignData(DataToSign, new SHA1CryptoServiceProvider());
                string str_SignedData = Convert.ToBase64String(signedData);
                return str_SignedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// 验证RAS 签名
        /// </summary>
        /// <param name="str_DataToVerify">明文</param>
        /// <param name="str_SignedData">签名数据</param>
        /// <param name="str_Public_Key">公钥</param>
        /// <returns></returns>
        public static bool RASVerifySignedHash(string str_DataToVerify, string str_SignedData, string str_Public_Key)
        {
            byte[] SignedData = Convert.FromBase64String(str_SignedData);

            ASCIIEncoding ByteConverter = new ASCIIEncoding();
            byte[] DataToVerify = ByteConverter.GetBytes(str_DataToVerify);
            try
            {
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
                RSAalg.FromXmlString(str_Public_Key);
                //RSAalg.ImportCspBlob(Convert.FromBase64String(str_Public_Key));
                return RSAalg.VerifyData(DataToVerify, new SHA1CryptoServiceProvider(), SignedData);

            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }
        #endregion
    }
}
