using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackendTests
{
    internal class UserTest
    {
        private readonly UserService userService;

        public UserTest(UserService userService)
        {
            this.userService = userService;
        }

        ///<summary>
        ///This function tests Requirement 7
        ///</summary>
        public void CreateUserTests()
        {
            //tests valid input
            string json = userService.createUser("amit@gmail.com", "Ah1236547");
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected" + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected: User added successfully");
            }

            // tests invalid input- illegal password (no uppercase letter)
            json = userService.createUser("gili@gmail.com", "h1236547");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with invalid password was created");
            }

            // tests invalid input- illegal password (no lowercase letter)
            json = userService.createUser("shimon@gmail.com", "A1236547");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with invalid password was created");
            }

            // tests invalid input- illegal password (no number)
            json = userService.createUser("alon@gmail.com", "Ahhskcud");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with invalid password was created");
            }

            // tests invalid input- illegal password (less than 6 characters)
            json = userService.createUser("sima@gmail.com", "Ah123");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with invalid password was created");
            }

            // tests invalid input- illegal password (over 20 characters)
            json = userService.createUser("lior@gmail.com", "Ahjkjekslss123465665546");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with invalid password was created");
            }

            // tests invalid input- email already exists
            json = userService.createUser("amit@gmail.com", "Sf1236547");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with email that already exists was added");
            }
            // tests invalid input- email invalid
            json = userService.createUser("amit", "Sf1236547");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with invalid email was added");
            }
            // tests invalid input- email invalid
            json = userService.createUser("amit%$#@gmail.com", "Sf1236547");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with invalid email was added");
            }
            // tests invalid input- email invalid
            json = userService.createUser("@gmail.com", "Sf1236547");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with invalid email was added");
            }
            // tests invalid input- email invalid
            json = userService.createUser("gmail.com", "Sf1236547");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with invalid email was added");
            }
            // tests invalid input- email invalid
            json = userService.createUser("nofar@gmail", "Sf1236547");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with invalid email was added");
            }
            // tests invalid input- email invalid
            json = userService.createUser("", "Sf1236547");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with invalid email was added");
            }
            json = userService.createUser(null, "Sf1236547");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as exepected: User with invalid email was added");
            }
        }

        ///<summary>
        ///This function tests Requirement 8
        ///</summary>
        public void LogoutTests()
        {
            //Tests valid input
            userService.createUser("ariel@gmail.com", "Ah1236547");
            string json = userService.logout("ariel@gmail.com");
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("Not as expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("As expected: User added successfully");
            }


            //Tests invalid input - user is not logged in (we have logged out)            
            json = userService.logout("ariel@gmail.com");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected: Logout was successfull");
            }


            //Tests invalid input - email is empty
            json = userService.logout("");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("As expected: Error - " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("Not as expected: Logout was successfull");
            }
        }

        ///<summary>
        ///This function tests Requirement 8
        ///</summary>
        public void LogInTest()
        {
            //tests valid input
            string json = userService.createUser("nofar@gmail.com", "Ah1236547");
            Dictionary<string, Object> ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("///not as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("as expected- User logged In ");
            }

            //tests unvalid input- uesr that logged in twice
            json = userService.login("nofar@gmail.com", "Ah1236547");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("not as expected- User logged In ");
            }

            userService.logout("nofar@gmail.com");
            //tests unvalid input- error password
            json = userService.login("nofar@gmail.com", "1236547");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("not as expected- User logged In ");
            }
            //tests unvalid input- null password
            json = userService.login("nofar@gmail.com", "");
            ans = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
            if (ans.ContainsKey("ErrorMessage"))
            {
                Console.WriteLine("as expected- Error: " + ans["ErrorMessage"]);
            }
            else
            {
                Console.WriteLine("not as expected- User logged In");
            }

        }
    }
}