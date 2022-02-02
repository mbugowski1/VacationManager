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
using VacationManagerLibrary;

namespace Navigation_Drawer_App.Views
{
    /// <summary>
    /// Logika interakcji dla klasy AccountView.xaml
    /// </summary>
    public partial class view1: UserControl
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public VacationEvent.Type Type { get; private set; }
        private string _recipient;
        public view1()
        {
            InitializeComponent();
            Credentials.Content = Globals.Firstname + " " + Globals.Lastname;
            Globals.Connection.dataReceived += GetSupervisor;
            var msg = new Message();
            msg.Operation = Message.Code.GetMySupervisors;
            Globals.Connection.SendMessage(Serializer.Serialize(msg));
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StartDate.SelectedDate == null)
                return;
            if (EndDate.SelectedDate == null)
                return;
            Start = (DateTime)StartDate.SelectedDate;
            End = (DateTime)EndDate.SelectedDate;
            var vacEvent = new VacationEvent();
            if ((bool)Demanded.IsChecked)
            {
                Type = VacationEvent.Type.Demanded;
            }
            else
            {
                Type = VacationEvent.Type.Normal;
            }
            vacEvent.Sender = Globals.Username;
            vacEvent.Recipient = _recipient;
            if(_recipient == Globals.Username)
                vacEvent.CodeId = VacationEvent.Code.Accepted;
            else
                vacEvent.CodeId = VacationEvent.Code.Pending;
            vacEvent.Start = Start;
            vacEvent.Stop = End;
            vacEvent.TypeId = Type;
            var msg = new Message();
            msg.Operation = Message.Code.AddEvent;
            msg.Data = Serializer.Serialize(vacEvent);
            Globals.Connection.SendMessage(Serializer.Serialize(msg));
            Sent().GetAwaiter();
            msg = new();
            msg.Operation = Message.Code.GetEventsFromMe;
            Globals.Connection.SendMessage(Serializer.Serialize(msg));

        }
        private void GetSupervisor(object sender, Message args)
        {
            if (args.Operation == Message.Code.GetMySupervisors)
            {
                var list = Serializer.Deserialize<List<string>>(args.Data);
                if (list.Count == 0)
                    _recipient = Globals.Username;
                else
                    _recipient = Serializer.Deserialize<List<string>>(args.Data)[0];
            }
            Globals.Connection.dataReceived -= GetSupervisor;
        }
        private async Task Sent()
        {
            State.Content = "Wyslano";
            await Task.Delay(5000);
            State.Content = "Oczekiwanie";
        }
    }
}
