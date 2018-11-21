using CrivServer.Data.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrivServer.AI
{
    public abstract class AIBrain : IDisposable
    {
        protected IConfiguration _crivConfig;
        protected CrivDbContext _crivContext;
        protected ILogger _logger;
        protected int SiteId;

        public AIBrain() { }

        public AIBrain(IConfiguration configuration, CrivDbContext context, ILoggerFactory logger)
        {
            _crivConfig = configuration;
            _crivContext = context;
            _logger = logger.CreateLogger<AIBrain>();
            SiteId = 1; //ToDo: configure dynamic site id
        }

        #region Dispose elements
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
                _crivConfig = null;
                _crivContext = null;
                _logger = null;
                SiteId = 0;
            }
            // free native resources if there are any.
        }
        #endregion
    }
}
