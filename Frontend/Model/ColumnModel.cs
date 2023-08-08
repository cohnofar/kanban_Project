using IntroSE.Kanban.Backend.ServiceLayer.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    public class ColumnModel : NotifiableModel
    {
        private string name;
        private bool flagAddTask;
        private int columnNum;
        private ObservableCollection<TaskModel> tasks;
        private BoardModel board;
        private UserModel user;
        private bool sortEnableForward = false;


        public string Name
        {
            get => name;
            set
            {
                name = value;
            }
        }
        public bool FlagAddTask
        {
            get => flagAddTask;
            set
            {
                flagAddTask = value;
            }
        }



        public int ColumnNum
        {
            get => columnNum;
            set
            {
                columnNum = value;
                foreach (TaskModel task in Tasks)
                {
                    task.ColumnNum = value;
                }
                RaisePropertyChanged("ColumnNum");
            }
        }


        public ObservableCollection<TaskModel> Tasks
        {
            get
            {
                return tasks;
            }
            set
            {
                tasks = value;
            }
        }

        public BoardModel Board {
            get { return board; }
            set 
            {
                board = value;
                foreach (TaskModel task in Tasks)
                {
                    task.BoardName = value.Name;
                    task.Column = this;
                }
            }
        }


        public bool SortEnableForward
        {
            get => sortEnableForward;
            set
            {
                if (Tasks.Count > 0)
                {
                    sortEnableForward = value;
                    RaisePropertyChanged("SortEnableForward");
                }
            }
        }

        //Constructor

        public ColumnModel(BackendController controller, ColumnToFront columnTF, UserModel user) : base(controller)
        {
            this.user = user;
            this.name = columnTF.Name;
            this.columnNum = columnTF.ColumnNum;
            Tasks = new ObservableCollection<TaskModel>();
            foreach (TaskToFront task in columnTF.TaskList)
            {
                Tasks.Add(new TaskModel(controller, task, columnTF.ColumnNum));
            }
        }
    }
}
