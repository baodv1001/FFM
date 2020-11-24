using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using FootballFieldManagement.ViewModels;
namespace FootballFieldManagement.Models
{
    public class Employee
    {
        //Properties
        private int idAccount;
        public int IdAccount { get => idAccount; set => idAccount = value; }

        private int idEmployee;

        public int IdEmployee { get => idEmployee; set => idEmployee = value; }

        private string name;

        public string Name { get => name; set { name = value; }  }

        private string gender;

        public string Gender { get => gender; set { gender = value;  } }
        
        private string phonenumber;

        public string Phonenumber { get => phonenumber; set { phonenumber = value;  } }

        private string address;

        public string Address { get => address; set { address = value;  } }

        private DateTime dateOfBirth;

        public DateTime DateOfBirth { get => dateOfBirth; set { dateOfBirth = value;  } }
        
        private double salary;

        public double Salary { get => salary; set { salary = value;  } }

        private string position;

        public string Position { get => position; set { position = value;  } }

        private DateTime startingdate;

        public DateTime Startingdate { get => startingdate; set { startingdate = value;  } }

        private byte[] image;

        public byte[] Image { get => image; set => image = value; }
        // Constructor

        public Employee()
        {

        }

        public Employee( int idEmployee, string name, string gender, string phonenumber, string address, DateTime dateOfBirth, double salary, string position, DateTime startingdate, int idAccount,byte[] image)
        {
            this.idAccount = idAccount;
            this.IdEmployee = idEmployee;
            this.name = name;
            this.gender = gender;
            this.phonenumber = phonenumber;
            this.address = address;
            this.dateOfBirth = dateOfBirth;
            this.salary = salary;
            this.position = position;
            this.startingdate = startingdate;
            this.image = image;
        }

    }
}
