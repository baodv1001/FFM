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
        public StockReceipt(int idStockReceipt, int idEmployee, DateTime dateTime, int total)
        {
            this.idStockReceipt = idStockReceipt;
            this.idEmployee = idEmployee;
            this.dateTimeStockReceipt = dateTime;
            this.total = total;
        }
        //Attribute
        private int idStockReceipt;
        public int IdStockReceipt { get => idStockReceipt; set => idStockReceipt = value; }

        private int idEmployee;
        public int IdEmployee { get => idEmployee; set => idEmployee = value; }

        private DateTime dateTimeStockReceipt;
        public DateTime DateTimeStockReceipt { get => dateTimeStockReceipt; set => dateTimeStockReceipt = value; }

        private int total;
        public int Total { get => total; set => total = value; }

    }
}
