using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using Task = IntroSE.Kanban.Backend.BusinessLayer.Task;

namespace IntroSE.Kanban.Backend.ServiceLayer.Objects
{
    public class TaskToFront
    {
        public readonly int iD;
        public readonly DateTime creationTime;
        public readonly string title;
        public readonly string description;
        public readonly DateTime dueDate;
        public readonly string emailAssignee;
        public string boardName;
        public int columnNum;

        public DateTime CreationTime { get { return creationTime; } }
        public DateTime DueDate { get { return dueDate; } }
        public string Title { get { return title; } }
        public string Description { get { return description; } }
        public string EmailAssignee { get { return emailAssignee; } }
        public int ID { get { return iD; } }
        public string BoardName { get { return boardName; } set { boardName = value; } }
        public int ColumnNum { get { return columnNum; } set { columnNum = value; } }

        public TaskToFront(int id, DateTime creationTime, string title, string description, DateTime DueDate, string emailAssignee)
        {
            this.iD = id;
            this.creationTime = creationTime;
            this.title = title;
            this.description = description;
            this.dueDate = DueDate;
            this.emailAssignee = emailAssignee;
        }

        public TaskToFront(int id, DateTime creationTime, string title, string description, DateTime dueDate, string emailAssignee, string boardName, int columnNum)
        {
            this.iD = id;
            this.creationTime = creationTime;
            this.title = title;
            this.description = description;
            this.dueDate = dueDate;
            this.emailAssignee = emailAssignee;
            this.boardName = boardName;
            this.columnNum = columnNum;
        }

        public TaskToFront(Task busiTask)
        {
            this.iD = busiTask.TASK_ID1;
            this.creationTime = busiTask.CREATION_DATE1;
            this.title = busiTask.Title;
            this.description = busiTask.Description;
            this.dueDate = busiTask.DueDate;
            this.emailAssignee = busiTask.Assignee;
        }


    }
}

