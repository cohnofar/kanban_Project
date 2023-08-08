using Frontend.Model;
using Frontend.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for BoardView.xaml
    /// </summary>
    public partial class BoardView : Window
    {
        private BoardViewModel boardViewModel;
        internal BoardView(UserModel userModel, BoardModel boardModel)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            boardViewModel = new BoardViewModel(userModel, boardModel);
            this.DataContext = boardViewModel;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Back_Board_Click(object sender, RoutedEventArgs e)
        {
            UserBoardsView UserBoard = new UserBoardsView(boardViewModel.User);
            UserBoard.Show();
            this.Close();
        }
    }
}
