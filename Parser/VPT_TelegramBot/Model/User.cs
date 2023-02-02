using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPT_TelegramBot.Model
{
    public class User
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int MessageMenuId { get; set; }
        public string? Role { get; set; }
        public string? UserName { get; set; }
        public string? DefaultGroup { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int MenuPosition { get; set; }
        public string? RoleKey { get; set; }
    }
}
