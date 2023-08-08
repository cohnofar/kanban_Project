using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOS;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;


namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Board
    {
        private string name;
        private Dictionary<int, Column> columns;
        private List<string> users;
        private int taskID;
        private readonly int BOARD_ID;
        private string owner;
        internal BoardDTO boardDalDTO;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public int BOARD_ID1 { get { return BOARD_ID; } }
        public string Name { get { return name; } }
        public List<string> Users { get { return users; } }
        public Dictionary<int, Column> Columns { get { return columns; } }

        public Board(string name, string actorUser, int boardID)
        {
            this.name = name;
            columns = new Dictionary<int, Column>();
            users = new List<string>();
            users.Add(actorUser);
            List<ColumnDTO> forColumnDTO = new List<ColumnDTO>();
            columns.Add(0, new Column(0, boardID));
            forColumnDTO.Add(columns[0].ColumnDalDTO);
            columns.Add(1, new Column(1, boardID));
            forColumnDTO.Add(columns[1].ColumnDalDTO);
            columns.Add(2, new Column(2, boardID));
            forColumnDTO.Add(columns[2].ColumnDalDTO);
            taskID = 0;
            this.BOARD_ID = boardID;
            this.owner = actorUser;
            boardDalDTO = new BoardDTO(boardID, name, actorUser, forColumnDTO);
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        internal Board(BoardDTO dto)
        {
            this.boardDalDTO = dto;
            this.name = dto.Name;
            this.owner = dto.Owner;
            this.BOARD_ID = dto.Id;
            this.columns = new Dictionary<int, Column>();
            foreach (ColumnDTO colDTO in dto.Columns)
            {
                Column col = new Column(colDTO);
                columns.Add(col.COLUMN_NUM1, col);
            }
            this.users = dto.UsersString;
            this.taskID = dto.taskMaxId(BOARD_ID); // needs to think how to implement
            boardDalDTO.IsPersistent = true;
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        /// <summary>
        /// get a list of the column's Tasks by the columnNum
        /// </summary>
        /// <param name="column"></param>
        /// <returns> a List of the tasks of the column with the given columnNum</returns>
        public List<Task> getColumn(int column)
        {
            if (!(columns.ContainsKey(column)))
            {
                log.Error($"error in get column's tasks list '{column}' is invalid column number");
                throw new Exception("Unable to return column's tasks list- invalid column number");
            }
            log.Info("geting column done");
            return columns[column].getAllTasks();
        }


        // <summary>
        /// limit the number of tasks in the specified column
        /// </summary>
        /// <param name="column"> the column number to limit</param>
        /// <param name="limit"> the desired limit</param>
        /// <param name="actorUser"> the user that wants to preform the action</param>
        public void limitTasksForColumn(int column, int limit, string actorUser)
        {
            if (!(columns.ContainsKey(column)))
            {
                log.Error($"error in Limiting task for column '{column}' is invalid column number");
                throw new Exception("Unable to limit column's tasks number - invalid column number");
            }
            if (!isOwner(actorUser))
            {
                log.Error($"error in Limiting task for column '{actorUser}' is not owner");
                throw new ArgumentException("Only the board's owner can limit tasks for column");
            }
            else
            {
                columns[column].limitTasksForColumn(limit);
                log.Info("limit task for column done");
            }
        }


        /// <summary>
        /// Add a task to this board
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="dueDate"></param>
        /// <returns>The ID of the added task</returns>
        public int createTask(DateTime dueDate, string title, string description, string actorUser)
        {
            if (!isJoined(actorUser))
            {
                log.Error($"error in creating task- '{actorUser}' in not member of the board");
                throw new ArgumentException("One must be a member of a board (joined) to add a task");
            }
            if (title == null || title.Equals(""))
            {

                log.Error("error in creating task-title or description is null or empty");
                throw new ArgumentException("title or description is invalid");
            }
            if (description == null)
                description = "";
            log.Info("create task done");
            columns[0].createTask(taskID, dueDate, title, description);
            taskID++;
            return taskID - 1;
        }


        // <summary>
        /// Advence a task to the next column
        /// </summary>
        /// <param name="column">the task's column number</param>
        /// <param name="taskId"> the taskID of the task to advance</param>
        /// <param name="actorUser">The user that wants to advance the task</param>
        public void advance(int column, int taskID, string actorUser)
        {
            if (!(columns.ContainsKey(column)))
            {
                log.Error($"error in advance Task '{column}' is invalid column number");
                throw new Exception("Unable to advance task - invalid column number");
            }
            if (column > 1)
            {
                log.Error("can't advance task from Done column");
                throw new Exception("Unable to advance task from Done column");
            }
            else
            {
                if (!getTask(taskID,column).isAssignee(actorUser))
                {
                    log.Error($"error in advance task '{actorUser}' is not assignee");
                    throw new Exception("Only the task's assignee can advance it!");
                }
                else
                {
                    try
                    {
                        boardDalDTO.advanceInDAL(column, taskID);
                        boardDalDTO.IsPersistent = false;
                    }
                    catch (Exception)
                    {
                        log.Warn($"Failed to advance task '{taskID}' from column number '{column}' in board '{BOARD_ID1}'");
                        throw new Exception($"Failed to advance task '{taskID}' from column number '{column}' in board '{BOARD_ID1}'");
                    }
                    Task taskToAddToNextColumn = columns[column].removeTask(taskID);
                    columns[column + 1].addTask(taskToAddToNextColumn);
                    boardDalDTO.IsPersistent = true;
                    log.Info("advance task done");
                }
            }

        }


        /// <summary>
        /// get a task by given task id
        /// </summary>
        /// <param name="taskID"> the desired task's taskID</param>
        /// <param name="column"> the desired task's column</param>
        /// <returns>The Task object with the given id</returns>
        public Task getTask(int taskID, int column)
        {
            Task toReturn = columns[column].getTask(taskID);
            if (toReturn == null)
            {
                log.Error($"task with Id '{taskID}' does not exist in column number '{column}'");
                throw new Exception("Task doesn't exist");

            }

            else
            {
                log.Info("get task done");
                return toReturn;
            }
        }


        /// <summary>
        /// get the limitation of tasks of this column
        /// <param name="column"> the desired column's number</param>
        /// </summary>
        /// <returns>the limit of tasks in this column. if a limit was not set returns -1</returns>
        public int getColumnLimit(int column)
        {
            if (!(columns.ContainsKey(column)))
            {
                log.Error($"error in getting column limit, '{column}' is invalid column number");
                throw new Exception("Unable to return column's tasks limit- invalid column number");
            }
            log.Info("get column limit done");
            return columns[column].MaxTasks;
        }


        /// <summary>
        /// Check if a user is the owner of a board
        /// </summary>
        /// <param name="actorUser">Username of the user we want to check</param>
        /// <returns>True if owner, else False</returns>
        public Boolean isOwner(string actorUser)
        {
            return (owner == actorUser);
        }


        /// <summary>
        /// Check if a user is joined to a board
        /// </summary>
        /// <param name="actorUser">Username of the user we want to check</param>
        /// <returns>True if joined, else False</returns>
        public Boolean isJoined(string actorUser)
        {
            return (users.Contains(actorUser));
        }


        // <summary>
        /// adding a UserService as a board's member
        /// </summary>
        /// <param name="actorUser">The user that wants to join the board</param>
        public void joinBoard(string actorUser)
        {
            if (isJoined(actorUser))
            {
                log.Error($"error in join board '{actorUser}' is already a member");
                throw new Exception("User is already a memner of this board");
            }
            else
            {
                try
                {
                    boardDalDTO.joinBoardUserToDAL(actorUser);
                    boardDalDTO.IsPersistent = false;
                }
                catch (Exception)
                {
                    log.Warn($"Failed to join user '{actorUser}' to board '{BOARD_ID1}'");
                    throw new Exception($"Failed to join user '{actorUser}' to board '{BOARD_ID1}' in bord-business layer");
                }
                users.Add(actorUser);
                log.Info($"adding '{actorUser}' to board '{BOARD_ID}' done");
                boardDalDTO.IsPersistent = true;
            }
        }


        // <summary>
        /// Advence a task to the next column
        /// </summary>
        /// <param name="actorUser">The user that wants to join the board</param>
        /// <returns>True if action was performed successfully, else False</returns>

        public Boolean leaveBoard(string actorUser)
        {
            Boolean toReturn = false;
            if (!users.Remove(actorUser))
            {
                log.Error($"error in leaving board -'{actorUser}'is not member in board {BOARD_ID}");
                throw new Exception("The User is not a member (never joined) of this board");
            }
            if (isOwner(actorUser))
            {
                log.Error($"error in leaving board -'{actorUser}' is owner of the board and he can't leave the board '{BOARD_ID}'");
                throw new Exception("The board's owner cannot leave the board");
            }
            else
            {
                try
                {
                    columns[0].columnDalDTO.removeUserFromTasks(actorUser);
                    columns[1].columnDalDTO.removeUserFromTasks(actorUser);
                }
                catch(Exception)
                {
                    log.Warn($"Failed to remove user '{actorUser}' forom the board '{BOARD_ID1}' membership");
                    throw new Exception($"Failed to remove user '{actorUser}' forom the board '{BOARD_ID1}' membership");
                }
                columns[0].removeUser(actorUser);
                columns[1].removeUser(actorUser);
                toReturn = true;
            }
            log.Info($"'{actorUser}' leave the board");
            return toReturn;
        }
        

        /// <summary>
        /// Transfer the ownership of the board
        /// </summary>
        /// <param name="newOwner">The new owner the current one wants to assign</param>
        /// <param name="actorUser">The current owner</param>
        public void transferOwnership(string newOwner, string actorUser)
        {
            if (!isOwner(actorUser))
            {
                log.Error($"error in transfer ownership -'{actorUser}' is not owner");
                throw new Exception("Only the board's owner can transfer the board's ownership");
            }
            if (!isJoined(newOwner))
            {
                log.Error($"error in transfer ownership -'{newOwner}' is not member in the board");
                throw new Exception("The board's owner has to be a member (joined) of the board");
            }
            else
            {
                try {
                    boardDalDTO.transferOwnershipInDAL(newOwner);
                    boardDalDTO.IsPersistent = false;
                }
                catch (Exception)
                {
                    log.Warn($"Failed to transfer ownership in board '{BOARD_ID1}' from '{actorUser}' to '{newOwner}'");
                    throw new Exception($"Failed to transfer ownership in board '{BOARD_ID1}'");
                }
                this.owner = newOwner;
                boardDalDTO.IsPersistent = true;
                log.Info($"ownership transferd from '{actorUser}' to '{newOwner}'");
            }
        }

        /// <summary>
        /// This method updates task title.
        /// </summary>
        /// <param name="actorUser">The email of the user </param>
        /// <param name="column">The Column of the task</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newTitle">New title for the task</param>
        public void updateTaskTitle(string actorUser, int column, int taskId, string newTitle)
        {
            Task toUpdate = getTask(taskId, column);
            if (toUpdate != null)
            {
                toUpdate.editTitle(actorUser, newTitle);
                log.Info("updateTaskTitle done");
            }
            else
                log.Error($"error in updateTaskTitle - '{taskId}' is not exist");
        }

        /// <summary>
        /// This method updates task Description.
        /// </summary>
        /// <param name="actorUser">The email of the user </param>
        /// <param name="column">The Column of the task</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDescription">New description for the task</param>
        public void updateTaskDescription(string actorUser, int column, int taskId, string newDescription)
        {
            Task toUpdate = getTask(taskId, column);
            if (toUpdate != null)
            {
                toUpdate.editDescription(actorUser, newDescription);
                log.Info("updateTaskDescription done");
            }
            else
                log.Error($"error in updateTaskDescription - '{taskId}' is not exist");
        }

        /// <summary>
        /// This method updates task dueDate.
        /// </summary>
        /// <param name="actorUser">The email of the user </param>
        /// <param name="column">The Column of the task</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDueDate">New dueDate for the task</param>
        public void updateTaskDueDate(string actorUser, int column, int taskId, DateTime newDueDate)
        {
            Task toUpdate = getTask(taskId, column);
            if (toUpdate != null)
            {
                toUpdate.editDueDate(actorUser, newDueDate);
                log.Info("updateTaskDueDate done");
            }
            else
                log.Error($"error in updateTaskDueDate - '{taskId}' is not exist");
        } 

        /// <summary>
        /// This method updates task Assignee.
        /// </summary>
        /// <param name="actorUser">The email of the user </param>
        /// <param name="column">The Column of the task</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newAssignee">New Assignee for the task</param>
        public void assignTask(string actorUser, int column, int taskId, string newAssignee)
        {
            if (!isJoined(newAssignee))
            {
                log.Error($"error in assignTask- '{newAssignee}' is not member in board '{BOARD_ID}'");
                throw new Exception("the new Assignee is not a member of this board");
            }
            Task toUpdate = getTask(taskId, column);
            if (toUpdate != null)
            {
                log.Info("assignTask done");
                toUpdate.editAssignee(newAssignee, actorUser);
            }
            else
            {
                log.Error($"error in assignTask - '{taskId}' is not exist");
            }
                
        }

        /// <summary>
        /// returns a Column Object
        /// </summary>
        /// <param name="int column">number of the column</param>
        /// <returns>column</returns>
        public Column getColumnObj(int column)
        {
            if (!(columns.ContainsKey(column)))
            {
                log.Error($"error in get column '{column}' is invalid column number");
                throw new Exception("Unable to return column- invalid column number");
            }
            log.Info("geting column done");
            return columns[column];
        }
    }
}
