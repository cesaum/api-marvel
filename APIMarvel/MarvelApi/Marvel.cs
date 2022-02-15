using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;


namespace APIMarvel.MarvelApi
{

    public class Marvel
    {
        private const string BASE_URL = "http://gateway.marvel.com/v1/public";
        private readonly string _publicKey = "2f823b13a92cf8d95f6ee0c3e1e7f966";
        private readonly string _privateKey = "0a81825ee89ceab9e7024e8be7dd55ce13165a5c";
        private static HttpClient _client = new HttpClient();


        private string CreateHash(string input)
        {
            var hash = String.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                hash = sBuilder.ToString();
            }
            return hash;

        }

        public async Task<CharacterDataWrapper> GetCharacters()
        {
            //we need a timestamp
            string timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            //we need use a hash to call the marvel api
            string s = String.Format("{0}{1}{2}", timestamp, _privateKey, _publicKey);

            string hash = CreateHash(s);
            //format the url string  with search critieria          
            string requestURL = String.Format(BASE_URL + "/characters?ts={0}&apikey={1}&hash={2}", timestamp, _publicKey, hash);

            var url = new Uri(requestURL);

            var response = await _client.GetAsync(url);

            string json;
            using (var content = response.Content)
            {
                json = await content.ReadAsStringAsync();
            }

            CharacterDataWrapper cdw = JsonConvert.DeserializeObject<CharacterDataWrapper>(json);

            StreamWriter sw = new StreamWriter("../personagensmarvel.txt");
            //Write a line of text
            sw.WriteLine(JsonConvert.SerializeObject(cdw.Data.Results));
            //Write a second line of text
            sw.WriteLine("From the StreamWriter class");
            //Close the file
            sw.Close();

            return cdw;

        }


        
    }
    public class MarvelError
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }

    public class MarvelUrl
    {
        public string Type { get; set; }
        public string Url { get; set; }
    }

    public class MarvelImage
    {
        public string Path { get; set; }
        public string Extension { get; set; }
        public override string ToString()
        {
            return string.Format("{0}.{1}", Path, Extension);
        }
        public string ToString(Image size)
        {
            return string.Format("{0}{1}.{2}", Path, size.ToParameter(), Extension);
        }
    }
    public class TextObject
    {
        public string Type { get; set; }
        public string Language { get; set; }
        public string Text { get; set; }
    }

}
