using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FootballFieldManagement.ViewModels
{
    class HomeViewModel
    {
        public ICommand LogOutCommand { get; set; }

        public HomeViewModel()
        {
            LogOutCommand = new RelayCommand<Window>((parameter) => true, (parameter) => parameter.Close());

        }
    }
}
