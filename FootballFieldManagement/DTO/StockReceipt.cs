using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace DTO
{
    class StockReceipt
    {
        //Constructor
        public StockReceipt()
        {

        }
        public StockReceipt(int idStockReceipt, int idEmployee, DateTime time, DateTime dateTime, double totalMoney)
        {
            this.idStockReceipt = idStockReceipt;
            this.idEmployee = idEmployee;
            this.dateTimeStockReceipt = dateTime;
            this.totalMoney = totalMoney;
        }
        //Attribute
        private int idStockReceipt;
        public int IdStockReceipt { get => idStockReceipt; set => idStockReceipt = value; }

        private int idEmployee;
        public int IdEmployee { get => idEmployee; set => idEmployee = value; }

        private DateTime dateTimeStockReceipt;
        public DateTime DateTimeStockReceipt { get => dateTimeStockReceipt; set => dateTimeStockReceipt = value; }

        private double totalMoney;
        public double TotalMoney { get => totalMoney; set => totalMoney = value; }

    }
}
