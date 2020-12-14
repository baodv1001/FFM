using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.Models
{
    class FootballField
    {
        //Properties

        private int idField;

        public int IdField { get => idField; set => idField = value; }

        private string name;

        public string Name { get => name; set => name = value; }

        private int type; //5 => sân 5, 7 => sân 7

        public int Type { get => type; set => type = value; }
        private int status;
        public int Status { get => status; set => status = value; }

        private long price;
        public long Price { get => price; set => price = value; }

        private string note;

        public string Note { get => note; set => note = value; }

        //Constructor

        public FootballField()
        {

        }
        
        public FootballField(int idField, string name, int type,int status, long price, string note)
        {
            this.idField = idField;
            this.name = name;
            this.type = type;
            this.status = status;
            this.price = price;
            this.note = note;
        }
    }
}
