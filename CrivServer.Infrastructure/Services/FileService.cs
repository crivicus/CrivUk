using CrivServer.Infrastructure.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CrivServer.Infrastructure.Services
{
    public class FileService:IFileService
    {
        private static IHostingEnvironment _env;
        private static IConfiguration _config;
        private static IEncryptor _encryptor;
        public FileService(IConfiguration config, IEncryptor encryptor, IHostingEnvironment env) { _env = env; _config = config; _encryptor = encryptor; }

        public string ReadFile(string filename)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), filename);
            var filelines = File.ReadAllLines(path);
            return filelines.ConvertStringArrayToString();
        }

        public Task OverwriteFile(string filename, string content)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), filename);
            File.Delete(path);
            File.AppendAllText(path, content);
            return Task.CompletedTask;
        }

        public Task SaveCertificate(string certname, string password, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), certname);
            File.WriteAllBytes(path, certificate.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Pfx, password));
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                _env = null;
                _config = null;
                _encryptor = null;

            }
            // free native resources if there are any.
        }
    }
}
