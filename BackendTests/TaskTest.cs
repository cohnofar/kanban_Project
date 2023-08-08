using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskService = IntroSE.Kanban.Backend.ServiceLayer.TaskService;

namespace BackendTests
{
    internal class TaskTest
    {
        private readonly UserService userService;
        private readonly BoardService boardService;
        private readonly TaskService taskService;
        private int taskTestCounter;

        public TaskTest(UserService user, BoardService board, TaskService task)
        {
            this.userService = user;
            this.boardService = board;
            this.taskService = task;
            this.taskTestCounter = 0;
        }

        ///<summary>
        ///This function tests Requirements 14,15
        ///</summary>
        public void EditTitleTests()
        {
            //Tests valid input
            userService.createUser("achiatheking@walla.co.il", "Ah1236547");
            boardService.addBoard("achiatheking@walla.co.il", "tryBoard");
            boardService.createTask("achiatheking@walla.co.il", "tryBoard", new DateTime(2023, 8, 15), "task1", "description for task"); // id 0
            taskService.assignTask("achiatheking@walla.co.il", "tryBoard",0,0, "achiatheking@walla.co.il");
            string json = taskService.editTitle("achiatheking@walla.co.il", "tryBoard",0 ,0, "trychangetitle");
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Task title was changed successfully");
            }
            taskTestCounter++;

            //Tests invalid input - Task does'nt exist
            json = taskService.editTitle("achiatheking@walla.co.il", "tryBoard",1, 10, "trychangedescription2"); //Assume taskk 10 doesnt exist
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Error didn't occur");
            }
            taskTestCounter++;

