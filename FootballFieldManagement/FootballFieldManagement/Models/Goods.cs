using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.Models
{
    class Goods
    {
        //Properties
        private int idGoods;
        public int IdGoods { get => idGoods; set => idGoods = value; }

        private string name;
        public string Name { get => name; set => name = value; }

        private int quantity;
        public int Quantity { get => quantity; set => quantity = value; }

        private long unitPrice;
        public long UnitPrice { get => unitPrice; set => unitPrice = value; }

        private string unit;
        public string Unit { get => unit; set => unit = value; }

        private byte[] imageFile;
        public byte[] ImageFile { get => imageFile; set => imageFile = value; }

        private int isDeleted;
        public int IsDeleted { get => isDeleted; set => isDeleted = value; }

        //Constructor
        public Goods()
        {

        }
        public Goods(int idGoods, string name, string unit, long price, byte[] img, int quantity = 0, int isdeleted = 0)
        {
            this.idGoods = idGoods;
            this.name = name;
            this.unit = unit;
            this.unitPrice = price;
            this.quantity = quantity;
            this.isDeleted = isdeleted;
            this.imageFile = img;
        }
    }
}
