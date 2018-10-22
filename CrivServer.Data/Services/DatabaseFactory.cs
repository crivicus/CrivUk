using System.Threading.Tasks;

namespace CrivServer.Data.Services
{
    public class DatabaseFactory: IDatabaseFactory
    {        
        public DatabaseFactory()
        {

        }
        public Task ConnectToDatabase(string connectionString)
        {
            
            return Task.CompletedTask;
        }
    }
}
