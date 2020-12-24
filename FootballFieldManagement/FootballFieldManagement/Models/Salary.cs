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

        private int idAccount;
        public int IdAccount { get => idAccount; set => idAccount = value; }
        private DateTime salaryMonth;
        public DateTime SalaryMonth { get => salaryMonth; set => salaryMonth = value; }
        private DateTime datePay;
        public DateTime DatePay { get => datePay; set => datePay = value; }

        public Salary()
        {

        }
        public Salary(int idEmployee, int numOfShift, int numOfFault, long total, int idAccount, DateTime datePay, DateTime month )
        {
            this.idEmployee = idEmployee;
            this.numOfFault = numOfFault;
            this.numOfShift = numOfShift;
            this.totalSalary = total;
            this.idAccount = idAccount;
            this.datePay = datePay;
            this.salaryMonth = month;
        }
    }
}
