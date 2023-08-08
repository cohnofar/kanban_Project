using System;
using System.Collections.Generic;
using log4net;
using log4net.Config;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using System.Reflection;
using System.IO;

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
    /// <para>
    /// The structure of the JSON of a Task, is:
    /// <code>
    /// {
    ///     "Id": &lt;int&gt;,
    ///     "CreationTime": &lt;DateTime&gt;,
    ///     "Title": &lt;string&gt;,
    ///     "Description": &lt;string&gt;,
    ///     "DueDate": &lt;DateTime&gt;
    /// }
    /// </code>
    /// </para>
    /// </summary>
    public class TaskService
    {
        private BoardController bc;
        private UserController uc;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public TaskService(BoardController bc, UserController uc)
        {
            
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            this.bc = bc;
            this.uc = uc;
        }

        /// <summary>
        /// This method updates task title.
        /// </summary>
        /// <param name="email">The email of the user </param>
        /// <param name="boardName">The name of the board </param>
        /// <param name="column">The Column of the task</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newTitle">New title for the task</param>
        /// <returns>The standard json string format, original ReturnValue - null </returns>
        public string editTitle(string email, string boardName, int column, int taskId, string newTitle)
        {
            try
            {
                if (!uc.getUser(email).loggedIn)
                {
                    Response<string> res =new Response<string>("user is not logged in", null);
                    log.Error("user is not logged in");
                    return res.ResponseToJson();
                }
                else
                {
                    bc.updateTaskTitle(email, boardName, column, taskId, newTitle);
                    Response<string> res = new Response<string>(null, null);
                    log.Info("Title was edit successfully");
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
        /// This method updates the due date of a task
        /// </summary>
        /// <param name="email">The email of the user </param>
        /// <param name="boardName">The name of the board </param>
        /// <param name="column">The Column of the task</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDate">The new due date of the column</param>
        /// <returns>The standard json string format, original ReturnValue - null </returns>
        public string editDueDate(string email, string boardName, int column, int taskId, DateTime newDate)
        {
                try
                {
                    if (!uc.getUser(email).loggedIn)
                    {
                    Response<string> res = new Response<string>("user is not logged in", null);
                        log.Error("user is not logged in");
                        return res.ResponseToJson();
                    }
                    else
                    {

                    bc.updateTaskDueDate(email, boardName, column, taskId, newDate);
                    Response<string> res = new Response<string>(null, null);
                        log.Info("Due Date was edit successfully");
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
        /// This method updates the description of a task.
        /// </summary>
        /// <param name="email">The email of the user </param>
        /// <param name="boardName">The name of the board </param>
        /// <param name="column">The Column of the task</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDescription">New description for the task</param>
        /// <returns>The standard json string format, original ReturnValue - null </returns>
        public string editDescription(string email, string boardName, int column, int taskId, string newDescription)
        {
            try
            {
                if (!uc.getUser(email).loggedIn)
                {
                    Response<string> res = new Response<string>("user is not logged in", null);
                    log.Error("user is not logged in");
                    return res.ResponseToJson();
                }
                else
                {
                    bc.updateTaskDescription(email, boardName, column, taskId, newDescription);
                    Response<string> res = new Response<string>(null, null);
                    log.Info("Description was edit successfully");
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
        /// This method assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="column">The column number. The first column is 0, the number increases by 1 for each column</param>
        /// <param name="taskID">The task to be updated identified a task ID</param>        
        /// <param name="emailAssignee">Email of the asignee user</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string assignTask(string email, string boardName, int column, int taskID, string emailAssignee)
        {
            try
            {
                if (!uc.getUser(email).loggedIn)
                {
                    Response<string> res = new Response<string>("user is not logged in", null);
                    log.Error("user is not logged in");
                    return res.ResponseToJson();
                }
                else
                {
                    bc.assignTask(email, boardName, column, taskID, emailAssignee);
                    Response<string> res = new Response<string>(null, null);
                    log.Info("assignee was added successfully");
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

    }
}
