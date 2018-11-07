using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrivServer.Infrastructure.Services
{
    public interface IFileService : IDisposable
    {
        string ReadFile(string filename);
        Task OverwriteFile(string filename, string content);
        Task SaveFiles(List<IFormFile> files, string folderName);
    }
}
