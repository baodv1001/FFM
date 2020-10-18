using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.Models
{
    class Account
    {
        //Properties

        private int idAccount;

        public int IdAccount { get => idAccount; set => idAccount = value; }

        private int idEmployee;

        public int IdEmployee { get => idEmployee; set => idEmployee = value; }
        
        private string username;

        public string Username { get => username; private set => username = value; }        

        private string password;

        public string Password { get => password; set => password = value; }        

        private int type;

        public int Type { get => type; set => type = value; }

        //Constructor

        public Account()
        {

        }

        public Account(int idAccount, int idEmployee, string username, string password, int type)
        {
            this.idAccount = idAccount;
            this.idEmployee = idEmployee;
            this.username = username;
            this.password = password;
            this.type = type;
        }
    }
}
