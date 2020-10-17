using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.Models
{
    class BillInfo
    {
        //Constructor
        public BillInfo()
        {

        }
        public BillInfo(int idBill, int idFoodballField, int idGoods, int amount)
        {
            this.idBill = idBill;
            this.idFoodballField = idFoodballField;
            this.idGoods = idGoods;
            this.amount = amount;
        }

        //Attribute
        private int idBill;
        public int IdBill { get => idBill; set => idBill = value; }

        private int idFoodballField;
        public int IdFoodballField { get => idFoodballField; set => idFoodballField = value; }

        private int idGoods;
        public int IdGoods { get => idGoods; set => idGoods = value; }

        private int amount;
        public int Amount { get => amount; set => amount = value; }

    }
}
