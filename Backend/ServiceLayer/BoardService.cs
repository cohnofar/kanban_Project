using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer.Objects;
using log4net;
using log4net.Config;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Each of the class' methods returns a JSON string with the following structure (see <see cref="System.Text.Json"/>):
    /// <code>
    /// {
    ///     "ErrorMessage": &lt;string&gt;,
    ///     "ReturnValue": &lt;object&gt;
    /// }
    /// </code>
    /// Where:
    /// <list type="bullet">
    ///     <item>
    ///         <term>ReturnValue</term>
    ///         <description>
    ///             The return value of the function.
    ///             <para>If the function does not return a value or an exception has occorred, then the field is undefined.</para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>ErrorMessage</term>
    ///         <description>If an exception has occorred, then this field will contain a string of the error message.</description>
    ///     </item>
    /// </list>
    /// </para>
    /// </summary>
    public class BoardService
    {
        private BoardController bc;
        private UserController uc;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BoardService(BoardController bc, UserController uc)
        {
            this.bc = bc;
            this.uc = uc;
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        /// <summary>
        /// This method adds a board and assign it to the user.
        /// </summary>
        /// <param name="userEmail">Email of the user</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>The standard json string format, original ReturnValue - null </returns>
        public string addBoard(string userEmail, string boardName)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    log.Error("User " + userEmail + " is not logged in");
                    return res.ResponseToJson(); ;
                }
                else
                {
                    bc.addBoard(userEmail, boardName);
                    Response<string> res = new Response<string>(null, null);
                    log.Info("Board " + boardName + " was added successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }


        /// <summary>
        /// This method removes a board to the specific user.
        /// </summary>
        /// <param name="userEmail">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>The standard json string format, original ReturnValue - null </returns>
        public string removeBoard(string userEmail, string boardName)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    log.Error("User " + userEmail + " is not logged in");
                    return res.ResponseToJson(); ;
                }
                else
                {
                    bc.deleteBoard(userEmail, boardName);
                    Response<string> res = new Response<string>(null, null);
                    log.Info("Board " + boardName + " removed successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }


        /// <summary>
        /// This method limits the number of tasks in a specific column.
        /// </summary>
        /// <param name="userEmail">The email address of the user</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="column">The column number</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>The standard json string format, original ReturnValue - null </returns>
        public string limitTasksForColumn(string userEmail, string boardName, int column, int limit)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    log.Error("User " + userEmail + " is not logged in");
                    return res.ResponseToJson(); ;
                }
                else
                {
                    bc.limitTasksForColumn(userEmail, boardName, column, limit);
                    Response<string> res = new Response<string>(null, null);
                    log.Info("Limit task for column was done successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }

        /// <summary>
        /// This method adds a new task.
        /// </summary>
        /// <param name="userEmail">Email of the user.</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns> The standard json string format, original ReturnValue  - string email </returns>
        public string createTask(string userEmail, string boardName, DateTime dueDate, string title, string description = "")
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    log.Error("User " + userEmail + " is not logged in");
                    return res.ResponseToJson(); ;
                }
                else
                {
                    bc.createTask(userEmail, boardName, dueDate, title, description);
                    Response<string> res = new Response<string>(null, null);
                    log.Info("Task was created successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }

        /// <summary>
        /// This method returns all the In progress tasks of the user.
        /// </summary>
        /// <param name="userEmail">Email of the user. </param>
        /// <returns> The standard json string format, original ReturnValue - a json tasks list </returns>
        public string getInProgress(string userEmail)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    log.Error("User " + userEmail + " is not logged in");
                    return res.ResponseToJson();
                }
                else
                {
                    List<IntroSE.Kanban.Backend.BusinessLayer.Task> inProgressList = bc.getInProgress(userEmail);
                    List<TaskToJson> taskJsonList = new List<TaskToJson>();
                    for (int i = 0; i < inProgressList.Count; i++)
                    {
                        TaskToJson task = new TaskToJson(inProgressList[i]);
                        taskJsonList.Add(task);
                    }
                    Response<List<TaskToJson>> res = new Response<List<TaskToJson>>(null, taskJsonList);
                    log.Info("get the in progress task was done successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }


        /// <summary>
        /// This method advances a task to the next column
        /// </summary>
        /// <param name="userEmail">Email of user. </param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="column">The column number</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>The standard json string format, original ReturnValue null </returns>
        public string advance(string userEmail, string boardName, int column, int taskId)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    log.Error("User " + userEmail + " is not logged in");
                    return res.ResponseToJson();
                }
                else
                {
                    bc.advance(userEmail, boardName, column, taskId);
                    Response<string> res = new Response<string>(null, null);
                    log.Info("advance task was done successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }

        /// <summary>
        /// This method get the limit of a column
        /// </summary>
        /// <param name="userEmail">Email of user. </param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="column">The column number</param>
        /// <returns>The standard json string format, original ReturnValue int - the column limit</returns>
        public string getColumnLimit(string userEmail, string boardName, int column)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    log.Error("User " + userEmail + " is not logged in");
                    return res.ResponseToJson();
                }
                else
                {
                    int toReturn = bc.getColumnLimit(userEmail, boardName, column);
                    Response<int> res;
                    res = new Response<int>(null, toReturn);
                    log.Info("Limit has returned successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }

        /// <summary>
        /// This method get the name of a column
        /// </summary>
        /// <param name="userEmail">Email of user. </param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="column">The column number</param>
        /// <returns>The standard json string format, original ReturnValue string - the columnName</returns>
        public string getColumnName(string userEmail, string boardName, int column)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    return res.ResponseToJson();
                }
                else
                {
                    string toReturn = bc.getColumnName(userEmail, boardName, column);
                    Response<string> res = new Response<string>(null, toReturn);
                    log.Info("Column name was returned successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                return res.ResponseToJson();
            }
        }

        /// <summary>
        /// This method get a list of all the column's tasks
        /// </summary>
        /// <param name="userEmail">Email of user. </param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="column">The column number</param>
        /// <returns> The standard json string format, original ReturnValue - a json tasks list </returns>
        public string getColumn(string userEmail, string boardName, int column)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    return res.ResponseToJson();
                }
                else
                {
                    List<IntroSE.Kanban.Backend.BusinessLayer.Task> columnList = bc.getColumn(userEmail, boardName, column);
                    List<TaskToJson> taskJsonList = new List<TaskToJson>();
                    for (int i = 0; i < columnList.Count; i++)
                    {
                        TaskToJson task = new TaskToJson(columnList[i]);
                        taskJsonList.Add(task);
                    }
                    Response<List<TaskToJson>> res = new Response<List<TaskToJson>>(null, taskJsonList);
                    log.Info("A list of the column's tasks was returned successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                return res.ResponseToJson();
            }
        }

        /// <summary>
        /// This method returns a list of IDs of all user's boards.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns> The standard json string format, original ReturnValue - list of int (IDs of all user's boards) </returns>
        public string getUserBoards(string userEmail)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    log.Error("User " + userEmail + " is not logged in");
                    return res.ResponseToJson();
                }
                else
                {
                    List<int> toReturn = bc.getUserBoards(userEmail);
                    Response<List<int>> res;
                    res = new Response<List<int>>(null, toReturn);
                    log.Info("List of the user " + userEmail + " boards was returned successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }

        /// <summary>
        /// This method returns a board's name
        /// </summary>
        /// <param name="boardId">The board's ID</param>
        /// <returns> The standard json string format, original ReturnValue - string (The name of the board) </returns>
        public string getBoardName(int boardId)
        {
            try
            {
                string toReturn = bc.getBoardName(boardId);
                Response<string> res = new Response<string>(null, toReturn);
                log.Info("Board's name was returned successfully");
                return res.ResponseToJson();
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                return res.ResponseToJson();
            }
        }


        /// <summary>
        /// This method adds a user as member to an existing board.
        /// </summary>
        /// <param name="userEmail">The email of the user that joins the board. Must be logged in</param>
        /// <param name="boardID">The board's ID</param>
        /// <returns>The standard json string format, original ReturnValue null</returns>
        public string joinBoard(string userEmail, int boardID)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    log.Error("User " + userEmail + " is not logged in");
                    return res.ResponseToJson(); ;
                }
                else
                {
                    bc.joinBoard(userEmail, boardID);
                    Response<string> res = new Response<string>(null, null);
                    log.Info("User " + userEmail + " joined Board " + boardID + " successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }


        /// <summary>
        /// This method removes a user from the members list of a board.
        /// </summary>
        /// <param name="userEmail">The email of the user. Must be logged in</param>
        /// <param name="boardID">The board's ID</param>
        /// <returns>The standard json string format, original ReturnValue null</returns>
        public string leaveBoard(string userEmail, int boardID)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    log.Error("User " + userEmail + " is not logged in");
                    return res.ResponseToJson(); ;
                }
                else
                {
                    bc.leaveBoard(userEmail, boardID);
                    Response<string> res = new Response<string>(null, null);
                    log.Info("User " + userEmail + " left Board " + boardID + " successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }


        /// <summary>
        /// This method transfers a board ownership.
        /// </summary>
        /// <param name="currentOwnerEmail">Email of the current owner. Must be logged in</param>
        /// <param name="newOwnerEmail">Email of the new owner</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>The standard json string format, original ReturnValue null</returns>
        public string transferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            try
            {
                if (!uc.isLoggedIn(currentOwnerEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    log.Error("User " + currentOwnerEmail + " is not logged in");
                    return res.ResponseToJson(); ;
                }
                else
                {
                    bc.transferOwnership(currentOwnerEmail, newOwnerEmail, boardName);
                    Response<string> res = new Response<string>(null, null);
                    log.Info("User " + currentOwnerEmail + " trasnered ownership of Board " + boardName + " to user" + newOwnerEmail + " successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }

        /// <summary>
        /// Load all data
        /// </summary>
        /// <returns>The standard json string format, original ReturnValue null</returns>
        internal Response<string> LoadData()
        {
            try
            {
                if (!uc.IsLoaded)
                {
                    uc.LoadUsers();
                }
                bc.LoadBoards();
                Response<string> res = new Response<string>(null, null);
                log.Info("Boards loaded successfully");
                return res;
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res;
            }
        }

        ///<summary>
        ///delete all data.
        ///</summary>
        /// <returns>The standard json string format, original ReturnValue null</returns>
        internal Response<string> DeleteData()
        {
            try
            {
                bc.DeleteData();
                Response<string> res = new Response<string>(null, null);
                log.Info("Delete Data done successfully");
                return res;
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error("error in deletiting data");
                return res;
            }

        }

        /// <summary>
        /// This method returns a board
        /// </summary>
        /// <param name="userEmail">The user's Email</param>
        /// <param name="boardName">The board's name</param>
        /// <returns> The standard json string format, original ReturnValue - Board </returns>
        public Response<BoardToFront> getBoard(string userEmail, string boardName)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<BoardToFront> res = new Response<BoardToFront>("User is not logged in", null);
                    log.Error("User " + userEmail + " is not logged in");
                    return res; 
                }
                else
                {
                    BoardToFront toReturn = new BoardToFront(bc.getBoardByName(userEmail, boardName));
                    toReturn.setColBoard();
                    Response<BoardToFront> res = new Response<BoardToFront>(null, toReturn);
                    log.Info("Board was returned successfully");
                    return res;
                }
            }
            catch (Exception ex)
            {
                Response<BoardToFront> res = new Response<BoardToFront>(ex.Message, null);
                log.Error(ex.Message);
                return res;
            }
        }

        /// <summary>
        /// This method get a list of all the columnToFront's tasks
        /// </summary>
        /// <param name="userEmail">Email of user. </param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="column">The column number</param>
        /// <returns> The standard json string format, original ReturnValue - a tasksToFront list </returns>
        public string getColumnToFrontTasks(string userEmail, string boardName, int columnNum)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<string> res = new Response<string>("User is not logged in", null);
                    return res.ResponseToJson();
                }
                else
                {
                    ColumnToFront column = new ColumnToFront(bc.getColumnObj(userEmail, boardName, columnNum));
                    List<TaskToFront> tasks = new List<TaskToFront>();
                    foreach (TaskToFront taskTF in column.TaskList)
                    {
                        tasks.Add(taskTF);
                    }
                    Response<List<TaskToFront>> res = new Response<List<TaskToFront>>(null, tasks);
                    log.Info("A list of the columnToFront's tasks was returned successfully");
                    return res.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                return res.ResponseToJson();
            }
        }

        public Response<List<string>> getUserBoardsNames(string userEmail)
        {
            try
            {
                if (!uc.isLoggedIn(userEmail))
                {
                    Response<List<string>> res = new Response<List<string>>("User is not logged in", null);
                    log.Error("User " + userEmail + " is not logged in");
                    return res;
                }
                else
                {
                    List<string> namesList = bc.getUserBoardsNames(userEmail);          
                    Response<List<string>> res = new Response<List<string>>(null, namesList);
                    log.Info("get the boards names was done successfully");
                    return res;
                }
            }
            catch (Exception ex)
            {
                Response<List<string>> res = new Response<List<string>>(ex.Message, null);
                log.Error(ex.Message);
                return res;
            }
        }
    }
}

