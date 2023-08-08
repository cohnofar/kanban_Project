using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.Objects;

namespace Frontend.Model
{
    public class BackendController
    {
        private ServiceFactory serviceFactory { get; set; }

        public BackendController(ServiceFactory serviceFactory, BoardService boardService, TaskService taskService, UserService userService)
        {
            this.serviceFactory = serviceFactory;
        }

        public BackendController()
        {
            this.serviceFactory = new ServiceFactory();
            //serviceFactory.LoadData();
        }

        /// <summary>
        /// This method activates "Login" function in service layer.
        /// </summary>
        /// <returns>A UserModel of the logged in user, throws message in case of an error</returns>
        public UserModel login(string email, string password)
        {
            string json = serviceFactory.userService.login(email, password);
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                throw new Exception(ans["ErrorMessage"].ToString());
            }
            return new UserModel(this, email);
        }

        /// <summary>
        /// This method activates "LoadData" function from service layer
        /// </summary>
        public void LoadData()
        {
            string json = serviceFactory.LoadData();
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                throw new Exception(ans["ErrorMessage"].ToString());
            }
        }

        /// <summary>
        /// This method activates "DeleteData" function from service layer
        /// </summary>
        public void DeleteData()
        {
            string json = serviceFactory.DeleteData();
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                throw new Exception(ans["ErrorMessage"].ToString());
            }
        }

        ///<summary>This method registers a new user to the system.</summary>
        ///<param name="email">the user e-mail address, used as the username for logging the system.</param>
        ///<param name="password">the user password.</param>
        public UserModel register(string userEmail, string password)
        {
            string json = serviceFactory.userService.createUser(userEmail, password);
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                throw new Exception(ans["ErrorMessage"].ToString());
            }
            return new UserModel(this, userEmail);
        }

        ///<summary>This method gets an email and returns all its related boards.</summary>
        ///<param name="email">the user e-mail address, used as the username for logging the system.</param>
        /// <returns>A List of al the boards names related to the user,throws message in case of an error</returns>
        public List<string> getUserBoardsNames(string email)
        {
            Response<List<string>> res = serviceFactory.boardService.getUserBoardsNames(email);
            if (res.ErrorMessage != null)
            {
                throw new Exception(res.ErrorMessage);
            }
            else
            {
                return res.ReturnValue;
            }
        }

        /// <summary>
        /// gets a service layer board
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>A board, throws message in case of an error</returns>
        public BoardToFront getBoard(string userEmail, string boardName)
        {
            Response<BoardToFront> res = serviceFactory.boardService.getBoard(userEmail, boardName);
            if (res.ErrorMessage != null)
            {
                throw new Exception(res.ErrorMessage);
            }
            else
            {
                return (res.ReturnValue);
            }
        }

        /// <summary>
        /// Returns a List of column Tasks
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnNum">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A List of tasks, throws message in case of an error</returns>
        public List<TaskToFront> getColumn(string userEmail, string boardName, int columnNum)
        {
            string json = serviceFactory.boardService.getColumnToFrontTasks(userEmail, boardName, columnNum);
            Dictionary<string, List<TaskToFront>> ans = JsonSerializer.Deserialize<Dictionary<string, List<TaskToFront>>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                throw new Exception(ans["ErrorMessage"].ToString());
            }
            else
            {
                return (ans["ReturnValue"]);
            }
        }
    }
}
