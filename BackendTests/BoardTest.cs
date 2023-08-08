using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.Objects;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskService = IntroSE.Kanban.Backend.ServiceLayer.TaskService;


namespace BackendTests
{
    internal class BoardTest
    {

        private readonly BoardService boardService;
        private readonly UserService userService;
        private readonly TaskService taskService;


        public BoardTest(BoardService board, UserService user, TaskService task)
        {
            this.boardService = board;
            this.userService = user;
            this.taskService = task;
        }

        ///<summary>
        ///This function tests Requirement 12
        ///</summary>
        public void AddTaskTests()
        {
            userService.createUser("nira@gmail.com", "Ah52369");
            boardService.addBoard("nira@gmail.com", "Board1");

            //Tests a valid input
            string json = boardService.createTask("nira@gmail.com", "Board1", new DateTime(2023, 2, 26), "firstTask", "my first task"); // id 1
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected: Task added successfully");
            }

            //Tests unvalid input - title is empty
            json = boardService.createTask("nira@gmail.com", "Board1", new DateTime(2023, 2, 26), "", "my second task");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected: Task with empty title added successfully");
            }

            //Tests a valid input - description is empty
            json = boardService.createTask("nira@gmail.com", "Board1", new DateTime(2023, 2, 26), "thirdTask"); // id 2
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected: Task with empty description added successfully");
            }

            //Tests invalid input - title is invalid (over 50 characters)
            json = boardService.createTask("nira@gmail.com", "Board1", new DateTime(2023, 2, 5), "fifthTask1fifthTask1fifthTask1fifthTask1fifthTask1fi", "my fifth task");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected: Task with invalid title added successfully");
            }

            //Tests invalid input - description is invalid (over 300 characters)
            json = boardService.createTask("nira@gmail.com", "Board1", new DateTime(2023, 2, 5), "sixthTask", "very long descriptionvery long descriptionvery long descriptionvery long descriptionvery long descriptionvery long descriptionvery long descriptionvery long descriptionvery long descriptionvery long descriptionvery long descriptionvery long descriptionvery long descriptionvery long descriptionvery long description");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected: Task with invalid description added successfully");
            }

            //Tests unvaild input - user not logged in
            userService.createUser("jonathan@gmail.com", "Jf1526347");
            boardService.addBoard("jonathan@gmail.com", "Board1");
            userService.logout("jonathan@gmail.com");
            json = boardService.createTask("jonathan@gmail.com", "Board1", new DateTime(2023, 2, 26), "task1", "my description");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected: Task added successfully by a not logged in user");
            }
            //tests unvalid due date
            userService.login("jonathan@gmail.com", "Jf1526347");
            json = boardService.createTask("jonathan@gmail.com", "Board1", new DateTime(2014, 2, 26), "task1", "my description");
           
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected: Task added successfully but the due date is unvalid");
            }
        }

        ///<summary>
        ///This function tests Requirement 9
        ///</summary>
        public void AddBoardsTests()
        {
            // tests valid input
            userService.createUser("amit@walla.co.il", "Hmmm12569");
            string json = boardService.addBoard("amit@walla.co.il", "tryBoard1");
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected: Board added successfully");
            }

            // tests invalid input: board name already exists for user
            json = boardService.addBoard("amit@walla.co.il", "tryBoard1");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected: Board with existing name for user added successfully");
            }

            // tests valid input: board name already exists for other user, but not for current user
            userService.createUser("rona@walla.co.il", "Hmmm12569");
            json = boardService.addBoard("rona@walla.co.il", "tryBoard1");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected: Board added successfully");
            }
        }

        ///<summary>
        ///This function tests Requirement 10
        ///</summary>
        public void LimitTasksForColumnTests()
        {
            // tests valid input
            userService.createUser("yossi@walla.co.il", "Hmmm12569");
            boardService.addBoard("yossi@walla.co.il", "tryBoard");
            string json = boardService.limitTasksForColumn("yossi@walla.co.il", "tryBoard", 0, 50);
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected: Column maximum number of tasks limited");
            }

            // tests invalid input- limit the column maximum task number to less than the tasks that are already in the column
            boardService.createTask("yossi@walla.co.il", "tryBoard", new DateTime(2024, 5, 10), "task1"); //id 0
            taskService.assignTask("yossi@walla.co.il", "tryBoard", 0, 0, "yossi@walla.co.il");
            boardService.createTask("yossi@walla.co.il", "tryBoard", new DateTime(2024, 5, 10), "tas2"); // id 1
            taskService.assignTask("yossi@walla.co.il", "tryBoard", 0, 1, "yossi@walla.co.il");
            json = boardService.limitTasksForColumn("yossi@walla.co.il", "tryBoard", 0, 0);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected: Column maximum number of tasks limited");
            }
        }

        ///<summary>
        ///This function tests Requirement 16
        ///</summary>
        public void GetInProgressTests()
        {
            //Tests valid input
            userService.createUser("gili@walla.co.il", "Ah1236547");
            boardService.addBoard("gili@walla.co.il", "tryBoard");
            boardService.createTask("gili@walla.co.il", "tryBoard", new DateTime(2023, 8, 15), "task1", "description for task"); // id 0
            taskService.assignTask("gili@walla.co.il", "tryBoard", 0, 0, "gili@walla.co.il");
            boardService.advance("gili@walla.co.il", "tryBoard", 0, 0);
            boardService.addBoard("gili@walla.co.il", "tryBoard2");
            boardService.createTask("gili@walla.co.il", "tryBoard2", new DateTime(2023, 8, 15), "task2", "description for task"); // id 0
            taskService.assignTask("gili@walla.co.il", "tryBoard2", 0, 1, "gili@walla.co.il");
            boardService.advance("gili@walla.co.il", "tryBoard2", 0, 0);
            string json = boardService.getInProgress("gili@walla.co.il");
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected: User added successfully");
                Console.WriteLine(json);
            }


            //Tests invalid input - user is not logged in
            userService.createUser("natan@walla.co.il", "NjK123569");
            userService.logout("natan@walla.co.il");
            json = boardService.getInProgress("natan@walla.co.il");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected: In Progress list was returned");
            }
        }

        ///<summary>
        ///This function tests Requirement 13
        ///</summary>
        public void AdvanceTest()
        {
            //tests valid input
            userService.createUser("gili@gmail.com", "Ah1236547");
            boardService.addBoard("gili@gmail.com", "board1");
            boardService.createTask("gili@gmail.com", "board1", new DateTime(2022, 12, 12), "title1", "description for task");// id 0
            taskService.assignTask("gili@gmail.com", "board1", 0, 0, "gili@gmail.com");
            string json = boardService.advance("gili@gmail.com", "board1", 0, 0);
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- task was advanced");
            }
            //tests valid input- task was advanced again
            json = boardService.advance("gili@gmail.com", "board1", 1, 0);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- task was advanced");
            }
            //tests valid input- task was advanced for the 3rd time
            json = boardService.advance("gili@gmail.com", "board1", 2, 0);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- task was advanced");
            }

        }

        ///<summary>
        ///This function tests Requirement 9
        ///</summary>
        public void RemoveBoardTest()
        {
            userService.createUser("tomer@gmail.com", "Ah1236547");
            boardService.addBoard("tomer@gmail.com", "board1");
            string json = boardService.removeBoard("tomer@gmail.com", "board1");
            //tests valid input
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- board wad removed");
            }
            //test unvalid input- board that doesn't exist
            json = boardService.removeBoard("tomer@gmail.com", "board2");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- board was removed ");
            }
        }

        ///<summary>
        ///This function tests Requirement 11
        ///</summary>
        public void GetColumnLimitTests()
        {
            userService.createUser("adi@gmail.com", "Ah1236547");
            boardService.addBoard("adi@gmail.com", "board1");

            //tests valid input
            string json = boardService.getColumnLimit("adi@gmail.com", "board1", 0);
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- A column maximum number of task was limited by default");
                Console.WriteLine(json);
            }

            //tests valid input
            json = boardService.getColumnLimit("adi@gmail.com", "board1", 1);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- A column maximum number of task was limited by default");
                Console.WriteLine(json);

            }

            //tests valid input
            json = boardService.getColumnLimit("adi@gmail.com", "board1", 2);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- A column maximum number of task was limited by default");
                Console.WriteLine(json);
            }

            //tests valid input
            boardService.limitTasksForColumn("adi@gmail.com", "board1", 1, 5);
            json = boardService.getColumnLimit("adi@gmail.com", "board1", 1);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- A column maximum number of task was limited");
                Console.WriteLine(json);
            }
        }
        public void GetUserBoardsTests()
        {

            //tests uvalid input- try to get boards list of user that doesnt exist
            string json = boardService.getUserBoards("ariel@gmail.com");
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- get a board list ");
            }
            userService.createUser("ariel@gmail.com", "Ah1236547");

            //tests uvalid input- try to get boards list of user that is not member in any board
            json = boardService.getUserBoards("ariel@gmail.com");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- returned a empty list");

            }
            //teats valid input
            boardService.addBoard("ariel@gmail.com", "board1"); // id 0
            boardService.joinBoard("ariel@gmail.com", 0);
            json = boardService.getUserBoards("ariel@gmail.com");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- get a list of boards");
            }
        }
        public void GetBoardNameTest()
        {

            //tests uvalid input- try to get name of board that doesnt exist
            string json = boardService.getBoardName(1212);
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- get a board name ");
            }


            //test valid input
            userService.createUser("lilian@walla.co.il", "KjHu8907");
            boardService.addBoard("lilian@walla.co.il", "boardtry5");
            json = boardService.getBoardName(0);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- get the name of the board");
            }
        }
        public void JoinBoardTest()
        {
            //tests unvalid input - user that is not exist
            string json = boardService.joinBoard("dudi@gmail.com", 0);
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- user joined to the board ");
            }
            //tests unvalid input - user that is not logged in
            userService.createUser("dudi@gmail.com", "Ah1236547");
            userService.logout("dudi@gmail.com");
            json = boardService.joinBoard("dudi@gmail.com", 0);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- user joined to the board ");
            }
            //tests unvalid input - board is not exist
            userService.login("dudi@gmail.com", "Ah1236547");
            json = boardService.joinBoard("dudi@gmail.com", 1212);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- user joined to the board ");
            }
            //tests valid input
            boardService.addBoard("dudi@gmail.com", "board2"); // id 0
            userService.createUser("karina@gmail.com", "AfTg789065");
            json = boardService.joinBoard("karina@gmail.com", 0);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- user joined to the board");
            }
            //testd unvalid input - try to join to a board that he is already in
            json = boardService.joinBoard("dudi@gmail.com", 0);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- user joined to the board ");
            }

            //test invalid input- try to join a board with same name as a board the user is already a member in
            userService.createUser("beyonce@gmail.com", "Queen2356");
            boardService.addBoard("beyonce@gmail.com", "board2");
            json = boardService.joinBoard("dudi@gmail.com", 1);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- user joined to the board with the same name");
            }

        }
        public void LeaveBoardTest()
        {
            userService.createUser("nili@gmail.com", "Ah1236547");
            // tests unvalid input- user is not logged in
            string json = boardService.leaveBoard("nili@gmail.com", 0);
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- user leave the board");
            }
            // tests unvalid input- try to leave board that he is not member in
            userService.login("nili@gmail.com", "Ah1236547");
            json = boardService.leaveBoard("nili@gmail.com", 0);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- user leave the board");
            }
            // test valid input
            boardService.addBoard("nili@gmail.com", "board3");// id 0
            boardService.joinBoard("nili@gmail.com", 0);
            userService.createUser("lara@gmail.com", "Ah1236547");
            boardService.joinBoard("lara@gmail.com", 0);
            json = boardService.leaveBoard("lara@gmail.com", 0);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- user leave the board");
            }
        }
        public void TransferOwnershipTest()
        {
            userService.createUser("shay@gmail.com", "Ah1236547");
            boardService.addBoard("shay@gmail.com", "board4");// id 3
            userService.createUser("adam@gmail.com", "Ah1236547");
            // tests unvalid input - user that is not member in the board
            string json = boardService.transferOwnership("shay@gmail.com", "adam@gmail.com", "board4");
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- ownership was transfered");

            }

            // test valid input
            boardService.joinBoard("adam@gmail.com", 0);
            json = boardService.transferOwnership("shay@gmail.com", "adam@gmail.com", "board4");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected- ownership was transfered");
            }
        }
        public void GetColumnNameTest()
        {
            userService.createUser("liat@gmail.com", "Ah1236547");
            boardService.addBoard("liat@gmail.com", "board4");// id 0
            // tests valid input
            string json = boardService.getColumnName("liat@gmail.com", "board4", 0);
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected-didn't get the column name");
            }
            else
            {
                Console.WriteLine($"as expected- get the column name");

            }

            // test uvalid input

            json = boardService.getColumnName("liat@gmail.com", "boardnotexist", 0);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- column name was given but the board not exist");
            }

            json = boardService.getColumnName("liat@gmail.com", "board4", 17);
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected- column name was given but number is invalid");
            }
        }

        public void testGetBoard()
        {
            userService.createUser("roeyb@gmail.com", "Ah12569");
            boardService.addBoard("roeyb@gmail.com", "board10");
            Response<BoardToFront> res = boardService.getBoard("roeyb@gmail.com", "board10");
            Console.WriteLine(res.ReturnValue.BoardName);
            Console.WriteLine(res.ReturnValue.Columns);
            Console.WriteLine(res.ReturnValue.getColumnsList());

        }
        public void getUserBoardsNamesTest()
        {
            userService.createUser("roeyb1@gmail.com", "Ah12569");
            boardService.addBoard("roeyb1@gmail.com", "board10");
            boardService.addBoard("roeyb1@gmail.com", "board11");
            Response<List<string>> res = boardService.getUserBoardsNames("roeyb1@gmail.com");
            Console.WriteLine(res.ReturnValue.ToString());
        }

    }



}