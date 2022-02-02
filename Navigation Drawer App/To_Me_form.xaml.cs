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
    public partial class To_Me_form : Window
    {
        private int ID;
        public To_Me_form(Person worker, VacationEvent ev)
        {
            InitializeComponent();
            Credentials.Content = worker.Firstname + " " + worker.Lastname;
            Position.Content = worker.Position;
            Start.Content = ev.Start.ToShortDateString();
            Stop.Content = ev.Stop.ToShortDateString();
            Type.Content = ev.TypeDesc;
            ID = (int)ev.ID;

            var message = new Message();
            message.Operation = Message.Code.ChangeEventCode;
            message.Data = Serializer.Serialize(new int[]{ ID, (int)VacationEvent.Code.Seen });
            Globals.Connection.SendMessage(Serializer.Serialize(message));
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message();
            message.Operation = Message.Code.ChangeEventCode;
            message.Data = Serializer.Serialize(new int[] { ID, (int)VacationEvent.Code.Accepted});
            Globals.Connection.SendMessage(Serializer.Serialize(message));
            this.Close();
        }

        private void Revert_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message();
            message.Operation = Message.Code.ChangeEventCode;
            message.Data = Serializer.Serialize(new int[] { ID, (int)VacationEvent.Code.Refused });
            Globals.Connection.SendMessage(Serializer.Serialize(message));
            this.Close();
        }
    }
}
