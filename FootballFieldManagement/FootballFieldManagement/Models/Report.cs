using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.Models
{
    class Report
    {
        //Properties
        private int month;
        private int year;
        private double revenue;
        private double expense;

        public int Month { get => month; set => month = value; }
        public int Year { get => year; set => year = value; }
        public double Revenue { get => revenue; set => revenue = value; }
        public double Expense { get => expense; set => expense = value; }

        //Constructors
        public Report()
        {

        }
        public Report(int m, int y, double ic, double oc)
        {
            month = m;
            year = y;
            revenue = ic;
            expense = oc;
        }
    }
}
