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
        public BillInfo(int idBill, int idGoods, int quantity)
        {
            this.idBill = idBill;
            this.idGoods = idGoods;
            this.quantity = quantity;
        }

        //Attribute
        private int idBill;
        public int IdBill { get => idBill; set => idBill = value; }

        private int idGoods;
        public int IdGoods { get => idGoods; set => idGoods = value; }

        private int quantity;
        public int Quantity { get => quantity; set => quantity = value; }
    }
}
