using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPT_TelegramBot.Controller;

namespace New_VPT_TelegramBot.Controller
{
    public class HttpDateController
    {
        public static void ReadAndPush(string path,string secretKey)
        {
            var schedules = SearchExcel.ReadAllExcelAtDirectory(path);

            var counter = 0;
            var b = schedules.GroupBy(_ => counter++ / 10).Select(v => v.ToList()).ToList();

            int count = 0;
            var temp = "";
            while (count < b.Count() - 1)
            {
                string message = "";
                if (temp != "work")
                {
                    message = HttpDatabase.Push(JsonConvert.SerializeObject(b[count]), secretKey).Result;
                    temp = "work";
                }
                if (message != "" && temp == "work")
                {
                    temp = "";
                    count++;
                    Console.WriteLine($"complite: {count}");
                }

            }

        }
    }
}
