using CrivServer.Data.Models;
using CrivServer.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CrivServer.Infrastructure.Extensions
{
    public static class ConfigurationInjectionExtension
    {
        public static IServiceCollection ConfigurationInjectionService(this IServiceCollection services, IConfiguration config, IHostingEnvironment env, IEncryptor manualEncryptor)
        {
            if (config.GetSection("InitialConfig").Value.Equals("true"))
            {
                config.InitialEncrypt(manualEncryptor, env);
            }
            else
            {
                config.DecryptConfiguration(env, manualEncryptor);
            }
            services.AddSingleton(config);

            return services;
        }

        public static IConfiguration DecryptConfiguration(this IConfiguration config, IHostingEnvironment env, IEncryptor encryptor)
        {
            config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value = encryptor.Decrypt(config.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection"));
            config.GetSection("PrimaryCertificate").GetSection("password").Value = encryptor.Decrypt(config.GetSection("PrimaryCertificate").GetValue<string>("password"));
            config.GetSection("PrimaryCertificate").GetSection("location").Value = encryptor.Decrypt(config.GetSection("PrimaryCertificate").GetValue<string>("location"));
            config.GetSection("BackupCertificate").GetSection("password").Value = encryptor.Decrypt(config.GetSection("BackupCertificate").GetValue<string>("password"));
            config.GetSection("BackupCertificate").GetSection("location").Value = encryptor.Decrypt(config.GetSection("BackupCertificate").GetValue<string>("location"));
            if (env.IsDevelopment())
            {
                config.GetSection("TestUser").GetSection("UserName").Value = encryptor.Decrypt(config.GetSection("TestUser").GetValue<string>("UserName"));
                config.GetSection("TestUser").GetSection("NormalizedUserName").Value = encryptor.Decrypt(config.GetSection("TestUser").GetValue<string>("NormalizedUserName"));
                config.GetSection("TestUser").GetSection("Email").Value = encryptor.Decrypt(config.GetSection("TestUser").GetValue<string>("Email"));
                config.GetSection("TestUser").GetSection("NormalizedEmail").Value = encryptor.Decrypt(config.GetSection("TestUser").GetValue<string>("NormalizedEmail"));
                config.GetSection("TestUser").GetSection("PasswordHash").Value = encryptor.Decrypt(config.GetSection("TestUser").GetValue<string>("PasswordHash"));
                config.GetSection("TestUser").GetSection("PhoneNumber").Value = encryptor.Decrypt(config.GetSection("TestUser").GetValue<string>("PhoneNumber"));
            }
            return config;
        }

        public static IConfiguration EncryptConfiguration(this IConfiguration config, IHostingEnvironment env, IEncryptor encryptor)
        {
            config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value = encryptor.Encrypt(config.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection"));
            config.GetSection("PrimaryCertificate").GetSection("password").Value = encryptor.Encrypt(config.GetSection("PrimaryCertificate").GetValue<string>("password"));
            config.GetSection("PrimaryCertificate").GetSection("location").Value = encryptor.Encrypt(config.GetSection("PrimaryCertificate").GetValue<string>("location"));
            config.GetSection("BackupCertificate").GetSection("password").Value = encryptor.Encrypt(config.GetSection("BackupCertificate").GetValue<string>("password"));
            config.GetSection("BackupCertificate").GetSection("location").Value = encryptor.Encrypt(config.GetSection("BackupCertificate").GetValue<string>("location"));

            return config;
        }

        public static IConfiguration InitialEncrypt(this IConfiguration config, IEncryptor encryptor, IHostingEnvironment env)
        {
            using (var fileprovider = new FileService(config, encryptor, env))
            {
                using (var json = new Serializer())
                {
                    var strAppSettings = fileprovider.ReadFile("appsettings-custom.json");
                    var appsettings = new CustomAppSettingsModel(strAppSettings);
                    var encryptedSettings = new CustomAppSettingsModel();

                    encryptedSettings.ConnectionStrings = new Dictionary<string, string>();
                    foreach (var conn in appsettings.ConnectionStrings)
                    {
                        encryptedSettings.ConnectionStrings.Add(conn.Key, encryptor.Encrypt(conn.Value));
                    }
                    encryptedSettings.PrimaryCertificate = new Certifiate()
                    {
                        location = encryptor.Encrypt(appsettings.PrimaryCertificate.location),
                        password = encryptor.Encrypt(appsettings.PrimaryCertificate.password)
                    };
                    encryptedSettings.BackupCertificate = new Certifiate()
                    {
                        location = encryptor.Encrypt(appsettings.BackupCertificate.location),
                        password = encryptor.Encrypt(appsettings.BackupCertificate.password)
                    };
                    encryptedSettings.TestUser = new ApplicationUser()
                    {
                        UserName = encryptor.Encrypt(appsettings.TestUser.UserName),
                        Email = encryptor.Encrypt(appsettings.TestUser.Email),
                        NormalizedUserName = encryptor.Encrypt(appsettings.TestUser.NormalizedUserName),
                        NormalizedEmail = encryptor.Encrypt(appsettings.TestUser.NormalizedEmail),
                        PhoneNumber = encryptor.Encrypt(appsettings.TestUser.PhoneNumber),
                        PasswordHash = encryptor.Encrypt(appsettings.TestUser.PasswordHash),
                        UserType = appsettings.TestUser.UserType
                    };
                    encryptedSettings.Protector = appsettings.Protector;
                    encryptedSettings.InitialConfig = "false";
                    fileprovider.OverwriteFile("appsettings-custom.json", json.Serialize(encryptedSettings));
                    fileprovider.SaveCertificate(appsettings.PrimaryCertificate.location, appsettings.PrimaryCertificate.password, encryptor.GenerateCertificate(appsettings.PrimaryCertificate.location, appsettings.PrimaryCertificate.password));
                    fileprovider.SaveCertificate(appsettings.BackupCertificate.location, appsettings.BackupCertificate.password, encryptor.GenerateCertificate(appsettings.BackupCertificate.location, appsettings.BackupCertificate.password));
                }
                return config;
            }
        }
    }
}
