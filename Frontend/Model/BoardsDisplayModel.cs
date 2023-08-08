using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Frontend.Model
{
    public class BoardsDisplayModel: NotifiableModel
    {
        private UserModel user;
        public ObservableCollection<BoardModel> Boards { get; set; }

        //Constructor
        public BoardsDisplayModel(BackendController controller, UserModel user) : base(controller)
        {
            this.user = user;
            List<BoardModel> boardsToField = new List<BoardModel>();
            List<string> boardNames = controller.getUserBoardsNames(user.Email);
            foreach (string boardName in boardNames)
            {
                BoardModel toAdd = new BoardModel(controller, user, boardName, false);
                toAdd.setColumnsBoard();
                boardsToField.Add(toAdd);
            }
            Boards = new ObservableCollection<BoardModel>(boardsToField);
        }
    }
}
