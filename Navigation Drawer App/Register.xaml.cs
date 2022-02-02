using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private LoginScreen parent;
        private MainWindow mainWindow;
        public Register(LoginScreen loginScreen, MainWindow mainWindow)
        {
            InitializeComponent();
            parent = loginScreen;
            this.mainWindow = mainWindow;
        }

        private void btnSubmit_click(object sender, RoutedEventArgs e)
        {
            RegisterUser(txtUsername.Text, txtPassword.Password, Firstname.Text, Surname.Text, Position.Text, Supervisor.Text);
            Globals.Username = txtUsername.Text;
            mainWindow.Show();
            Close();
        }
        private bool RegisterUser(string username, string password, string firstname, string lastname, string position, string supervisor)
        {
            var message = new Message();
            message.Operation = Message.Code.AddUser;
            string[] data = new string[] { username, password, firstname, lastname, position };
            message.Data = Serializer.Serialize(data);
            Globals.Connection.SendMessage(Serializer.Serialize(message));
            data = new string[] { username, supervisor };
            message.Operation = Message.Code.NewSupervisor;
            message.Data = Serializer.Serialize(data);
            Globals.Connection.SendMessage(Serializer.Serialize(message));
            return true;
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
    }
}