            //Tests invalid input - Task title is empty
            json = taskService.editTitle("achiatheking@walla.co.il", "tryBoard", 0, 0, "");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Task title was changed");
            }
            taskTestCounter++;

            //Tests invalid input - Task title is over 50 characters
            json = taskService.editTitle("achiatheking@walla.co.il", "tryBoard", 0, 0, "012345678901234567890123456789012345678901234567890123456789"); //Assume task ID is 234
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Task title was changed");
            }
            taskTestCounter++;

            //Tests invalid input - Task is done
            boardService.advance("achiatheking@walla.co.il", "tryBoard", 0, 0);
            boardService.advance("achiatheking@walla.co.il", "tryBoard", 1, 0);
            json = taskService.editTitle("achiatheking@walla.co.il", "tryBoard", 0, 0, "itdoesntmatter");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Task title was changed");
            }
            taskTestCounter++;
        }

        ///<summary>
        ///This function tests Requirements 14,15
        ///</summary>
        public void EditDescriptionTests()
        {
            //Tests valid input
            userService.createUser("yehudabestmetargel@walla.co.il", "Ah1236547");
            boardService.addBoard("yehudabestmetargel@walla.co.il", "tryBoard");
            boardService.createTask("yehudabestmetargel@walla.co.il", "tryBoard", new DateTime(2023, 8, 15), "task1", "description for task"); // id 0
            taskService.assignTask("yehudabestmetargel@walla.co.il", "tryBoard", 0, 0, "yehudabestmetargel@walla.co.il");
            string json = taskService.editDescription("yehudabestmetargel@walla.co.il", "tryBoard", 0, 0, "trychangedescription"); 
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Task description was changed successfully");
            }
            taskTestCounter++;

            //Tests valid input - Task description is empty
            json = taskService.editDescription("yehudabestmetargel@walla.co.il", "tryBoard", 0, 0, ""); 
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Task description was deleted successfully");
            }
            taskTestCounter++;

            //Tests invalid input - Task does'nt exist
            json = taskService.editDescription("yehudabestmetargel@walla.co.il", "tryBoard", 0, 3, "trychangedescription2"); 
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Error - " + ans["ErrorMessage"]);
            }//
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Error didn't occur");
            }
            taskTestCounter++;

            //Tests invalid input - Task description is over 300 characters
            json = taskService.editTitle("yehudabestmetargel@walla.co.il", "tryBoard", 0, 0, "012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789"); //Assume task ID is 234
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Task title was changed");
            }
            taskTestCounter++;

            //Tests invalid input - Task is done
             boardService.advance("yehudabestmetargel@walla.co.il", "tryBoard", 0, 0);
            boardService.advance("yehudabestmetargel@walla.co.il", "tryBoard", 1, 0);
            json = taskService.editDescription("yehudabestmetargel@walla.co.il", "tryBoard",2, 0, "itdoesntmatter");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Task title was changed");
            }
            taskTestCounter++;
        }

        ///<summary>
        ///This function tests Requirements 14,15
        ///</summary>
        public void EditDueDateTest()
        {
            userService.createUser("ariana@gmail.com", "Ah1236547");
            boardService.addBoard("ariana@gmail.com", "board1");
            boardService.createTask("ariana@gmail.com", "board1", new DateTime(2022, 12, 12), "title1", "description for task");// id 0
            taskService.assignTask("ariana@gmail.com", "board1", 0, 0, "ariana@gmail.com");
            //tests valid input
            string json = taskService.editDueDate("ariana@gmail.com", "board1", 0, 0, new DateTime(2022, 12, 13));
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Due Date was updated");
            }
            taskTestCounter++;
            //test unvalid duedate
            json = taskService.editDueDate("ariana@gmail.com", "board1", 0, 0, new DateTime(2014, 12, 13));
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " not As expected: Due Date was updated but the due date is unvalid");
            }
            taskTestCounter++;


            //tests unvalid input-  editig task that in the 3rd column
            boardService.advance("ariana@gmail.com", "board1", 0, 0);
            boardService.advance("ariana@gmail.com", "board1", 1, 0);
            json = taskService.editDueDate("ariana@gmail.com", "board1", 2, 0, new DateTime(2022, 12, 18));
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Due date was updated ");
            }
            taskTestCounter++;
        }
        public void AssignTaskTest()
        {
            userService.createUser("liel@gmail.com", "Ah1236547");
            boardService.addBoard("liel@gmail.com", "board1");// id 0
            boardService.joinBoard("liel@gmail.com", 0);
            boardService.createTask("liel@gmail.com", "board1", new DateTime(2022, 12, 12), "title1", "description for task");// id 0?
            //tests valid input- the task creator try to assign himself
            string json = taskService.assignTask("liel@gmail.com", "board1", 0, 0, "liel@gmail.com");
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: user has assign to the task");
            }
            taskTestCounter++;

            userService.createUser("yarden@gmail.com", "Ah1236547");
            boardService.joinBoard("yarden@gmail.com", 0);
            boardService.createTask("yarden@gmail.com", "board1", new DateTime(2022, 12, 12), "title2", "description for task");// id 1?
            //tests valid input- the task creator try to assign othermember in the board
            json = taskService.assignTask("yarden@gmail.com", "board1", 0, 1, "liel@gmail.com");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: user has assign to the task");
            }
            taskTestCounter++;

            //tests valid input- the assignee replace he assign
            json = taskService.assignTask("liel@gmail.com", "board1", 0, 1, "yarden@gmail.com");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: user has assign to the task");
            }
            taskTestCounter++;

            //tasts unvalid input- the creator try to assign user that in not member in the board
            userService.createUser("tomer@gmail.com", "Ah1236547");
            boardService.createTask("yarden@gmail.com", "board1", new DateTime(2022, 12, 12), "title3", "description for task");// id 2?
            json = taskService.assignTask("yarden@gmail.com", "board1", 0, 2, "tomer@gmail.com");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected - assignee has changed ");
            }
            taskTestCounter++;

            //tasts unvalid input- try to assign task that already assigned
            json = taskService.assignTask("liel@gmail.com", "board1", 0, 1, "liel@gmail.com");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Test number " + taskTestCounter + " As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Test number " + taskTestCounter + " Not as expected - assignee has changed ");
            }
            taskTestCounter++;
        }

    }
}