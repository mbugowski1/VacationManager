using Navigation_Drawer_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Navigation_Drawer_App.Commands
{
    public class UpdateViewCommand : ICommand
    {

        private MainViewModel viewModel;

        public UpdateViewCommand(MainViewModel viewModel) //constructor
        {
            this.viewModel = viewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)//if executed
        {
            if (parameter.ToString() == "view1")
            {
                viewModel.SelectedViewModel = new view1();
            }
            else if (parameter.ToString() == "view2")
            {
                viewModel.SelectedViewModel = new view2();
            }
            else if (parameter.ToString() == "view3")
            {
                viewModel.SelectedViewModel = new view3();
            }
        }
    }
}
