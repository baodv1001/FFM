using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.Models
{
    class StockReceiptInfo
    {
        //Constructor
        public StockReceiptInfo()
        {

        }
        public StockReceiptInfo(int idStockReceipt, int idGoods, int quantity, int importPrice)
        {
            this.idStockReceipt = idStockReceipt;
            this.idGoods = idGoods;
            this.quantity = quantity;
            this.importPrice = importPrice;
        }
        //Attribute
        private int idStockReceipt;
        public int IdStockReceipt { get => idStockReceipt; set => idStockReceipt = value; }

        private int idGoods;
        public int IdGoods { get => idGoods; set => idGoods = value; }

        private int quantity;
        public int Quantity { get => quantity; set => quantity = value; }

        private int importPrice;
        public int ImportPrice { get => importPrice; set => importPrice = value; }
    }
}
