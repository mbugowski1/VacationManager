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
using System.Windows.Shapes;
using VacationManagerLibrary;

namespace Navigation_Drawer_App
{
    /// <summary>
    /// Interaction logic for About_form.xaml
    /// </summary>
    public partial class About_form : Window
    {
        public About_form(Person worker, VacationEvent ev)
        {
            InitializeComponent();
            Credentials.Content = worker.Firstname + " " + worker.Lastname;
            Position.Content = worker.Position;
            Start.Content = ev.Start.ToShortDateString();
            Stop.Content = ev.Stop.ToShortDateString();
            Type.Content = ev.TypeDesc;
            State.Content = ev.CodeDesc;
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
