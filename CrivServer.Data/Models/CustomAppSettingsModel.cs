using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrivServer.Data.Models
{
    public class CustomAppSettingsModel
    {
        public string InitialConfig { get; set; }
        public string Protector { get; set; }
        public Dictionary<string,string> ConnectionStrings { get; set; }
        public ApplicationUser TestUser { get; set; }
        public Certifiate PrimaryCertificate { get; set; }
        public Certifiate BackupCertificate { get; set; }

        public CustomAppSettingsModel() { }
        public CustomAppSettingsModel(string json)
        {
            JObject appSettings = JObject.Parse(json);
            JToken connStrings = appSettings["ConnectionStrings"];
            JToken jUser = appSettings["TestUser"];
            JToken pCert = appSettings["PrimaryCertificate"];
            JToken sCert = appSettings["BackupCertificate"];
            InitialConfig = (string)appSettings["InitialConfig"];
            Protector = (string)appSettings["Protector"];
            ConnectionStrings = new Dictionary<string, string>();
            ConnectionStrings.Add("DefaultConnection", (string)connStrings["DefaultConnection"]);
            PrimaryCertificate = new Certifiate() { location = (string)pCert["location"], password = (string)pCert["password"] };
            BackupCertificate = new Certifiate() { location = (string)sCert["location"], password = (string)sCert["password"] };
            TestUser = new ApplicationUser() {
                UserName = (string)jUser["UserName"],
                Email = (string)jUser["Email"],
                NormalizedUserName = (string)jUser["NormalizedUserName"],
                NormalizedEmail = (string)jUser["NormalizedEmail"],
                PhoneNumber = (string)jUser["PhoneNumber"],
                PasswordHash = (string)jUser["PasswordHash"],
                UserType = (int)jUser["UserType"]
            };
        }
    }

    public class Certifiate
    {
        public string location { get; set; }
        public string password { get; set; }
    }
}