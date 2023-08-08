using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOS
{
    class TaskDTO : DTO
    {
        public const string BoardIDTable = "BoardID";
        public const string CreationDateTable = "CreationDate";
        public const string DueDateTable = "DueDate";
        public const string TitleTable = "Title";
        public const string DescriptionTable = "Description";
        public const string AssigneeTable = "Assignee";
        public const string IsEditableTable = "IsEditable";
        public const string ColumnNumberTable = "ColumnNumber";
        private int boardID;
        private DateTime creationDate;
        private DateTime dueDate;
        private string title;  
        private string description;
        private string assignee;
        private bool isEditable;
        private int columnNumber;
        internal TaskDTO(int id, int boardID, string title, string description, DateTime creationDate, DateTime dueDate, string assignee) : base(new TaskMapper())
        {
            Id = id;
            this.boardID = boardID;
            this.creationDate = creationDate;
            this.dueDate = dueDate;
            this.title = title;
            this.description = description;
            if (assignee != null)
                this.assignee = assignee;
            else
                this.assignee = "null";
            isEditable = true;
            this.columnNumber = 0;
        }
        internal TaskDTO(int id, int boardID, string title, string description, DateTime creationDate, DateTime dueDate, string assignee, int columnNumber) : base(new TaskMapper())
        {
            Id = id;
            this.boardID = boardID;
            this.creationDate = creationDate;
            this.dueDate = dueDate;
            this.title = title;
            this.description = description;
            this.assignee = assignee;
            this.columnNumber = columnNumber;
            isPersistent = true;
            if (columnNumber != 2)
                isEditable = true;
            else
                isEditable = false;
        }
        internal int BoardID
        {
            get { return boardID; }
        }
        internal string Title
        {
            get { return title; }
            set
            {
                bool res = ((TaskMapper)_controller).Update(Id, boardID, TitleTable, value);
                if (!res)
                    throw new Exception($"Failed to update title for task '{Id}' from '{title}' to '{value}' in DB");
                title = value;
            }
        }
        internal bool IsEditable
        {
            get { return isEditable; }
            set
            {
                isEditable = value;
            }
        }

        internal string Description
        {
            get { return description; }
            set
            {
                bool res = ((TaskMapper)_controller).Update(Id, boardID, DescriptionTable, value);
                if (!res)
                    throw new Exception($"Failed to update description for task '{Id}' from '{description}' to '{value}' in DB");
                description = value;
            }
        }

        internal int ColumnNumber
        {
            get { return columnNumber; }
            set
            {
                bool res = ((TaskMapper)_controller).Update(Id, boardID, ColumnNumberTable, value);
                if (!res)
                    throw new Exception($"Failed to update column number for task '{Id}' from '{columnNumber}' to '{value}' in DB");
                columnNumber = value;
            }
        }

        internal DateTime CreationDate
        {
            get { return creationDate; }
        }
        internal DateTime DueDate
        {
            get { return dueDate; }
            set
            {
                bool res = ((TaskMapper)_controller).Update(Id, boardID, DueDateTable, value.ToString());
                if (!res)
                    throw new Exception($"Failed to update due date for task '{Id}' from '{columnNumber}' to '{value}' in DB");
                dueDate = value;
            }
        }
        internal string Assignee
        {
            get { return assignee; }
            set
            {
                if (value != null)
                {
                    bool res = ((TaskMapper)_controller).Update(Id, boardID, AssigneeTable, value);
                    if (!res)
                        throw new Exception($"Failed to update the assignee for task '{Id}' from '{assignee}' to '{value}' in DB");
                    assignee = value;
                }
                else 
                    value = "-1";
            }
        }

        /// <summary>
        /// Calls setter to update the task title in DB
        /// </summary>
        /// <param name="newTitle">The new title to update</param>
        internal void editTitleInDAL(string newTitle)
        {
            Title = newTitle;
        }


        /// <summary>
        /// Calls setter to update the task description in DB
        /// </summary>
        /// <param name="newDescription">The new description to update</param>
        internal void editDescriptionInDAL(string newDescript)
        {
            Description = newDescript;
        }
       

        /// <summary>
        /// Calls setter to update the task DueDate in DB
        /// </summary>
        /// <param name="newDate">The new DueDate to update</param>
        internal void editDueDateInDAL(DateTime newDate)
        {
            DueDate = newDate;
        }


        /// <summary>
        /// Calls setter to update the task Assignee in DB
        /// </summary>
        /// <param name="newAssignee">The new assignee to update</param>
        internal void editAssigneeInDAL(string newAssignee)
        {
            Assignee = newAssignee;
        }

    }
}
