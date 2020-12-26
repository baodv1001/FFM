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
        private int idAccount;

        public int IdSalaryRecord { get => idSalaryRecord; set => idSalaryRecord = value; }
        public DateTime SalaryRecordDate { get => salaryRecordDate; set => salaryRecordDate = value; }
        public long Total { get => total; set => total = value; }
        public int IdAccount { get => idAccount; set => idAccount = value; }

        public SalaryRecord()
        {
        }
        public SalaryRecord(int id, DateTime dateTime, long totalMoney, int idAccount)
        {
            this.idSalaryRecord = id;
            this.salaryRecordDate = dateTime;
            this.total = totalMoney;
            this.idAccount = idAccount;
        }
    }
}
