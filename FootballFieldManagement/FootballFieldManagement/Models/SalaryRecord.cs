using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.Models
{
    class SalaryRecord
    {
        private int idSalaryRecord;
        private DateTime salaryRecordDate;
        private long total;

        public int IdSalaryRecord { get => idSalaryRecord; set => idSalaryRecord = value; }
        public DateTime SalaryRecordDate { get => salaryRecordDate; set => salaryRecordDate = value; }
        public long Total { get => total; set => total = value; }

        public SalaryRecord()
        {
        }
        public SalaryRecord(int id, DateTime dateTime, int totalMoney)
        {
            idSalaryRecord = id;
            salaryRecordDate = dateTime;
            total = totalMoney;
        }
    }
}
