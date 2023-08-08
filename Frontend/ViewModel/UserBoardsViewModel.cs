using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frontend.Model;

namespace Frontend.ViewModel
{
    public class UserBoardsViewModel: Notifiable
    {
        public UserModel user { get; }
        private BackendController controller;
        public BoardsDisplayModel Display { get; private set; }
        private string? name;
        private BoardModel? selectedBoard = null;
        private TaskModel? selectedTask;
        private bool taskEnableForward;
        private bool boardEnableForward;



        public string? Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        public BoardModel SelectedBoard
        {
            get
            {
                return selectedBoard;
            }
            set
            {
                if (value == null)
                {
                    selectedBoard = value;
                    BoardEnableForward = false;
                }
                else
                {
                    selectedBoard = value;
                    SelectedTask = null;
                    BoardEnableForward = true;
                }
                RaisePropertyChanged("SelectedBoard");
            }
        }

        public TaskModel? SelectedTask
        {
            get
            {
                return selectedTask;
            }
            set
            {
                if (value == null)
                {
                    selectedTask = value;
                    TaskEnableForward = value != null;
                }
                else
                {
                    selectedTask = value;
                    SelectedBoard = null;
                    TaskEnableForward = value != null;
                }
                RaisePropertyChanged("SelectedTask");
            }
        }

        public bool TaskEnableForward
        {
            get => taskEnableForward;
            private set
            {
                taskEnableForward = value;
                RaisePropertyChanged("TaskEnableForward");
            }
        }

        public bool BoardEnableForward
        {
            get => boardEnableForward;
            private set
            {
                boardEnableForward = value;
                RaisePropertyChanged("BoardEnableForward");
            }
        }

        //Constructor
        public UserBoardsViewModel(UserModel user)
        {
            boardEnableForward = false;
            taskEnableForward = true;
            this.user = user;
            this.controller = user.Controller;
            Display = new BoardsDisplayModel(controller, user);
        }
    }
}
