using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPT_TelegramBot.Controller
{
    public class HttpDatabase
    {
        public async static Task<string> Push(string jsons, string secretKey)
        {
            Console.Clear();
            Console.WriteLine("Wait...");
            HttpClient client = new();

            var value = new Dictionary<string, string>()
            {
                {"secretKey", secretKey},
                {"schedules", jsons }
            };

            var content = new FormUrlEncodedContent(value);
            var response = await client.PostAsync("http://f0771572.xsph.ru/API/AddSchedule.php", content);
            var responseString = await response.Content.ReadAsStringAsync();
            Console.Clear();
            Console.WriteLine("Complited");
            return responseString.ToString();

        }


        public async static Task<string> GetLastSevenDaySchedule()
        {
            Console.Clear();
            Console.WriteLine("Wait...");
            HttpClient httpClient = new();
            var response = await httpClient.GetStringAsync("http://f0771572.xsph.ru/API/GetLastSevenDaySchedules.php");
            Console.Clear();
            Console.WriteLine("Complited");
            return response.ToString();
        }
    }
}
