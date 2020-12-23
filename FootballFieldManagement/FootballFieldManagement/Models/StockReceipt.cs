using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace FootballFieldManagement.Models
{
    class StockReceipt
    {
        //Constructor
        public StockReceipt()
        {

        }
        public StockReceipt(int idStockReceipt, int idAccount, DateTime dateTime, long total)
        {
            this.idStockReceipt = idStockReceipt;
            this.idAccount = idAccount;
            this.dateTimeStockReceipt = dateTime;
            this.total = total;
        }
        //Attribute
        private int idStockReceipt;
        public int IdStockReceipt { get => idStockReceipt; set => idStockReceipt = value; }

        private int idAccount;
        public int IdAccount { get => idAccount; set => idAccount = value; }

        private DateTime dateTimeStockReceipt;
        public DateTime DateTimeStockReceipt { get => dateTimeStockReceipt; set => dateTimeStockReceipt = value; }

        private long total;
        public long Total { get => total; set => total = value; }

    }
}
