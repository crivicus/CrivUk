using CrivServer.Infrastructure.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CrivServer.Infrastructure.Extensions
{
    public static class DataProtectionExtension
    {
        public static IServiceCollection ConfigureDataProtectionService(this IServiceCollection services, IConfiguration config, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                var keyDir = new DirectoryInfo(Path.Combine(env.ContentRootPath, @"\App_Data\Keys\").ToString());
                services.AddDataProtection()
                    .UseCryptographicAlgorithms(
                        new AuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                        })
                        .PersistKeysToFileSystem(keyDir);
            }
            else
            {
                var mainCertificate = config.GetSection("PrimaryCertificate");
                var secondaryCertificate = config.GetSection("BackupCertificate");

                services.AddDataProtection()
                    .UseCryptographicAlgorithms(
                        new AuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                        })
                    .PersistKeysToFileSystem(
                        new DirectoryInfo(@"\\server\share\directory\"))
                    .ProtectKeysWithCertificate(
                        new X509Certificate2(mainCertificate.GetValue<string>("location"), mainCertificate.GetValue<string>("password")))
                    .UnprotectKeysWithAnyCertificate(
                        new X509Certificate2(secondaryCertificate.GetValue<string>("location"), secondaryCertificate.GetValue<string>("password")));
            }
            return services;
        }

        public static IConfiguration DecryptConfiguration(this IConfiguration config, IEncryptor encryptor)
        {
            config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value = encryptor.Decrypt(config.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection"));
            config.GetSection("PrimaryCertificate").GetSection("password").Value = encryptor.Decrypt(config.GetSection("PrimaryCertificate").GetValue<string>("password"));            
            config.GetSection("PrimaryCertificate").GetSection("location").Value = encryptor.Decrypt(config.GetSection("PrimaryCertificate").GetValue<string>("location"));
            config.GetSection("BackupCertificate").GetSection("password").Value = encryptor.Decrypt(config.GetSection("BackupCertificate").GetValue<string>("password"));
            config.GetSection("BackupCertificate").GetSection("location").Value = encryptor.Decrypt(config.GetSection("BackupCertificate").GetValue<string>("location"));

            return config;
        }

        public static IConfiguration EncryptConfiguration(this IConfiguration config, IEncryptor encryptor)
        {
            config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value = encryptor.Encrypt(config.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection"));
            config.GetSection("PrimaryCertificate").GetSection("password").Value = encryptor.Encrypt(config.GetSection("PrimaryCertificate").GetValue<string>("password"));
            config.GetSection("PrimaryCertificate").GetSection("location").Value = encryptor.Encrypt(config.GetSection("PrimaryCertificate").GetValue<string>("location"));
            config.GetSection("BackupCertificate").GetSection("password").Value = encryptor.Encrypt(config.GetSection("BackupCertificate").GetValue<string>("password"));
            config.GetSection("BackupCertificate").GetSection("location").Value = encryptor.Encrypt(config.GetSection("BackupCertificate").GetValue<string>("location"));

            return config;
        }
    }
}
