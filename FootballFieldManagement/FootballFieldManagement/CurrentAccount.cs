using FootballFieldManagement.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FootballFieldManagement
{
    public static class CurrentAccount //Tài khoản hiện tại đang đăng nhập vào hệ thống
    {
        private static bool type;
        private static string displayName;
        private static byte[] image;
        private static int idAccount;
        private static string password;
        public static bool Type { get => type; set => type = value; }
        public static string DisplayName { get => displayName; set => displayName = value; }
        public static byte[] Image { get => image; set => image = value; }
        public static int IdAccount { get => idAccount; set => idAccount = value; }
        public static string Password { get => password; set => password = value; }
    }
}
