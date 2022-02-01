using Navigation_Drawer_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Navigation_Drawer_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            //set tooltip visibility

            if(Tg_btn.IsChecked== true)
            {
                tt_add.Visibility = Visibility.Collapsed;
                tt_my_holidays.Visibility = Visibility.Collapsed;
                tt_holidays_to_me.Visibility = Visibility.Collapsed;
                tt_logout.Visibility = Visibility.Collapsed;
            }
            else
            {
                tt_add.Visibility = Visibility.Visible;
                tt_my_holidays.Visibility = Visibility.Visible;
                tt_holidays_to_me.Visibility = Visibility.Visible;
                tt_logout.Visibility = Visibility.Visible;
            }
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_btn.IsChecked = false;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnLogout_click(object sender, RoutedEventArgs e)
        {
            LoginScreen loginScreen = new LoginScreen();
            loginScreen.Show();
            this.Close();
        }
    }
}
