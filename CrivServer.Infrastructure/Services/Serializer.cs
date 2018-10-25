using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrivServer.Infrastructure.Services
{
    public class Serializer : ISerializer
    {
        private static Newtonsoft.Json.JsonSerializer _json;
        private static Newtonsoft.Json.JsonSerializer Json
        {
            get
            {
                if (_json == null) _json = new Newtonsoft.Json.JsonSerializer();
                return _json;
            }
        }

        public string ContentType { get { return "application/json"; } }

        public string Serialize(object content)
        {
            return JsonConvert.SerializeObject(content,Formatting.Indented);
        }

        public TOutput Deserialize<TOutput>(string content)
        {
            return JsonConvert.DeserializeObject<TOutput>(content);
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
                _json = null;
            }
            // free native resources if there are any.
        }
    }
}
