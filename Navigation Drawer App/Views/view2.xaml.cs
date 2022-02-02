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
    /// Logika interakcji dla klasy HomeView.xaml
    /// </summary>
    public partial class view2 : UserControl
    {
        private VacationEvent ev;
        public view2()
        {
            InitializeComponent();
            Globals.Connection.dataReceived += LoadEvents;
            Message msg = new ();
            msg.Operation = Message.Code.GetEventsFromMe;
            Globals.Connection.SendMessage(Serializer.Serialize(msg));
        }
        private void LoadEvents(object source, Message message)
        {
            List<VacationEvent> eventList;
            if (message.Operation == Message.Code.GetEventsFromMe)
            {
                eventList = Serializer.Deserialize<List<VacationEvent>>(message.Data);
                Application.Current.Dispatcher.Invoke(() => DrawEvents(eventList));
            }
        }
        private void DrawEvents(List<VacationEvent> events)
        {
            EventListBox.Items.Clear();
            foreach (var alert in events)
            {
                var textBlock = new TextBlock();
                textBlock.Text = $"Urlop od dnia {alert.Start.ToShortDateString()} do {alert.Stop.ToShortDateString()}";
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.TextAlignment = TextAlignment.Center;

                var button = new Button();
                button.Width = 200;
                button.Height = 100;
                button.Cursor = Cursors.Hand;
                button.Click += (source, args) =>
                {
                    ev = alert;
                    Globals.Connection.dataReceived += DrawEvent;
                    var message = new Message();
                    message.Operation = Message.Code.GetData;
                    message.Data = Serializer.Serialize(alert.Sender);
                    Globals.Connection.SendMessage(Serializer.Serialize(message));
                };

                button.Content = textBlock;
                EventListBox.Items.Add(button);
            }
        }
        private void DrawEvent(object source, Message message)
        {
            if (message.Operation == Message.Code.GetData)
            {
                Person person = Serializer.Deserialize<Person>(message.Data);
                Application.Current.Dispatcher.Invoke(() => new About_form(person, ev).Show());
                Globals.Connection.dataReceived -= DrawEvent;
            }
        }
        private void LoadWorkerData(object source, Message message)
        {

        }
    }
}
