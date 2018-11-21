using CrivServer.Data.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CrivServer.AI.Memory
{
    public class FileMemory : AIBrain, IDisposable
    {
        private string _AIDir;

        public FileMemory() { _AIDir = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(),"AI"); }

        public FileMemory(IConfiguration configuration, CrivDbContext context, ILoggerFactory logger) : base(configuration, context, logger)
        {
            _AIDir = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "AI");
        }

        public async Task<List<string>> FindInFilesAsync(string input)
        {
            var path = Path.Combine(_AIDir, input + ".txt");
            try
            {
                return new List<string>(await File.ReadAllLinesAsync(path));
            }
            catch { }
            return new List<string>();
        }

        #region Dispose elements
        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual new void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                base.Dispose();
            }
            // free native resources if there are any.
        }
        #endregion
    }
}
