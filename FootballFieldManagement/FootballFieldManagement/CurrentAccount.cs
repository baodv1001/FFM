using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement
{
    public static class CurrentAccount
    {
        private static bool type;
        private static string displayName;
        private static string image;

        public static bool Type { get => type; set => type = value; }
        public static string DisplayName { get => displayName; set => displayName = value; }
        public static string Image { get => image; set => image = value; }
    }
}
