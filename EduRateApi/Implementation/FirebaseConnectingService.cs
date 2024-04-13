using EduRateApi.Interfaces;
using FireSharp;
using FireSharp.Config;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EduRateApi.Implementation
{
    public class FirebaseConnectingService : IFirebaseConnectingService
    {
        public string GetApiKey()
        {
            string configFile = "Config/firebaseConfig.json";
            string configJson = File.ReadAllText(configFile);
            var config = JObject.Parse(configJson);
            return config["API_KEY"].ToString();
        }

        public FirebaseClient GetFirebaseClient()
        {
            var firebaseConfigPath = "Config/firebaseConfig.json";
            var configJson = System.IO.File.ReadAllText(firebaseConfigPath);
            var config = JsonConvert.DeserializeObject<FirebaseConfig>(configJson);

            return new FireSharp.FirebaseClient(config);
        }
    }
}
