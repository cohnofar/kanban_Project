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
using Frontend.ViewModel;
using Frontend.Model;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for RegistrationView.xaml
    /// </summary>
    public partial class RegistrationView : Window
    {
        private LoginViewModel loginViewModel;
        public RegistrationView()
        {
            loginViewModel = new LoginViewModel();
            this.DataContext = loginViewModel;
            loginViewModel.LoadData();
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// This method checks for the exictance of the logging in user and creates the UserBoardView page of the user if pasword and mail match
        /// </summary>
        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            UserModel user = loginViewModel.login();
            if (user != null)
            {
                UserBoardsView UserBoard = new UserBoardsView(user);
                UserBoard.Show();
                this.Close();
            }
        }

        /// <summary>
        /// This method creates a new user with the inserted info and creates an empty UserBoardView page
        /// </summary>
        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            UserModel user = loginViewModel.register();
            if (user != null)
            {
                UserBoardsView UseBoard = new UserBoardsView(user);
                UseBoard.Show();
                this.Close();
            }


        }
    }
}
