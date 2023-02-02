using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPT_TelegramBot.Model;

namespace VPT_TelegramBot.Controller
{
    public class HttpUser
    {
        
        public static User GetUserAtId(string id, string secretKey)
        {
            string json = JsonConvert.SerializeObject(new User() { UserId = id });
            return JsonConvert.DeserializeObject<User>(HttpUser.GetUserAtJson(json, secretKey).Result);
        }

        public static User UpdateUserInfoAtId(User user, string secretKey)
        {
            user.Role = user.Role == null || user.Role.ToString().Trim() == "" ? "Student" : user.Role.ToString();
            string json = JsonConvert.SerializeObject(user);
            return JsonConvert.DeserializeObject<User>(HttpUser.GetUserAtJson(json, secretKey).Result);
        }

        static async Task<string> GetUserAtJson(string jsons, string secretKey)
        {

            HttpClient client = new();

            var value = new Dictionary<string, string>()
            {
                {"secretKey", secretKey},
                {"user", jsons }
            };

            var content = new FormUrlEncodedContent(value);
            var response = await client.PostAsync("http://f0771572.xsph.ru/API/GetUserChat.php", content);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString.ToString();

        }
    }
}
