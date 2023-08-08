using IntroSE.Kanban.Backend.ServiceLayer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    public class TaskModel : NotifiableModel
    {
        private int id;
        private string title;
        private string description;
        private DateTime dueDate;
        private DateTime creationTime;
        private string assignee;
        private string? boardName;
        private int columnNum;
        private ColumnModel? column;

        public int Id
        {
            get => id;
        }
        public string Title
        {
            get => title;
        }
        public string Description
        {
            get => description;
        }

        public DateTime DueDate
        {
            get => dueDate;
        }

        public DateTime CreationTime
        {
            get => creationTime;
            set
            {
                this.creationTime = value;
                RaisePropertyChanged("CreationTime");
            }
        }

        public string Assignee
        {
            get => assignee;

        }

        public string? BoardName { get { return boardName; } set { boardName = value; } }

        public ColumnModel? Column { get { return column; } set { column = value; } }

        public int ColumnNum
        {
            get => columnNum;
            set
            {
                this.columnNum = value;
                RaisePropertyChanged("ColumnOrdinal");
            }
        }

        public TaskModel(BackendController controller, TaskToFront taskTF, int columnNum) : base(controller)
        {
            this.id = taskTF.ID;
            this.title = taskTF.Title;
            this.description = taskTF.Description;
            this.creationTime = taskTF.CreationTime;
            this.dueDate = taskTF.DueDate;
            this.columnNum = columnNum;
            this.assignee = taskTF.EmailAssignee;
        }
    }
}