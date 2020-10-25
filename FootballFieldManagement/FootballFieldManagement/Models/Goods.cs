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

        private int amount;

        public int Amount { get => amount; set => amount = value; }

        private double price;

        public double Price { get => price; set => price = value; }

        private string unit;

        public string Unit { get => unit; set => unit = value; }

        //Constructor

        public Goods()
        {

        }

        public Goods(int idGoods, string name, int amount = 0, string unit = "", double price = 0)
        {
            this.idGoods = idGoods;
            this.name = name;
            this.amount = amount;
            this.unit = unit;
            this.price = price;
        }
    }
}
