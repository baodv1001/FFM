using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace FootballFieldManagement.Models
{
    class Salary
    {
        private int numOfShift;
        public int NumOfShift { get => numOfShift; set => numOfShift = value; }
        private int numOfFault;
        public int NumOfFault { get => numOfFault; set => numOfFault = value; }
        private long totalSalary;
        public long TotalSalary { get => totalSalary; set => totalSalary = value; }
        private int idEmployee;
        public int IdEmployee { get => idEmployee; set => idEmployee = value; }

        private DateTime salaryMonth;
        public DateTime SalaryMonth { get => salaryMonth; set => salaryMonth = value; }
        private int idSalaryRecord;
        public int IdSalaryRecord { get => idSalaryRecord; set => idSalaryRecord = value; }


        public Salary()
        {

        }
        public Salary(int idEmployee, int numOfShift, int numOfFault, long total, DateTime month, int idSalaryRecord)
        {
            this.idEmployee = idEmployee;
            this.numOfFault = numOfFault;
            this.numOfShift = numOfShift;
            this.totalSalary = total;
            this.salaryMonth = month;
            this.IdSalaryRecord = idSalaryRecord;
        }
    }
}
