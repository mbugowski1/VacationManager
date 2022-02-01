using Navigation_Drawer_App.Commands;
using Navigation_Drawer_App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Navigation_Drawer_App.ViewModels
{
    public class MainViewModel:BaseViewModel
    {
        private BaseViewModel _selectedViewModel = new view0();

        public BaseViewModel SelectedViewModel
        {
            get { return _selectedViewModel; }
            set {
                _selectedViewModel = value; 
                OnPropertyChanged(nameof(SelectedViewModel));
            }
        }

        public ICommand UpdateViewCommand { get; set; }

        public MainViewModel()
        {
            UpdateViewCommand = new UpdateViewCommand(this);
        }

    }
}
