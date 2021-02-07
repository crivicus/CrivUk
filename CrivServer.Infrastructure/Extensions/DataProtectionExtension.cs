using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace CrivServer.Infrastructure.Extensions
{
    public static class DataProtectionExtension
    {
        public static IServiceCollection ConfigureDataProtectionService(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
        {
            var mainCertificate = config.GetSection("PrimaryCertificate");
            var secondaryCertificate = config.GetSection("BackupCertificate");

            if (env.IsDevelopment())
            {
                var keyDir = new DirectoryInfo(Path.Combine(env.ContentRootPath, "App_Data","Keys").ToString());
                services.AddDataProtection()
                    .UseCryptographicAlgorithms(
                        new AuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                        })
                        .PersistKeysToFileSystem(keyDir)
                        .ProtectKeysWithCertificate(
                            new X509Certificate2(mainCertificate.GetValue<string>("location"), mainCertificate.GetValue<string>("password")))
                        .UnprotectKeysWithAnyCertificate(
                            new X509Certificate2(secondaryCertificate.GetValue<string>("location"), secondaryCertificate.GetValue<string>("password")));
            }
            else
            {              
                services.AddDataProtection()
                    .UseCryptographicAlgorithms(
                        new AuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                        })
                    .PersistKeysToFileSystem(
                        new DirectoryInfo(Path.Combine(env.ContentRootPath, "App_Data", "Keys").ToString()))
                    .ProtectKeysWithCertificate(
                        new X509Certificate2(mainCertificate.GetValue<string>("location"), mainCertificate.GetValue<string>("password")))
                    .UnprotectKeysWithAnyCertificate(
                        new X509Certificate2(secondaryCertificate.GetValue<string>("location"), secondaryCertificate.GetValue<string>("password")));
            }
            return services;
        }               
    }
}
