using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballFieldManagement.Models;

namespace FootballFieldManagement.DAL
{
    class AccountDal : DataProvider
    {
        List<Account> listAccount;
        internal List<Account> ListAccount { get => listAccount; set => listAccount = value; }
        public AccountDal()
        {
            ListAccount = new List<Account>();
            DataTable dt = LoadData("Account");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Account acc = new Account(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[2].ToString(), int.Parse(dt.Rows[i].ItemArray[3].ToString()));
                listAccount.Add(acc);
            }
        }
    }
}
