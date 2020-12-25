using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.Models
{
    class SalarySetting
    {
        private long salaryBase;
        public long SalaryBase { get => salaryBase; set => salaryBase = value; }
        private long moneyPerShift;
        public long MoneyPerShift { get => moneyPerShift; set => moneyPerShift = value; }
        private long moneyPerFault;
        public long MoneyPerFault { get => moneyPerFault; set => moneyPerFault = value; }
        private string typeEmployee;
        public string TypeEmployee { get => typeEmployee; set => typeEmployee = value; }
        private int standardWorkDays;
        public int StandardWorkDays { get => standardWorkDays; set => standardWorkDays = value; }

        public SalarySetting()
        {

        }
        public SalarySetting(long salaryBase, long moneyPerShift, long moneyPerFault, string typeEmployee, int standardWorkDays)
        {
            this.salaryBase = salaryBase;
            this.moneyPerShift = moneyPerShift;
            this.moneyPerFault = moneyPerFault;
            this.typeEmployee = typeEmployee;
            this.standardWorkDays = standardWorkDays;
        }
    }
}
