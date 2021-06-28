using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Xu.Common
{
    /// <summary>
    /// RSAC 帮助类
    /// </summary>
    public class RSACryption
    {
        private static readonly string PublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCD84Pei8dcNzhz8JqaL0d0j0og4YZL++BWNCxBumgBGGPl7dKkTO1hzeF3ixudgrp1TPafc0pCGXvvnczwBxuAb7nseoP5Oj4H9TVsOxmV1fqLMmikLGdwjvPlK1Yclck+9bbe+h8fzv0bWM3uvQb8fF6qhNMhJGs/oZTEQB2BSwIDAQAB";
        private static readonly string PrivateKey = "MIICWwIBAAKBgQCD84Pei8dcNzhz8JqaL0d0j0og4YZL++BWNCxBumgBGGPl7dKkTO1hzeF3ixudgrp1TPafc0pCGXvvnczwBxuAb7nseoP5Oj4H9TVsOxmV1fqLMmikLGdwjvPlK1Yclck+9bbe+h8fzv0bWM3uvQb8fF6qhNMhJGs/oZTEQB2BSwIDAQABAoGAQENjIAnXiFPkjKLLyPfpxxzaL3Vm4K7FLXavbzuH17C3Ro4zHo3Qtud8PapkQqwef26CVlnh+ptKvwKNgwETJRohUlM6Fwhl/CJpdRDnbV1w5y8/NlFyXgI4mS35H8wj0N7OWO1aRgsZ3g55o6w1iajSb0yys4ME0kdXTUQk0uECQQDzbGEw7zgJA1SfDGJC3+fIiEHAWL/LxzB6l9BmbXpEkKKsqYbxLaeBnNUusWa40hb/+Jej4+wX/sxVdRb6YDjzAkEAisTCCASLBH+f+MWzWEJrjiBDglcyw9nLS6ogO+YymMy1vNO588aAKts4SUQF+O+hyiIXL0Nbb+hr8wCyVG6sSQJASdYbGQPG5Hz9Iw1XlN9j6CDkiNqiusYdv2HjVd5pUvjoTyVRCEEH6TnQNEydUvxu+4/FN3JAP/sKsfVFVgbv3wJAQOcdyRo22vfGHlh5NUJ7g5HbgU6/U5K93rnHMbzM1WKJbbOpOTcSIvk9Lic+k9ugVCX1qgla7tBKDPG6dnr84QJAaWKR8MfVRpOUC7yANLj/+mbh9NTcBEt4grpziKpj5Plt8B6pQFdZtEJHOr4Vlc8f3MbzQPgC9vNcC4XY1bSQGg==";

        /// <summary>
        /// 生成PEM格式的公钥和密钥
        /// </summary>
        /// <param name="strength">长度</param>
        /// <returns>Item1:公钥；Item2:私钥；</returns>
        public static (string, string) CreateKeyPair(int strength = 1024)
        {
            RsaKeyPairGenerator r = new RsaKeyPairGenerator();
            r.Init(new KeyGenerationParameters(new SecureRandom(), strength));
            AsymmetricCipherKeyPair keys = r.GenerateKeyPair();

            TextWriter privateTextWriter = new StringWriter();
            PemWriter privatePemWriter = new PemWriter(privateTextWriter);
            privatePemWriter.WriteObject(keys.Private);
            privatePemWriter.Writer.Flush();

            TextWriter publicTextWriter = new StringWriter();
            PemWriter publicPemWriter = new PemWriter(publicTextWriter);
            publicPemWriter.WriteObject(keys.Public);
            publicPemWriter.Writer.Flush();

            var item1 = publicTextWriter.ToString().Replace("\r\n", "").Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "");
            var item2 = privateTextWriter.ToString().Replace("\r\n", "").Replace("-----END RSA PRIVATE KEY-----", "").Replace("-----BEGIN RSA PRIVATE KEY-----", "");
            return (item1, item2);
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="context"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string context, string publicKey = "")
        {
            if (string.IsNullOrEmpty(publicKey))
                publicKey = PublicKey;

            UTF8Encoding ByteConverter = new UTF8Encoding();
            byte[] DataToEncrypt = ByteConverter.GetBytes(context);
            try
            {
                var rsa = RSA.Create();
                rsa.ImportParameters(CreateRsaFromPublicKey(publicKey));

                byte[] bytes = rsa.Encrypt(DataToEncrypt, RSAEncryptionPadding.Pkcs1);
                string str = Convert.ToBase64String(bytes);
                return str;
            }
            catch (CryptographicException)
            {
                throw;
            }
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="context"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string RSADecrypt(string context, string privateKey = "")
        {
            if (string.IsNullOrEmpty(privateKey))
                privateKey = PrivateKey;

            byte[] DataToDecrypt = Convert.FromBase64String(context);
            try
            {
                var rsa = RSA.Create();
                rsa.ImportParameters(CreateRsaFromPrivateKey(privateKey));

                byte[] bytes = rsa.Decrypt(DataToDecrypt, RSAEncryptionPadding.Pkcs1);
                UTF8Encoding ByteConverter = new UTF8Encoding();
                string str = ByteConverter.GetString(bytes);
                return str;
            }
            catch (CryptographicException)
            {
                return null;
            }
        }

        #region 解析

        /// <summary>
        /// 使用私钥创建RSA实例
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        private static RSAParameters CreateRsaFromPrivateKey(string privateKey)
        {
            string tmp = privateKey.Replace("\r\n", "").Replace("-----END RSA PRIVATE KEY-----", "").Replace("-----BEGIN RSA PRIVATE KEY-----", "");
            var privateKeyBits = System.Convert.FromBase64String(tmp);
            var RSAparams = new RSAParameters();

            using (var binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                RSAparams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.D = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.P = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Q = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DP = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }
            return RSAparams;
        }

        /// <summary>
        /// 导入密钥算法
        /// </summary>
        /// <param name="binr"></param>
        /// <returns></returns>
        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            int count;
            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
            {
                byte highbyte = binr.ReadByte();
                byte lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        /// <summary>
        /// 使用公钥创建RSA实例
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        private static RSAParameters CreateRsaFromPublicKey(string publicKey)
        {
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] x509key;
            byte[] seq = new byte[15];
            var tmp = publicKey.Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "").Replace("\r\n", "");

            x509key = Convert.FromBase64String(tmp);
            _ = x509key.Length;

            using (var mem = new MemoryStream(x509key))
            {
                using (var binr = new BinaryReader(mem))
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return new RSAParameters();

                    seq = binr.ReadBytes(15);
                    if (!CompareBytearrays(seq, SeqOID))
                        return new RSAParameters();

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103)
                        binr.ReadByte();
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();
                    else
                        return new RSAParameters();

                    bt = binr.ReadByte();
                    if (bt != 0x00)
                        return new RSAParameters();

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return new RSAParameters();

                    twobytes = binr.ReadUInt16();
                    byte highbyte = 0x00;

                    byte lowbyte;
                    if (twobytes == 0x8102)
                        lowbyte = binr.ReadByte();
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte();
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return new RSAParameters();
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {
                        binr.ReadByte();
                        modsize -= 1;
                    }

                    byte[] modulus = binr.ReadBytes(modsize);

                    if (binr.ReadByte() != 0x02)
                        return new RSAParameters();
                    int expbytes = (int)binr.ReadByte();
                    byte[] exponent = binr.ReadBytes(expbytes);
                    var rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    return rsaKeyInfo;
                }
            }
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        #endregion 解析

        /// <summary>
        /// 判断一个字符串是否被Base64加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64(string str)
        {
            string base64Pattern = "^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$";
            return Regex.IsMatch(str, base64Pattern);
        }
    }
}