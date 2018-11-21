using CrivServer.Data.Contexts;
using CrivServer.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrivServer.AI.Memory
{
    public class DatabaseMemory : AIBrain, IDisposable
    {

        public DatabaseMemory() { }
        public DatabaseMemory(IConfiguration configuration, CrivDbContext context, ILoggerFactory logger) : base(configuration, context, logger) { }

        public async Task<List<string>> FindRelatedStringAsync(string input)
        {
            var related = new List<string>();
            related = await Task.Run(() => FindRelatedContentAsync(input).Result.Select(x=>x.url).ToList());
            return related;
        }

        public async Task<List<DbContentModel>> FindRelatedContentAsync(string input)
        {
            var related = new List<DbContentModel>();
            related = await Task.Run(()=> _crivContext.ContentModels.Where(x=>CheckRelated(x, input)).ToList());
            return related;
        }

        public bool CheckRelated(DbContentModel x, string input)
        {
            if (x.url.Contains(input) || x.content_h1.Contains(input) || x.tab_title.Contains(input) || x.page_title.Contains(input))
                return true;
            else
                return false;
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
