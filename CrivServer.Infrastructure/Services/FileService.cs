using CrivServer.Infrastructure.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrivServer.Infrastructure.Services
{
    public class FileService:IFileService
    {
        private static IHostingEnvironment _env;
        private static IConfiguration _config;
        private static IEncryptor _encryptor;
        private static IDataProtector _protector;
        public FileService(IConfiguration config, IEncryptor encryptor, IHostingEnvironment env) { _env = env; _config = config; _encryptor = encryptor; }
        public FileService(IConfiguration config, IDataProtectionProvider provider, IHostingEnvironment env) { _env = env; _config = config; _protector = provider.CreateProtector(_config.GetValue<string>("Protector")); }

        /// <summary>
        /// Unsafe ReadFile for use with initial encryption
        /// </summary>
        public string ReadFile(string filename)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), filename);
            var filelines = File.ReadAllLines(path);
            return filelines.ConvertStringArrayToString();
        }

        /// <summary>
        /// Unsafe overwrite for use with initial encryption
        /// </summary>
        public Task OverwriteFile(string filename, string content)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), filename);
            File.Delete(path);
            File.AppendAllText(path, content);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Save a cert for use with encryption
        /// </summary>
        public Task SaveCertificate(string certname, string password, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), certname);
            File.WriteAllBytes(path, certificate.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Pfx, password));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Save a byte array to the filename.filetype at PublicDirectory+location
        /// note: ensure filtered inputs as unsafe.
        /// </summary>
        public async Task SaveFile(string location, string filename, string filetype, byte[] toSave)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.GetData("PublicDirectory").ToString(), location, filename + "." + filetype).ToString();
            await File.WriteAllBytesAsync(path, toSave);            
        }

        public async Task SaveFiles(List<IFormFile> files, string folderName)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // ToDo: safely save to a non temporary location
            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
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
                _protector = null;
            }
            // free native resources if there are any.
        }
    }
}
