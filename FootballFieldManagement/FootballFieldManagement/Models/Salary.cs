using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FootballFieldManagement.Models
{
    class Salary
    {
        private long salaryBasic;
        public long SalaryBasic { get => salaryBasic; set => salaryBasic = value; }
        private int numOfShift;
        public int NumOfShift { get => numOfShift; set => numOfShift = value; }
        private long moneyPerShift;
        public long MoneyPerShift { get => moneyPerShift; set => moneyPerShift = value; }
        private int numOfFault;
        public int NumOfFault { get => numOfFault; set => numOfFault = value; }
        private long moneyPerFault;
        public long MoneyPerFault { get => moneyPerFault; set => moneyPerFault = value; }
        private long totalSalary;
        public long TotalSalary { get => totalSalary; set => totalSalary = value; }
        private int idEmployee;
        public int IdEmployee { get => idEmployee; set => idEmployee = value; }
        public Salary()
        {

        }
        public Salary(long salaryBasic, int numOfShift, long moneyPerShift, int numOfFault, long moneyPerFault, int idEmployee, long totalSalary)
        {
            this.salaryBasic = salaryBasic;
            this.numOfShift = numOfShift;
            this.moneyPerShift = moneyPerShift;
            this.numOfFault = numOfFault;
            this.moneyPerFault = moneyPerFault;
            this.totalSalary = totalSalary;
            this.idEmployee = idEmployee;
        }
    }
}
