using New_VPT_TelegramBot.Controller;
using Newtonsoft.Json;
using Schedule_ExcelReader_Core.Controllers;
using Schedule_ExcelReader_Core.Models;
using System.Net;
using VPT_TelegramBot.Controller;
using VPT_TelegramBot.Model;


//var schedulesa = GetLastSevenDaySchedule();
var user = JsonConvert.SerializeObject(new User() { UserId = "4234642572452342", UserName = "RoarOfHell" });
var key = "flgopr9ErFgI2P5019fkdyenxcga93po";

//var users = HttpUser.GetUserAtId("qweqweqwe132", key);

//BotTelegram bot = new BotTelegram(key, "5026266968:AAFusmu5MnMokdifr_ZO1Nad5HLHbQ4Gnqg");
//bot.Start();
//Console.ReadLine();

GoogleParsing googleParsing = new GoogleParsing("1vzKOEmF84_dr8PUXSKc9z3IkWG6-dmQq");
var files2 = googleParsing.GetDriveFiles();
var files = googleParsing.GetContainsInFolder("1vzKOEmF84_dr8PUXSKc9z3IkWG6-dmQq"); // 1vzKOEmF84_dr8PUXSKc9z3IkWG6-dmQq - Id корневой папки с расписанием (требуется добавить в избранное на основном аккаунте)
googleParsing.DownloadAllScheduleAtDisk(files);

Console.WriteLine("Complited!");
Console.ReadLine();
//@"D:\VPT_ScheduleTesting\Schedule" - path



List<Schedule> GetLastSevenDaySchedule()
{
    List<Schedule>? schedules = JsonConvert.DeserializeObject<List<Schedule>>(HttpDatabase.GetLastSevenDaySchedule().Result);

    return schedules != null ? schedules : new List<Schedule>();

}


//5026266968:AAFusmu5MnMokdifr_ZO1Nad5HLHbQ4Gnqg