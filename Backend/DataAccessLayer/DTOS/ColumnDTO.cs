using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOS
{
    class ColumnDTO : DTO
    {

        public const string ColumnBoardIDTable = "BoardID";
        public const string ColumnNumberTable = "ColumnNumber";
        public const string ColumnNameTable = "ColumnName";
        public const string ColumnTaskLimitTable = "TaskLimit";

        private int columnNum;
        private int boardID;
        private string columnName;
        private int taskLimit = -1;
        private List<TaskDTO> tasks;


        internal ColumnDTO(int id,int boardID, int columnNum, string columnName, int limit) : base(new ColumnMapper())
        {
            Id = id;
            this.boardID = boardID;
            this.columnNum = columnNum;
            this.columnName = columnName;
            this.taskLimit = limit;
            this.tasks = new List<TaskDTO>();
        }


        internal int BoardID
        {
            get { return boardID; }
        }

        internal int ColumnNumber
        {
            get { return columnNum; }
            set
            {
                bool res = _controller.Update(Id, ColumnNumberTable, value);
                if (!res)
                    throw new Exception($"Failed to update column number for column '{columnName}' in board '{boardID}' in the DB");
                columnNum = value;
            }
        }
        internal string ColumnName
        {
            get { return columnName; }
            set
            {
                bool res = _controller.Update(Id, ColumnNameTable, value);
                if (!res)
                    throw new Exception($"Failed to update column name for column '{columnName}' in board '{boardID}' in the DB");
                columnName = value;
            }
        }

        internal int TaskLimit
        {
            get { return taskLimit; }
            set
            {
                bool res = _controller.Update(Id, ColumnTaskLimitTable, value);
                if (!res)
                    throw new Exception($"Failed to update the amount of tasks limit from '{taskLimit}' to '{value}' for column '{columnName}' in board '{boardID}' in the DB");
                taskLimit = value;
            }
        }

        public List<TaskDTO> Tasks { get { return tasks; } set { tasks = value; } }

        /// <summary>
        /// Adds an existing task to a ColumnDTO's task list, and sets it's column field accordingly.
        /// </summary>
        /// <param name="taskToAdd">The TaskDTO to move column</param>
        public void addToTasksList (TaskDTO taskToAdd)
        {
            tasks.Add(taskToAdd);
            taskToAdd.ColumnNumber = this.columnNum;
            if (ColumnNumber == 2)
                taskToAdd.IsEditable = false;
        }

        /// <summary>
        /// removes an existing task from a ColumnDTO's task list.
        /// </summary>
        /// <param name="taskID">The ID of the TaskDTO to move column</param>
        /// <returns> The removed Task</returns>
        public TaskDTO removeFromTasksList(int taskID)
        {
            TaskDTO taskToRemove = getTaskDTO(taskID);
            if (taskToRemove == null)
            {
                throw new Exception($"Failed to find the taskDTO of task '{taskID}' in column '{columnNum}' in board '{boardID}'");
            }
            bool removed = tasks.Remove(taskToRemove);
            if (!removed)
            {
                throw new Exception($"Failed to remove taskDTO of task '{taskID}' from tasks list in column '{columnNum}' in board '{boardID}'");
            }
            return taskToRemove;
        }

        /// <summary>
        /// get a TaskDTO object by its taskID
        /// </summary>
        /// <param name="taskID">The ID of the Task we desire to get</param>
        /// <returns> The TaskDTO object</returns>
        private TaskDTO getTaskDTO(int taskID)
        {
            foreach (TaskDTO task in tasks)
            {
                if (task.Id == taskID)
                    return task;
            }
            return null;
        }

        /// <summary>
        /// updates the tasks limitation in DTO and updates in DB by calling the setter that updates in DB.
        /// </summary>
        /// <param name="limit">The limit of tasks</param>
        public void limitTasksForColumnInDAL(int limit)
        {
            TaskLimit = limit;
        }

        /// <summary>
        /// Calls the mapper to insert a new task to DB.
        /// </summary>
        /// <param name="newTask">the new task to insert to DB</param>
        internal void addTaskToDAL(TaskDTO newTask)
        {
            bool res = ((ColumnMapper)Controller).TaskMapper.Insert(newTask);
            if (!res)
            {
                throw new Exception($"Failed to create new task '{newTask.Id}' in board '{boardID}' in DB");
            }
            tasks.Add(newTask);
            newTask.IsPersistent = true;
        }

        /// <summary>
        /// Removes a user from being an assignee of all tasks in DB by calling the mapper.
        /// </summary>
        /// <param name="email">The email of the UserService to unassign</param>
        internal void removeUserFromTasks(string email)
        {
            foreach(TaskDTO task in tasks)
            {
                if (task.Assignee.Equals(email))
                    task.Assignee = "empty";
            }
        }
    }
}
