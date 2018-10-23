using CrivServer.Data.Contexts;
using System.Threading.Tasks;

namespace CrivServer.Data.Services
{
    public class DatabaseFactory: IDatabaseFactory
    {
        private readonly CrivDbContext _crivContext;

        public DatabaseFactory(CrivDbContext crivContext)
        {
            _crivContext = crivContext;
        }
        public Task ConnectToDatabase(string connectionString)
        {
            
            return Task.CompletedTask;
        }
    }
}
