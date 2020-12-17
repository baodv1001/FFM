using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.Models
{
    class TimeFrame
    {
        private int id;
        public int Id { get => id; set => id = value; }
        private string startTime;
        public string StartTime { get => startTime; set => startTime = value; }
        private string endTime;
        public string EndTime { get => endTime; set => endTime = value; }
        private long price;
        public long Price { get => price; set => price = value; }
        private int fieldType;
        public int FieldType { get => fieldType; set => fieldType = value; }

        public TimeFrame()
        {

        }
        public TimeFrame(int id, string start, string end, int type, long price)
        {
            this.id = id;
            this.startTime = start;
            this.endTime = end;
            this.fieldType = type;
            this.price = price;
        }
    }
}
