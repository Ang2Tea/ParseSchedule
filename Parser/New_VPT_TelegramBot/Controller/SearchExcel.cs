using Schedule_ExcelReader_Core.Models;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schedule_ExcelReader_Core.Controllers;

namespace New_VPT_TelegramBot.Controller
{   
    public static class SearchExcel
    {
        public static List<Schedule> ReadAllExcelAtDirectory(string mainDirectory)
        {
            var files = Directory.EnumerateFiles(mainDirectory, "*.*", SearchOption.AllDirectories).Where(p => p.EndsWith(".xlsx") || p.EndsWith(".xls")).ToList();
            
            List<Schedule> schedules = new List<Schedule>();
            files.ForEach(p =>
            {
                Excel excel = new(p);
                excel.ReadExel();
                var schedule = excel.ToSchedule();
                if (schedule is not null) schedules.Add(schedule);

            });
            return schedules;
        }
    }
}
