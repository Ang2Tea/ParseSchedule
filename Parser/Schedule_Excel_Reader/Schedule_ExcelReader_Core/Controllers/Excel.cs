using ExcelDataReader;
using Schedule_ExcelReader_Core.Enums;
using Schedule_ExcelReader_Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_ExcelReader_Core.Controllers;
public class Excel
{
    private readonly string fileDirectory;
    private DataTable dataTable;
    private bool checkRead;
    private Dictionary<ScheduleExcelColumn, int> scheduleExcelColumns;

    public Excel(string fileDirectory)
    {
        if (fileDirectory is null || fileDirectory == "") throw new Exception("Empty filename");

        this.fileDirectory = fileDirectory;
        checkRead = false;
    }

    private string GetData(int i, ScheduleExcelColumn exelColumn)
    {
        return dataTable.Rows[i].ItemArray[scheduleExcelColumns[exelColumn]].ToString();
    }
    private Dictionary<ScheduleExcelColumn, int> GetScheduleColumn()
    {
        Dictionary<ScheduleExcelColumn, int> result = new();

        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
            if (dataTable.Rows[i++].ItemArray[0].ToString().ToLower() != "учебная группа") continue;

            for (int j = 0; j < dataTable.Rows[i].ItemArray.Length; j++)
            {
                string element = dataTable.Rows[i].ItemArray[j].ToString();
                if (element is null || element.Trim() == "") continue;
                
                ScheduleExcelColumn addItem = element.ToLower() switch
                {
                    "дисциплина" => ScheduleExcelColumn.Discipline,
                    "преподаватель" => ScheduleExcelColumn.Teacher,
                    "аудитория" => ScheduleExcelColumn.Auditorium,
                    _ => ScheduleExcelColumn.None // Без этого предуприжения даются если с ним буде тне работать убери
                };
                if (element.ToLower() == "номер пары")
                {
                    result.Add(ScheduleExcelColumn.NumCouple, j);
                    result.Add(ScheduleExcelColumn.StudyGroup, j);
                    continue;
                }

                result.Add(addItem, j);
            }
            break;
        }

        return result;
    }

    public void ReadExel()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        FileInfo file = new(fileDirectory);

        if (!file.Exists && !file.Name.EndsWith(".xls") && !file.Name.EndsWith(".xlsx")) throw new Exception("File does not exist");

        using FileStream stream = File.Open(fileDirectory, FileMode.Open, FileAccess.Read);
        using var edr = ExcelReaderFactory.CreateReader(stream);

        dataTable = edr
            .AsDataSet()
            .Tables[0]
            .AsDataView()
            .Table;

        scheduleExcelColumns = GetScheduleColumn();

        checkRead = true;
    }
    public Schedule ToSchedule()
    {
        if (!checkRead) throw new Exception("You dont read file");

        string dateStr = dataTable.Rows[0].ItemArray[0].ToString();

        if (dateStr.Trim() == "") return null;

        Schedule schedule = new()
        {
            Date = Date.ExtractFromString(dateStr)
        };

        DataRowCollection dataRowCollection = dataTable.Rows;

        List<Group> group = new();

        for (int i = 2; i < dataRowCollection.Count; i++)
        {
            if (dataRowCollection[i].ItemArray[0].ToString().ToLower() == "номер пары") continue;
            var groupName = GetData(i, ScheduleExcelColumn.StudyGroup).Split("с ")[0];
            if (groupName.Length > 1)
            {
                group.Add(new Group() { NameGroup = groupName, Couples = new List<Couple>() });
                continue;
            }

            if (!int.TryParse(GetData(i, ScheduleExcelColumn.NumCouple), out int numCouple)) continue;

            if(GetData(i, ScheduleExcelColumn.Teacher).Split('/').Length > 1 ||
                GetData(i, ScheduleExcelColumn.Discipline).Split('/').Length > 1 ||
                GetData(i, ScheduleExcelColumn.Auditorium).Split('/').Length > 1)
                for (int j = 0; j < GetData(i, ScheduleExcelColumn.Teacher).Split('/').Length; j++)
                {
                    var auditorium = GetData(i, ScheduleExcelColumn.Auditorium).Split('/');
                    var discipline = GetData(i, ScheduleExcelColumn.Discipline).Split('/');
                    var teacher = GetData(i, ScheduleExcelColumn.Teacher).Split('/');
                    group[^1].Couples.Add(new()
                    {
                        Discipline = discipline.Length > 1 ? discipline[discipline.Length > j ? j : discipline.Length-1].Trim() : discipline[0],
                        Auditorium = auditorium.Length > 1 ? auditorium[auditorium.Length > j ? j : auditorium.Length - 1].Trim() : auditorium[0].Trim(),
                        Teacher = teacher.Length > 1 ? teacher[teacher.Length > j ? j : teacher.Length - 1].Trim() : teacher[0].Trim(),
                        NumCouple = numCouple
                    });
                }
            else
                group[^1].Couples.Add(new()
                {
                    Discipline = GetData(i, ScheduleExcelColumn.Discipline).Trim(),
                    Auditorium = GetData(i, ScheduleExcelColumn.Auditorium).Trim(),
                    Teacher = GetData(i, ScheduleExcelColumn.Teacher).Trim(),
                    NumCouple = numCouple
                });

        }
        schedule.Groups = group;

        return schedule;
    }
}
