using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CrivServer.Infrastructure.Services
{
    public class Encryptor:IEncryptor
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ManualEncryptor _manualProtectionProvider;
        private readonly string _purposeString;

        public Encryptor()
        {
            _purposeString = "manual.encryption";
            _manualProtectionProvider = new ManualEncryptor(Encoding.UTF8.GetBytes(_purposeString));
        }
        public Encryptor(string protector)
        {
            _purposeString = protector;
            _manualProtectionProvider = new ManualEncryptor(Encoding.UTF8.GetBytes(_purposeString));
        }
        public Encryptor(string password, string salt)
        {
            _purposeString = password;
            _manualProtectionProvider = new ManualEncryptor(Encoding.UTF8.GetBytes(salt));
        }

        public Encryptor(IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _purposeString = configuration["Protector"];
        }

        public Encryptor(IDataProtectionProvider dataProtectionProvider, string protector)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _purposeString = protector;
        }

        public string Encrypt(string input)
        {
            if (_dataProtectionProvider != null)
            {
                var protector = _dataProtectionProvider.CreateProtector(_purposeString);
                return protector.Protect(input);
            }
            return _manualProtectionProvider.EncryptString(input, _purposeString);
        }

        public string Decrypt(string cipherText)
        {
            if (_dataProtectionProvider != null)
            {
                var protector = _dataProtectionProvider.CreateProtector(_purposeString);
                return protector.Unprotect(cipherText);
            }
            return _manualProtectionProvider.DecryptString(cipherText, _purposeString);
        }

        public string ManualEncrypt(string input, string password)
        {
            return _manualProtectionProvider.EncryptString(input, password);
        }

        public string ManualDecrypt(string cipherText, string password)
        {
            return _manualProtectionProvider.DecryptString(cipherText, password);
        }

        public Task FileEncrypt(string filename, string password)
        {
             _manualProtectionProvider.EncryptFile(filename, password);
            return Task.CompletedTask;
        }

        public Task FileDecrypt(string filename, string password)
        {
            _manualProtectionProvider.DecryptFile(filename, password);
            return Task.CompletedTask;
        }

        public X509Certificate2 GenerateCertificate(string certName, string exportpassword)
        {
            var random = new SecureRandom();
            var certificateGenerator = new X509V3CertificateGenerator();

            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

            certificateGenerator.SetIssuerDN(new X509Name($"C=NL, O=SomeCompany, CN={certName}"));
            certificateGenerator.SetSubjectDN(new X509Name($"C=NL, O=SomeCompany, CN={certName}"));
            certificateGenerator.SetNotBefore(DateTime.UtcNow.Date);
            certificateGenerator.SetNotAfter(DateTime.UtcNow.Date.AddYears(1));

            const int strength = 2048;
            var keyGenerationParameters = new KeyGenerationParameters(random, strength);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);

            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            var issuerKeyPair = subjectKeyPair;
            const string signatureAlgorithm = "SHA256WithRSA";
            var signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, issuerKeyPair.Private);
            var bouncyCert = certificateGenerator.Generate(signatureFactory);

            // Lets convert it to X509Certificate2
            X509Certificate2 certificate;

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry($"{certName}_key", new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { new X509CertificateEntry(bouncyCert) });
            if(string.IsNullOrWhiteSpace(exportpassword))
                exportpassword = Guid.NewGuid().ToString("x");

            using (var ms = new System.IO.MemoryStream())
            {
                store.Save(ms, exportpassword.ToCharArray(), random);
                certificate = new X509Certificate2(ms.ToArray(), exportpassword, X509KeyStorageFlags.Exportable);
            }

            //Console.WriteLine($"Generated cert with thumbprint {certificate.Thumbprint}");
            return certificate;
        }
        // You can also load the certificate from to CurrentUser store
        private X509Certificate2 LoadCertificate(string subject)
        {
            var userStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            userStore.Open(OpenFlags.OpenExistingOnly);
            if (userStore.IsOpen)
            {
                var collection = userStore.Certificates.Find(X509FindType.FindBySubjectName, Environment.MachineName, false);
                if (collection.Count > 0)
                {
                    return collection[0];
                }
                userStore.Close();
            }
            return null;
        }

        // Or store the certificate to the local store.
        private void SaveCertificate(X509Certificate2 certificate)
        {
            var userStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            userStore.Open(OpenFlags.ReadWrite);
            userStore.Add(certificate);
            userStore.Close();
        }
    }

    public class ManualEncryptor
    {
        private readonly byte[] _saltBytes;
        public ManualEncryptor()
        {
            _saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        }
        public ManualEncryptor(byte[] salt)
        {
            _saltBytes = salt;
        }

        public string EncryptString(string text, string password)
        {
            byte[] baPwd = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            byte[] baPwdHash = SHA256Managed.Create().ComputeHash(baPwd);

            byte[] baText = Encoding.UTF8.GetBytes(text);

            byte[] baSalt = GetRandomBytes();
            byte[] baEncrypted = new byte[baSalt.Length + baText.Length];

            // Combine Salt + Text
            for (int i = 0; i < baSalt.Length; i++)
                baEncrypted[i] = baSalt[i];
            for (int i = 0; i < baText.Length; i++)
                baEncrypted[i + baSalt.Length] = baText[i];

            baEncrypted = AES_Encrypt(baEncrypted, baPwdHash);

            string result = Convert.ToBase64String(baEncrypted);
            return result;
        }

        public string DecryptString(string text, string password)
        {
            byte[] baPwd = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            byte[] baPwdHash = SHA256Managed.Create().ComputeHash(baPwd);

            byte[] baText = Convert.FromBase64String(text);

            byte[] baDecrypted = AES_Decrypt(baText, baPwdHash);

            // Remove salt
            int saltLength = GetSaltLength();
            byte[] baResult = new byte[baDecrypted.Length - saltLength];
            for (int i = 0; i < baResult.Length; i++)
                baResult[i] = baDecrypted[i + saltLength];

            string result = Encoding.UTF8.GetString(baResult);
            return result;
        }

        public void EncryptFile(string file, string password)
        {
            //string file = "C:\\SampleFile.DLL";
            //string password = "abcd1234";

            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string fileEncrypted = string.Format("{0}_encrypted",file);

            File.WriteAllBytes(fileEncrypted, bytesEncrypted);
        }

        public void DecryptFile(string fileEncrypted, string password)
        {
            //string fileEncrypted = "C:\\SampleFileEncrypted.DLL";
            //string password = "abcd1234";

            byte[] bytesToBeDecrypted = File.ReadAllBytes(fileEncrypted);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string file = string.Format("{0}_decrypted", fileEncrypted);
            File.WriteAllBytes(file, bytesDecrypted);
        }

        private byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = _saltBytes;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }

        private byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = _saltBytes;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }
            return decryptedBytes;
        }

        private byte[] GetRandomBytes()
        {
            int saltLength = GetSaltLength();
            byte[] ba = new byte[saltLength];
            RNGCryptoServiceProvider.Create().GetBytes(ba);
            return ba;
        }

        private int GetSaltLength()
        {
            return _saltBytes.Length;
        }
    }
}
