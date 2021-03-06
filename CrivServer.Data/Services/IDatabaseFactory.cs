﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrivServer.Data.Services
{
    public interface IDatabaseFactory
    {
        Task ConnectToDatabase(string connectionString);
    }
}
