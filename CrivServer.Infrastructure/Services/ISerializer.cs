﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CrivServer.Infrastructure.Services
{
    public interface ISerializer
    {
        string ContentType { get; }
        string Serialize(object content);
        TOutput Deserialize<TOutput>(string content);
    }
}
