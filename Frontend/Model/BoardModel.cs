using IntroSE.Kanban.Backend.ServiceLayer.Objects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Frontend.Model
{
    public class BoardModel : NotifiableModel
    {
        public string Name { get; }
        private UserModel user;
        private ObservableCollection<ColumnModel> columns;
        private TaskModel selectedTask;
        private ColumnModel selectedColumn;
        private bool colEnableForward;
        private bool taskEnableForward;


        public ObservableCollection<ColumnModel> Columns
        {
            get
            {
                return columns;
            }
            set
            {
                columns = value;
            }
        }

        public TaskModel SelectedTask
        {
            get
            {
                return selectedTask;
            }
            set
            {
                selectedTask = value;
                TaskEnableForward = value != null;
                RaisePropertyChanged("SelectedTask");
            }
        }

        public ColumnModel SelectedColumn
        {
            get
            {
                return selectedColumn;
            }
            set
            {
                if (selectedColumn != null)
                {
                    SelectedColumn.SortEnableForward = false;
                }
                selectedColumn = value;
                ColEnableForward = value != null;
                if (selectedColumn != null)
                {
                    selectedColumn.SortEnableForward = value != null;
                }
                RaisePropertyChanged("SelectedColumn");
            }
        }

        public bool ColEnableForward
        {
            get => colEnableForward;
            private set
            {
                colEnableForward = value;
                RaisePropertyChanged("ColEnableForward");
            }
        }

        public bool TaskEnableForward
        {
            get => taskEnableForward;
            internal set
            {
                taskEnableForward = value;
                RaisePropertyChanged("TaskEnableForward");
            }
        }



        //Constructor
        public BoardModel(BackendController controller, UserModel user, string name, bool isNew) : base(controller)
        {

            this.user = user;
            Name = name;
            List<ColumnModel> boardColumns = new List<ColumnModel>();
            Columns = new ObservableCollection<ColumnModel>();
            BoardToFront srcBoard = controller.getBoard(user.Email, Name);
            foreach (int columnTF in srcBoard.Columns.Keys)
            {
                ColumnModel toAdd = new ColumnModel(controller, srcBoard.Columns[columnTF], user);
                Columns.Add(toAdd);
            }
            colEnableForward = true;
            taskEnableForward = true;
            RaisePropertyChanged("Columns");
        }

        public void setColumnsBoard()
        {
            foreach (ColumnModel col in this.columns)
            {
                col.Board = this;
            }
        }
    }
}
