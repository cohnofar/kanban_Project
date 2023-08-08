using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using log4net.Config;


namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// <para>
    /// A class for creating all Service Layer objects, and activating Load and Delete data from DB to all of the system. 
    /// The class methods return a string.
    /// </para>
    /// </summary>
    public class ServiceFactory
    {
        public UserService userService;
        public TaskService taskService;
        public BoardService boardService;
        private UserController userController;
        private BoardController boardController;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public ServiceFactory()
        {
            userController = new UserController(); 
            boardController = new BoardController();
            userService = new UserService(userController);
            taskService = new TaskService(boardController, userController);
            boardService = new BoardService(boardController, userController);
        }


        public ServiceFactory(UserController uc, BoardController bc)
        {
            userController = uc;
            boardController = bc;
            userService = new UserService(userController);
            taskService = new TaskService(boardController, userController);
            boardService = new BoardService(boardController, userController);
        }

        /// <summary>
        /// This method activates all "DeleteData" functions from the different Service Layer classes
        /// </summary>
        /// <returns>The standard json string format, original ReturnValue null</returns>
        public string DeleteData()
        {
            Response<string> userRes = userService.DeleteData();
            Response<string> boardRes = boardService.DeleteData();
            Response<string> res;
            if (boardRes.ErrorMessage != null && userRes.ErrorMessage != null)
            {
                res = new Response<string>(boardRes.ErrorMessage +"\n"+ userRes.ErrorMessage, null);
            }
            else if (boardRes.ErrorMessage != null)
            {
                res = new Response<string>(boardRes.ErrorMessage, null);
            }
            else if (userRes.ErrorMessage != null)
            {
                res = new Response<string>(userRes.ErrorMessage, null);
            }
            else
            {
                res = new Response<string>(null, null);
            }
            return res.ResponseToJson();
        }

        /// <summary>
        /// This method activates all "LoadData" functions from the different Service Layer classes
        /// </summary>
        /// <returns>The standard json string format, original ReturnValue null</returns>
        public string LoadData()
        {
            Response<string> userRes = userService.LoadUsers();
            Response<string> boardRes = boardService.LoadData();
            Response<string> res;
            if (boardRes.ErrorMessage != null && userRes.ErrorMessage != null)
            {
                res = new Response<string>(boardRes.ErrorMessage + "\n" + userRes.ErrorMessage, null);
            }
            else if (boardRes.ErrorMessage != null)
            {
                res = new Response<string>(boardRes.ErrorMessage, null);
            }
            else if (userRes.ErrorMessage != null)
            {
                res = new Response<string>(userRes.ErrorMessage, null);
            }
            else
            {
                res = new Response<string>(null, null);
            }
            return res.ResponseToJson();
        }
    }
}
