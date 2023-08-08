using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.BusinessLayer;
using TaskService = IntroSE.Kanban.Backend.ServiceLayer.TaskService;

namespace BackendTests
{
    internal class MainClass
    {
        static public void Main(String[] args)
        {
            UserController uc = new UserController();
            BoardController bc = new BoardController();

            IntroSE.Kanban.Backend.ServiceLayer.UserService userService = new IntroSE.Kanban.Backend.ServiceLayer.UserService(uc);
            IntroSE.Kanban.Backend.ServiceLayer.BoardService boardService = new IntroSE.Kanban.Backend.ServiceLayer.BoardService(bc, uc);
            TaskService taskService = new IntroSE.Kanban.Backend.ServiceLayer.TaskService(bc, uc);

            UserTest userTest = new UserTest(userService);
            BoardTest boardTest = new BoardTest(boardService, userService, taskService);
            TaskTest taskTest = new TaskTest(userService, boardService, taskService);
            ServiceFactory sf = new ServiceFactory(uc, bc);

            Console.WriteLine("delete data check");
            sf.DeleteData();

            Console.WriteLine("User Service Tests");
            //userTest.CreateUserTests();
            //userTest.LogInTest();
            //userTest.LogoutTests();

            Console.WriteLine("Board Service Tests");
            //boardTest.AddBoardsTests();
            //boardTest.RemoveBoardTest();
            //boardTest.LimitTasksForColumnTests();
            //boardTest.AddTaskTests();
            //boardTest.GetInProgressTests();
            //boardTest.AdvanceTest();
            //boardTest.GetUserBoardsTests();
            //boardTest.TransferOwnershipTest();
            //boardTest.JoinBoardTest();
            //boardTest.GetBoardNameTest();
            //boardTest.GetColumnLimitTests();
            //boardTest.LeaveBoardTest();
            //boardTest.GetColumnNameTest();
            //boardTest.testGetBoard();
            boardTest.getUserBoardsNamesTest();
            Console.WriteLine("Task Service Tests");
            //taskTest.EditTitleTests();
            //taskTest.EditDueDateTest();
            //taskTest.EditDescriptionTests();

            Console.WriteLine("Ww are the bestttttttttttttttttttttt");
        }
    }
}