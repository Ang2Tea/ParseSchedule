using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_ExcelReader_Core.Models
{
    public class GoogleDriveFile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long? Size { get; set; }
        public long? Version { get; set; }
        public System.DateTime? CreatedTime { get; set; }
        public System.Collections.Generic.IList<string> Parents { get; set; }
        public string MimeType { get; set; }
    }
}
