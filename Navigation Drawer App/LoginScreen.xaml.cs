using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VacationManagerBackend;
using VacationManagerLibrary;

namespace Navigation_Drawer_App
{
    /// <summary>
    /// Logika interakcji dla klasy LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        private MainWindow dashboard;
        public LoginScreen()
        {
            InitializeComponent();
            Globals.Connection = new Network("127.0.0.1", 1337);
            Globals.Connection.dataReceived += Login;
            Globals.Connection.Connect();
        }
        public bool isDarkTheme { get; set; }

        private readonly PaletteHelper paletteHelper = new PaletteHelper();
        private void toggleTheme(object sender, RoutedEventArgs e)
        {
            ITheme theme = paletteHelper.GetTheme();

            if (isDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark)
            {
                isDarkTheme = false;
                theme.SetBaseTheme(Theme.Light);
            }
            else
            {
                isDarkTheme = true;
                theme.SetBaseTheme(Theme.Dark);
            }
            paletteHelper.SetTheme(theme);
        }

        private void exitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void btnSubmit_click(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text.Length == 0 || txtPassword.Password.Length == 0)
                return;
            string[] cred = new string[2];
            cred[0] = txtUsername.Text;
            cred[1] = txtPassword.Password;
            var message = new Message();
            message.Operation = Message.Code.CheckPass;
            message.Data = Serializer.Serialize(cred);
            Globals.Connection.SendMessage(Serializer.Serialize(message));
        }
        private void Login(object source, Message message)
        {
            if(message.Operation == Message.Code.CheckPass)
            {
                bool result = Serializer.Deserialize<bool>(message.Data);
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    if (result)
                    {
                        dashboard = new MainWindow(this);
                        dashboard.Show();
                        this.Visibility = Visibility.Collapsed;
                        txtUsername.Text = String.Empty;
                        txtPassword.Password = String.Empty;

                        Globals.Connection.dataReceived += GetData;
                        Message msg = new Message();
                        msg.Operation = Message.Code.GetData;
                        byte[] data = Serializer.Serialize(msg);
                        //https://open.spotify.com/track/40YcuQysJ0KlGQTeGUosTC?si=d4db8de5a268467e
                        Globals.Connection.SendMessage(data);
                    }
                    else
                    {
                        IncorrectText.Visibility = Visibility.Visible;
                    }
                });
            }
        }
        private void GetData(object source, Message message)
        {
            if(message.Operation == Message.Code.GetData)
            {
                Person person = Serializer.Deserialize<Person>(message.Data);
                Globals.Username = person.Username;
                Globals.Firstname = person.Firstname;
                Globals.Lastname = person.Lastname;
                Globals.Position = person.Position;
            }
        }
    }
}
