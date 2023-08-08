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
using Frontend.Model;
using Frontend.ViewModel;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for UserBoardsView.xaml
    /// </summary>
    public partial class UserBoardsView : Window
    {
            internal UserBoardsViewModel userBoardsViewModel;
        public UserBoardsView(UserModel user)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.userBoardsViewModel = new UserBoardsViewModel(user);
            DataContext = userBoardsViewModel;
        }

        /// <summary>
        /// This method creates a new board view of the user's selected board
        /// </summary>
        private void Select_Board_Click(object sender, RoutedEventArgs e)
        { 
                BoardView board = new BoardView(userBoardsViewModel.user, userBoardsViewModel.SelectedBoard);
                board.Show();
                this.Hide();
        }
    }
}

