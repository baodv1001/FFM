using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    class StockReceiptInfo
    {
        //Constructor
        public StockReceiptInfo()
        {

        }
        public StockReceiptInfo(int idStockReceipt, int idGoods, int amount)
        {
            this.idStockReceipt = idStockReceipt;
            this.idGoods = idGoods;
            this.amount = amount;
        }
        //Attribute
        private int idStockReceipt;
        public int IdStockReceipt { get => idStockReceipt; set => idStockReceipt = value; }

        private int idGoods;
        public int IdGoods { get => idGoods; set => idGoods = value; }
        
        private int amount;
        public int Amount { get => amount; set => amount = value; }

    }
}
