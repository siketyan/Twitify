using Newtonsoft.Json;
using System.IO;

namespace Twitify.Objects
{
    public class Credentials
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("access_token_secret")]
        public string AccessTokenSecret { get; set; }


        [JsonIgnore]
        private readonly string _path;


        public Credentials(string path)
        {
            _path = path;
        }

        public static Credentials Open(string path)
        {
            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<Credentials>(json);
            }
            catch (FileNotFoundException)
            {
                return new Credentials(path);
            }
        }
        
        public void Save()
        {
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText(_path, json);
        }
    }
}