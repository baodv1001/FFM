using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    class Goods
    {
        //Properties

        private int idGoods;

        public int IdGoods { get => idGoods; set => idGoods = value; }

        private string name;

        public string Name { get => name; set => name = value; }

        private int amount;

        public int Amount { get => amount; set => amount = value; }

        private DateTime receivedDate;

        public DateTime ReceivedDate { get => receivedDate; set => receivedDate = value; }

        private double price;

        public double Price { get => price; set => price = value; }

        private string unit;

        public string Unit { get => unit; set => unit = value; }

        //Constructor

        public Goods()
        {

        }

        public Goods(int idGoods, string name, int amount, DateTime receivedDate, double price, string unit)
        {
            this.idGoods = idGoods;
            this.name = name;
            this.amount = amount;
            this.receivedDate = receivedDate;
            this.price = price;
            this.unit = unit;
        }
    }
}
