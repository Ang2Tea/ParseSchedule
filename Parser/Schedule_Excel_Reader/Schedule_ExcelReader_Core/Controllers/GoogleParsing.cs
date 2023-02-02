using ExcelDataReader;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Schedule_ExcelReader_Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

namespace Schedule_ExcelReader_Core.Controllers
{
    public class GoogleParsing
    {
        private string id;
        private readonly string secretClient = "Secret/client_secret.json";
        private readonly string driveServiceCredentials = "Secret/DriveServiceCredentials.json";
        public GoogleParsing(string id)
        {
            this.id = id;
            if (!File.Exists(secretClient))
            {
                Console.WriteLine($"Error: file not found on the path: {Path.GetFullPath(secretClient)}");
                throw new Exception("Error: Client Secret not valid!");
            }
            
        }

        private static string[] Scopes = { Google.Apis.Drive.v3.DriveService.Scope.Drive };

        private Google.Apis.Drive.v3.DriveService GetService()
        {
            UserCredential credential;


            using (var stream = new FileStream(System.IO.Path.Combine(secretClient), FileMode.Open, FileAccess.Read))
            {

                String FilePath = System.IO.Path.Combine(driveServiceCredentials);

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }

            
            return new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveRestAPI-v3"
            });
        }

        private Google.Apis.Drive.v2.DriveService GetService_v2()
        {
            UserCredential credential;


            using (var stream = new FileStream(System.IO.Path.Combine(secretClient), FileMode.Open, FileAccess.Read))
            {

                String FilePath = System.IO.Path.Combine(driveServiceCredentials);

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }

            
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveRestAPI-v2",
            });
        }

        public List<GoogleDriveFile> GetDriveFiles()
        {
            
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = GetService().Files.List();
            FileListRequest.Fields = "nextPageToken, files(*)";

            //IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFile> FileList = new();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    GoogleDriveFile File = new GoogleDriveFile
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        Version = file.Version,
                        CreatedTime = file.CreatedTime,
                        Parents = file.Parents,
                        MimeType = file.MimeType
                    };
                    FileList.Add(File);
                }
            }
            return FileList;
        }
        public List<GoogleDriveFile> GetDriveFilesAtId()
        {
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = GetService().Files.List();
            

            FileListRequest.Fields = "nextPageToken, files(*)";

            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFile> FileList = new();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Id == id)
                    {
                        GoogleDriveFile File = new GoogleDriveFile
                        {
                            Id = file.Id,
                            Name = file.Name,
                            Size = file.Size,
                            Version = file.Version,
                            CreatedTime = file.CreatedTime,
                            Parents = file.Parents,
                            MimeType = file.MimeType
                        };
                        FileList.Add(File);

                    }
                   
                }
            }
            return FileList;
        }
        public Dictionary<string, string> GetContainsInFolder(string folderId)
        {

            List<string> ChildList = new List<string>();
            Google.Apis.Drive.v2.DriveService ServiceV2 = GetService_v2();
            ChildrenResource.ListRequest ChildrenIDsRequest = ServiceV2.Children.List(folderId);
            List<GoogleDriveFile> Filter_FileList = new List<GoogleDriveFile>();

            if (IsContentBlackList(ServiceV2.Files.Get(folderId).Execute().Title))
            {
                return null;
            }

            do
            {
                var children = ChildrenIDsRequest.Execute();

                if (children.Items != null && children.Items.Count > 0)
                {
                    foreach (var file in children.Items)
                    {
                        ChildList.Add(file.Id);

                    }
                }
                ChildrenIDsRequest.PageToken = children.NextPageToken;

            } while (!String.IsNullOrEmpty(ChildrenIDsRequest.PageToken));

            Dictionary<string, string> filesResource = new();

            foreach (var item in ChildList)
            {
                Dictionary<string, string> items = GetContainsInFolder(item);
                if (items == null) continue;
                if (items.Count > 0)
                    foreach (var item1 in items)
                        filesResource.Add(item1.Key, item1.Value);
                else
                {
                    var file = ServiceV2.Files.Get(item).Execute();
                    filesResource.Add(file.Id, file.Title);
                }
                
            }

            return filesResource;
        }

        public void DownloadAllScheduleAtDisk(Dictionary<string, string> idTitle)
        {
            if (idTitle == null) return;
            if (!Directory.Exists("DownloadFile")) Directory.CreateDirectory("DownloadFile");
            foreach (var item in idTitle)
            {
                WebClient webClient = new WebClient();
                string path = $"DownloadFile/{item.Value}.xls";
                webClient.DownloadFile($@"https://drive.google.com/uc?export=download&id={item.Key}", path);
            }
        }

        private bool IsContentBlackList(string name)
        {
            List<string> strings = new() { "архив", "январь", "февраль", "март", "апрель", "май", "июнь", "июль", "август", "сентябрь", "октябр", "ноябрь", "декабрь", "расписание" };
            if (name.ToLower() != "1 корпус расписание")
                foreach (var item in strings)
                    if (name.ToLower().Contains(item)) return true;

            return false;
        }
    }
}
