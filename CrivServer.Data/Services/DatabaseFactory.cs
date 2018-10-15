using CrivServer.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
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
