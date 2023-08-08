using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOS;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class BoardController
    {
        private Dictionary<int, Board> boards;
        private Dictionary<string, List<Board>> usersBoardsDict;
        private int boardID;
        private BoardMapper boardMapper;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int COL_ID0 = 0;
        private int COL_ID1 = 1;
        private int COL_ID2 = 2;

        internal BoardMapper BoardMapper { get { return boardMapper; } }

        public BoardController()
        {
            boards = new Dictionary<int, Board>();
            usersBoardsDict = new Dictionary<string, List<Board>>();
            boardID = 0;
            boardMapper = new BoardMapper();
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        /// <summary>
        /// Loading all boards from the DB into the local memory. 
        /// </summary>
        public void LoadBoards()
        {
            Dictionary<int, BoardDTO> boardsDict = boardMapper.LoadBoards();
            foreach (int id in boardsDict.Keys)
            {
                BoardDTO boardDTO = boardsDict[id];
                Board board = new Board(boardDTO);
                boards.Add(id, board);
                foreach (string email in boardDTO.UsersString)
                {
                    if (!usersBoardsDict.ContainsKey(email))
                        usersBoardsDict[email] = new List<Board>();
                    usersBoardsDict[email].Add(board);
                }
            }
            boardID = boardMapper.BoardMaxId();
        }


        /// <summary>
        /// Deleting data. 
        /// </summary>
        public void DeleteData()
        {
            try
            {
                boardMapper.DeleteAll(boardID);
            }
            catch (Exception ex)
            {
                log.Warn(ex);
            }


        }


        /// <summary>
        /// Add a new board 
        /// </summary>
        /// <param name="toBeAdmin">email of user</param>
        /// <param name="name">name of board</param>
        public int addBoard(string toBeAdmin, string name)
        {
            if (toBeAdmin == null || name == null || name.Equals("") || toBeAdmin.Equals(""))
            {
                log.Error("error in addBoard - user or name is empty or null");
                throw new ArgumentException("User or name is invalid");
            }
            if (boardNameExistsForUser(toBeAdmin, name))
            {
                log.Error($"error in addBoard - user {toBeAdmin} already have board in name {name}");
                throw new Exception("Can't add board to user because the use already joined to a board with the same name");
            }
            Board newBoard = new Board(name, toBeAdmin, this.boardID);
            try
            {
                newBoard.boardDalDTO.addBoardToDAL();
                newBoard.boardDalDTO.IsPersistent = false;
            }
            catch (Exception)
            {
                log.Warn($"Failed to create board for '{toBeAdmin}'");
                throw new Exception($"Failed to create board for '{toBeAdmin}'");
            }
            boards.Add(boardID, newBoard);
            addToUsersBoardslist(toBeAdmin, newBoard);
            newBoard.boardDalDTO.IsPersistent = true;
            boardID++;
            log.Info($"Board created '{boardID}' successfully");
            return (boardID-1);
        }
        

        /// <summary>
        /// Deletes the board 
        /// </summary>
        /// <param name="actorUser">email of user</param>
        /// <param name="name">name of the board to remove</param>  
        public void deleteBoard(string actorUser, string name)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in deleteBoard - user empty or null");
                throw new Exception("User is invalid");
            }
            Board boardToRemove = getBoardByName(actorUser, name);
            if (boardToRemove == null)
            {
                log.Error($"error in deleteBoard - board {name} not exsit in {actorUser}");
                throw new Exception("Removing a board that doesn't exist is invalid");
            }
            if (!boardToRemove.isOwner(actorUser))
            {
                log.Error($"error in deleteBoard - user {actorUser} is not owner of board {name}");
                throw new Exception("Only the board's owner can delete the board");
            }
            else
            {
                try
                {
                    boardToRemove.boardDalDTO.deleteBoardFromDAL();
                    boardToRemove.boardDalDTO.IsPersistent = false;
                }
                catch (Exception e)
                {
                    log.Warn($"Failed to delete board '{boardToRemove.BOARD_ID1}'");
                    throw new Exception(e.Message);
                }
                List<string> boardUsers = boardToRemove.Users;
                foreach (string user in boardUsers)
                {
                    usersBoardsDict[user].Remove(boardToRemove);
                }
                boards.Remove(boardToRemove.BOARD_ID1);
                boardToRemove.boardDalDTO.IsPersistent = false;
                //tobeImplemented - deleting all the data from DB including tasks etc.
                log.Info($"deleteBoard {name} done");
            }
        }


        /// <summary>
        /// Check if the board name exists in a specific user board list
        /// </summary>
        /// <param name="actorUser">email of user</param>
        /// <param name="name">name of board</param>
        /// <returns>True if a board with the same name exsits for this user, False if not</returns>
        public Boolean boardNameExistsForUser(string actorUser, string name)
        {
            if (actorUser == null || name == null || actorUser.Equals("") || name.Equals(""))
            {
                log.Error("error in boardNameExistsForUser - user or name is empty or null");
                throw new Exception("User or board name is invalid");
            }
            if (usersBoardsDict.ContainsKey(actorUser))
            {
                foreach (Board board in usersBoardsDict[actorUser])
                {
                    if (board.Name.Equals(name))
                    {
                        return true;
                    }
                }
            }
            log.Info("boardNameExistsForUser done");
            return false;
            
        }


        /// <summary>
        /// Check if the board name exists in a specific user board list
        /// </summary>
        /// <param name="boardID">ID of the board to check if exists</param>
        /// <returns>True if a board with this ID exsits, False if not</returns>
        public Boolean boardExists(int boardID)
        {
            return boards.ContainsKey(boardID);
        }


        /// <summary>
        /// This method limits the number of tasks in a specific column.
        /// </summary>
        /// <param name="actorUser">The email address of the user</param>
        /// <param name="name">The name of the board</param>
        /// <param name="column">The column number</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        public void limitTasksForColumn(string actorUser, string name, int column, int limit)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in limitTasksForColumn - user empty or null");
                throw new Exception("User is invalid");
            }
            Board boardForMethod = getBoardByName(actorUser, name);
            if (boardForMethod == null)
            {
                log.Error($"error in limitTasksForColumn - board {name} doesn't exist in user {actorUser}");
                throw new Exception("Limiting number of tasks in a board that doesn't exist for this user is invalid");
            }
            else
            {
                boardForMethod.limitTasksForColumn(column, limit, actorUser);
            }
            log.Info($"limitTasksForColumn done in board {name}");
        }


        /// <summary>
        /// This method adds a new task.
        /// </summary>
        /// <param name="actorUser">Email of the user.</param>
        /// <param name="name">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns> The ID of the new Task </returns>
        public int createTask(string actorUser, string name, DateTime dueDate, string title, string description)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in createTask - user empty or null");
                throw new Exception("User is invalid");
            }
            Board boardForMethod = getBoardByName(actorUser, name);
            if (boardForMethod == null)
            {
                log.Error($"error in createTask - board {name} doesn't exist in user {actorUser}");
                throw new Exception("Creating a task in a board that doesn't exist for this user is invalid");
            }
            else
            {
                int newTaskID = boardForMethod.createTask(dueDate, title, description, actorUser);
                log.Info($"createTask done in board {name}");
                return newTaskID;
            }
        }


        /// <summary>
        /// Returns a list of all the In progress tasks that the user is assigned to.
        /// </summary>
        /// <param name="actorUser">Email of the user.</param>
        /// <returns>A List of Tasks that are assigned to the user from "1" column from all the actorUser's board</returns>
        public List<Task> getInProgress(string actorUser)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in getInProgress - user empty or null");
                throw new Exception("User is invalid");
            }
            List<Task> list = new List<Task>();
            if (usersBoardsDict.ContainsKey(actorUser))
            {
                foreach (Board board in usersBoardsDict[actorUser])
                {
                    foreach (Task task in board.getColumn(1))
                    {
                        if (task.isAssignee(actorUser))
                            list.Add(task);
                    }
                }
            }
            log.Info($"getInProgress for user {actorUser} done");
            return list;
        }


        /// <summary>
        /// This method advances a task to the next column
        /// </summary>
        /// <param name="actorUser">Email of user </param>
        /// <param name="name">The name of the board</param>
        /// <param name="column">The column number</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        public void advance(string actorUser, string name, int column, int taskId)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in advance - user empty or null");
                throw new Exception("User is invalid");
            }
            Board boardForMethod = getBoardByName(actorUser, name);
            if (boardForMethod == null)
            {
                log.Error($"error in advance - board {name} doesn't exist in user {actorUser}");
                throw new Exception("Creating a task in a board that doesn't exist for this user is invalid");
            }
            else
            {
                boardForMethod.advance(column, taskId, actorUser);
            }
            log.Info($"advance for user {actorUser} done");

        }


        /// <summary>
        /// This method get the limit of tasks in a specific column.
        /// </summary>
        /// <param name="actorUser">The email address of the user</param>
        /// <param name="name">The name of the board</param>
        /// <param name="column">The column number</param>
        /// <returns>the limit of tasks in this column. if a limit was not set returns -1</returns>
        public int getColumnLimit(string actorUser, string name, int column)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in getColumnLimit - user empty or null");
                throw new Exception("User is invalid");
            }
            Board boardForMethod = getBoardByName(actorUser, name);
            if (boardForMethod == null)
            {
                log.Error($"error in getColumnLimit - board {name} doesn't exist in user {actorUser}");
                throw new Exception("A user can't access the limit of tasks in a board that doesn't exist for this user is invalid");
            }
            else
            {
                log.Info($"getColumnLimit for user {actorUser} done");
                return (boardForMethod.getColumnLimit(column));
            }
            
        }


        /// <summary>
        /// This method get the name of a column
        /// </summary>
        /// <param name="column">The column number</param>       
        /// <param name="actorUser">The email address of the user</param>
        /// <param name="name">The name of the board</param>
        /// <returns>the column's name</returns>
        public string getColumnName(string actorUser, string name, int column)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in getColumnName - user empty or null");
                throw new Exception("User is invalid");
            }
            if (name == null || name.Equals(""))
            {
                log.Error("error in getColumnName - board name empty or null");
                throw new Exception("Board's name is invalid");
            }
            Board boardForMethod = getBoardByName(actorUser, name);
            if (boardForMethod != null)
            {
                string toReturn = "";
                if (column == COL_ID0)
                    toReturn = "backlog";
                else if (column == COL_ID1)
                    toReturn = "in progress";
                else if (column== COL_ID2)
                    toReturn = "done";
                else
                { 
                    log.Error($"error in getColumnName - column num is invalid");
                    throw new Exception("column num is invalid");
                }
               
                log.Info($"getColumnName for user {actorUser} done");
                return toReturn;
            }
            else
            {
                log.Error($"error in getColumnName - board name not exist for User {actorUser}");
                throw new Exception("Board's name is not exist for User");
            }
        }


        /// <summary>
        /// This method get a list of all the column's tasks
        /// </summary>
        /// <param name="userEmail">Email of user. </param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="column">The column number</param>
        /// <returns> a list of all of the column's taks </returns>
        public List<Task> getColumn(string actorUser, string name, int column)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in getColumn - user empty or null");
                throw new Exception("User is invalid");
            }
            if (name == null || name.Equals(""))
            {
                log.Error("error in getColumn - board name empty or null");
                throw new Exception("Board's name is invalid");
            }
            Board boardForMethod = getBoardByName(actorUser, name);
            if (boardForMethod == null)
            {
                log.Error($"error in getColumn - board {name} doesn't exist in user {actorUser}");
                throw new Exception("A user can't access tasks in a board that doesn't exist for this user");
            }
            else
            {
                List<Task> toReturn = boardForMethod.getColumn(column);
                log.Info($"getColumn for user {actorUser} done");
                return toReturn;
            }
        }


        /// <summary>
        /// Returns the requested board object.
        /// </summary>
        /// <param name="boardID">ID of the board</param>
        /// <returns> returnes the requested board object.</returns>
        public Board getBoard(int boardID)
        {
            if (!boardExists(boardID))
            {
                throw new Exception("Get board that doesn't exist");
            }
            return boards[boardID];
        }


        /// <summary>
        /// Add board to the user's list of boards 
        /// </summary>
        /// <param name="actorUser">email of user that wants to join the </param>
        /// <param name="boardID">ID of the board to join</param>
        public void joinBoard(string actorUser, int boardID)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in joinBoard - user empty or null");
                throw new Exception("User is invalid");
            }
            else
            {
                Board board = getBoard(boardID);
                if (boardNameExistsForUser(actorUser, board.Name))
                {
                    if (usersBoardsDict[actorUser].Contains(board))
                    {
                        log.Warn("error in joinBoard - user already joined to this board");
                        throw new Exception($"User already joined to this board.");
                    }
                    else
                    {
                        log.Error("error in joinBoard - board name already exists for user");
                        throw new Exception($"Can't join to board with same name as a bord the user already join in");
                    }
                }
                else
                {
                    try
                    {
                        board.joinBoard(actorUser);
                    }
                    catch (Exception e)
                    {
                        log.Warn($"Failed to join user '{actorUser}' to board '{boardID}'");
                        throw new Exception(e.Message);
                    }
                    addToUsersBoardslist(actorUser, getBoard(boardID));
                    log.Info($"user '{actorUser}' joined board '{board.Name}':'{boardID}' successfully");
                }

            }
            log.Info($"user {actorUser} joined board {boardID}");
        }

        /// <summary>
        /// This method removes a user from the members list of a board.
        /// </summary>
        /// <param name="actorUser">The email of the user. Must be logged in</param>
        /// <param name="boardID">The board's ID</param>
        public void leaveBoard(string actorUser, int boardID)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in leaveBoard - user empty or null");
                throw new Exception("User is invalid");
            }
            else
            {
                Board boardToLeave = getBoard(boardID);
                if (boardToLeave.leaveBoard(actorUser))
                    usersBoardsDict[actorUser].Remove(boardToLeave);
            }
            log.Info($" user {actorUser} leaved board {boardID}");
        }

        /// <summary>
        /// This method transfers a board ownership.
        /// </summary>
        /// <param name="actorUser">The current owner</param>
        /// <param name="newOwner">The new owner the current one wants to assign</param>
        /// <param name="name">The name of the board</param>
        public void transferOwnership(string actorUser, string newOwner, string name)
        {
            if (actorUser == null || actorUser.Equals("") || newOwner == null || newOwner.Equals(""))
            {
                log.Error("error in transferOwnership - user empty or null");
                throw new Exception("one of the users is invalid");
            }
            if (name == null || name.Equals(""))
            {
                log.Error("error in transferOwnership - Board name empty or null");
                throw new Exception("Board's name is invalid");
            }
            Board boardForMethod = getBoardByName(actorUser, name);
            if (boardForMethod == null)
            {
                log.Error($"error in transferOwnership - board {name} doesn't exist in user {actorUser}");
                throw new Exception("A user can't transfer ownership of a board that doesn't exist");
            }
            else
            {
                boardForMethod.transferOwnership(newOwner, actorUser);
            }
            log.Info($" owner ship on board {name} transferd from {actorUser} to {newOwner}");
        }


        /// <summary>
        /// Add board to the dictionary of the user's list of boards 
        /// </summary>
        /// <param name="actorUser">email of user that wants to join the board</param>
        /// <param name="boardToAdd">ID of the board to join</param>
        public void addToUsersBoardslist(string actorUser, Board boardToAdd)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in addToUsersBoardslist - user empty or null");
                throw new Exception("User is invalid");
            }
            else
            {
                if (!usersBoardsDict.ContainsKey(actorUser))
                {
                    List<Board> usersBoards = new List<Board>();
                    usersBoards.Add(boardToAdd);
                    usersBoardsDict.Add(actorUser, usersBoards);
                }
                else
                {
                    if (!boardNameExistsForUser(actorUser, boardToAdd.Name)) ///invalid if it doesnt exist?? 
                    {
                        usersBoardsDict[actorUser].Add(boardToAdd);
                    }
                }
            }
            log.Info($" user {actorUser} added to board {boardToAdd}");
        }


        /// <summary>
        /// This method returns a list of IDs of all user's boards.
        /// </summary>
        /// <param name="actorUser"></param>
        /// <returns> The standard json string format, original ReturnValue - list of int (IDs of all user's boards) </returns>
        public List<int> getUserBoards(string actorUser)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in getUserBoards - user empty or null");
                throw new Exception("User is invalid");
            }
            else
            {
                List<int> toReturn = new List<int>();
                if (usersBoardsDict.ContainsKey(actorUser))
                {
                    foreach (Board board in usersBoardsDict[actorUser])
                    {
                        toReturn.Add(board.BOARD_ID1);
                    }
                }
                log.Info($"getUserBoards for user {actorUser} done");
                return toReturn;
            }
        }


        /// <summary>
        /// get a the board's name by ID
        /// </summary>
        /// <param name="boardID">ID of the board to join</param>
        /// <returns> The board's name</returns>
        public string getBoardName(int boardID) 
        { 
            string toReturn = getBoard(boardID).Name; 
            return toReturn; //only for assignees??? if yes - add a check
        }


        /// <summary>
        /// Returns the requested board object.
        /// </summary>
        /// <param name="actorUser">Username of the user</param>
        /// <param name="name">requested boaard's name</param>
        /// <returns> returnes the requested board object.</returns>
        public Board getBoardByName(string actorUser, string name)
        {
            if (!usersBoardsDict.ContainsKey(actorUser))
            {
                log.Error($"error in getBoardByName - {actorUser} is not  member in board {name}");
                throw new Exception("User is not a member of any board");
            }
            foreach (Board board in usersBoardsDict[actorUser])
            {
                if (board.Name.Equals(name))
                {
                    return board;
                }
            }
            log.Info($"getBoardByName for user {actorUser} done");
            return null;
        }


        /// <summary>
        /// This method updates task title.
        /// </summary>
        /// <param name="actorUser">The email of the user </param>
        /// <param name="name">The name of the board </param>
        /// <param name="column">The Column of the task</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newTitle">New title for the task</param>
        public void updateTaskTitle(string actorUser, string name, int column, int taskId, string newTitle)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in updateTaskTitle - user empty or null");
                throw new Exception("User is invalid");
            }
            if (name == null || name.Equals(""))
            {
                log.Error("error in updateTaskTitle - name empty or null");
                throw new Exception("Board's name is invalid");
            }
            if (!isColumnEditable(column))
            {
                log.Error($"error in updateTaskTitle - can not edit task in column {column}");
                throw new Exception("Tasks that are not in columns 'backlog' or 'in Progress' cannot be edited");
            }
            Board boardForMethod = getBoardByName(actorUser, name);
            if (boardForMethod == null)
            {
                log.Error($"error in updateTaskTitle - board {name} doesn't exist in user {actorUser}");
                throw new Exception("updating a task in a board that doesn't exist for this user is invalid");
            }
            else
            {
                boardForMethod.updateTaskTitle(actorUser, column, taskId, newTitle);
            }
            log.Info($"updateTaskTitle to {newTitle} done for user {actorUser} done");
        }


        /// <summary>
        /// This method updates task Description.
        /// </summary>
        /// <param name="actorUser">The email of the user </param>
        /// <param name="name">The name of the board </param>
        /// <param name="column">The Column of the task</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDescription">New Description for the task</param>
        public void updateTaskDescription(string actorUser, string name, int column, int taskId, string newDescription)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in updateTaskDescription - user empty or null");
                throw new Exception("User is invalid");
            }
            if (name == null || name.Equals(""))
            {
                log.Error("error in updateTaskDescription - name empty or null");
                throw new Exception("Board's name is invalid");
            }
            if (!isColumnEditable(column))
            {
                log.Error($"error in updateTaskDescription - can not edit task in column {column}");
                throw new Exception("Tasks that are not in columns 'backlog' or 'in Progress' cannot be edited");
            }
            Board boardForMethod = getBoardByName(actorUser, name);
            if (boardForMethod == null)
            {
                log.Error($"error in updateTaskDescription - board {name} doesn't exist in user {actorUser}");
                throw new Exception("updating a task in a board that doesn't exist for this user is invalid");
            }
            else
            {
                boardForMethod.updateTaskDescription(actorUser, column, taskId, newDescription);
            }
            log.Info($"updateTaskDescription to {newDescription} done for user {actorUser} done");
        }


        /// <summary>
        /// This method updates task DueDate.
        /// </summary>
        /// <param name="actorUser">The email of the user </param>
        /// <param name="name">The name of the board </param>
        /// <param name="column">The Column of the task</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDueDate">New DueDate for the task</param>
        public void updateTaskDueDate(string actorUser, string name, int column, int taskId, DateTime newDueDate)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in updateTaskDueDate - user empty or null");
                throw new Exception("User is invalid");
            }
            if (name == null || name.Equals(""))
            {
                log.Error("error in updateTaskDueDate - name empty or null");
                throw new Exception("Board's name is invalid");
            }
            if (!isColumnEditable(column))
            {
                log.Error($"error in updateTaskDueDate - can not edit task in column {column}");
                throw new Exception("Tasks that are not in columns 'backlog' or 'in Progress' cannot be edited");
            }
            Board boardForMethod = getBoardByName(actorUser, name);
            if (boardForMethod == null)
            {
                log.Error($"error in updateTaskDueDate - board {name} doesn't exist in user {actorUser}");
                throw new Exception("updating a task in a board that doesn't exist for this user is invalid");
            }
            else
            {
                boardForMethod.updateTaskDueDate(actorUser, column, taskId, newDueDate);
            }
            log.Info($"updateTaskDueDate to {newDueDate} done for user {actorUser} done");
        }


        /// <summary>
        /// This method updates task Assignee.
        /// </summary>
        /// <param name="actorUser">The email of the user </param>
        /// <param name="name">The name of the board </param>
        /// <param name="column">The Column of the task</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newAssignee">New Assignee for the task</param>
        public void assignTask(string actorUser, string name, int column, int taskId, string newAssignee)
        {
            if (actorUser == null || actorUser.Equals("") || newAssignee == null || newAssignee.Equals(""))
            {
                log.Error("error in assignTask - user empty or null");
                throw new Exception("User is invalid");
            }
            if (name == null || name.Equals(""))
            {
                log.Error("error in assignTask - name empty or null");
                throw new Exception("Board's name is invalid");
            }
            if (!isColumnEditable(column))
            {
                log.Error($"error in assignTask - can not edit task in column {column}");
                throw new Exception("Tasks that are not in columns 'backlog' or 'in Progress' cannot be edited");
            }
            Board boardForMethod = getBoardByName(actorUser, name);
            if (boardForMethod == null)
            {
                log.Error($"error in assignTask - board {name} doesn't exist in user {actorUser}");
                throw new Exception("updating a task in a board that doesn't exist for this user is invalid");
            }
            else
            {
                boardForMethod.assignTask(actorUser, column, taskId, newAssignee);
            }
            log.Info($"assignTask to {newAssignee} done for user {actorUser} done");
        }


        /// <summary>
        /// Check if a a column is editable
        /// </summary>
        /// <param name="column">column we want to check</param>
        /// <returns>True if can edit, else False</returns>
        private Boolean isColumnEditable(int column)
        {
        if (column == 0 | column == 1)
            {
                return true;
            }
        return false;
        }

        /// <summary>
        /// This method get a list of all the column's tasks
        /// </summary>
        /// <param name="userEmail">Email of user. </param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="column">The column number</param>
        /// <returns> a list of all of the column's taks </returns>
        public Column getColumnObj(string actorUser, string boardName, int column)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in getColumnObj - user empty or null");
                throw new Exception("User is invalid");
            }
            if (boardName == null || boardName.Equals(""))
            {
                log.Error("error in getColumnObj - board name empty or null");
                throw new Exception("Board's name is invalid");
            }
            Board boardForMethod = getBoardByName(actorUser, boardName);
            if (boardForMethod == null)
            {
                log.Error($"error in getColumnObj - board {boardName} doesn't exist in user {actorUser}");
                throw new Exception("A user can't access tasks in a board that doesn't exist for this user");
            }
            else
            {
                Column toReturn = boardForMethod.getColumnObj(column);
                log.Info($"getColumnObj for user {actorUser} done");
                return toReturn;
            }
        }

        public List<String> getUserBoardsNames(string actorUser)
        {
            if (actorUser == null || actorUser.Equals(""))
            {
                log.Error("error in getUserBoards - user empty or null");
                throw new Exception("User is invalid");
            }
            else
            {
                List<string> toReturn = new List<string>();
                if (usersBoardsDict.ContainsKey(actorUser))
                {
                    foreach (Board board in usersBoardsDict[actorUser])
                    {
                        toReturn.Add(board.Name);
                    }
                }
                log.Info($"getUserBoards for user {actorUser} done");
                return toReturn;
            }
        }
    }
}
