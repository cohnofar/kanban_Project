using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOS;
using log4net;
using System.Reflection;
using System.IO;
using log4net.Config;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    
    public class UserController
    {
        private Dictionary<string, User> users;
        private UserMapper userMapper;
        private Boolean isLoaded = false;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public Boolean IsLoaded { get { return isLoaded; } set { isLoaded = value; } }
        
        public UserController()
        {
            this.users = new Dictionary<string, User>();
            this.userMapper = new UserMapper();
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
        /// <summary>
        /// Loading all users from the DB into the local memory. 
        /// </summary>
        public void LoadUsers()
        {
            Dictionary<string, UserDTO> usersDTOs = userMapper.LoadUsers();
            foreach (string email in usersDTOs.Keys)
            {
                User user = new User(usersDTOs[email]);
                users[email] = user;
            }
            IsLoaded = true;
        }


        /// <summary>
        /// Deleting all users from the DB into the local memory. 
        /// </summary>
        public void DeleteData()
        {
            try
            {
                userMapper.DeleteAll();
            }
            catch (Exception ex)
            {
                log.Warn(ex);
            }


        }


        /// <summary>
        /// Registers a new user to the system.
        /// </summary>
        /// <param name="email">the user email address, used as the username for logging the system.</param>
        /// <param name="password">the user password.</param>
        public void createUser(string email, string password)
        {
            if (email == null|| email.Equals("")|| password == null|| password.Equals(""))
            {
                log.Error("error in createUser -password or email is empty or null");
                throw new ArgumentException("password or email is invalid");
            }
            if (!checkEmail(email))
            {
                log.Error($"error in createUser -email {email} is invalid");
                throw new Exception("email is invalid");
            }
            Password pass = new Password(password);
            if (emailExists(email) == true)
            {
                log.Error($"error in createUser -email {email} is exist");
                throw new Exception("email is exist");
            }
            User user = new User(email, pass);
            user.LoggedIn = true;
            try
            {
                user.userDalDTO.addUserToDAL();
                user.userDalDTO.IsPersistent = false;
            }
            catch (Exception)
            {
                log.Warn($"Failed to Register the user '{email}'");
                throw new Exception($"Failed to Register for '{email}' with password '{password}'");
            }
            users.Add(user.Email, user);
            user.userDalDTO.IsPersistent = true;
            log.Info($"User '{email}' registered successfully");
        }

        /// <summary>
        /// Checks if the email answers the requirements of an email adress structure
        /// </summary>
        /// <param name="email">Email to check if is legal</param>
        /// <returns>True if the email answers the requirements of an email adress structure, False if not</returns>
        private bool checkEmail (string email)
        {
            bool ans = true;
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Regex regex2 = new Regex(@"^\w+([.-]?\w+)@\w+([.-]?\w+)(.\w{2,3})+$");
            Regex regex3 = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            Regex regex4 = new Regex(@"^[a-zA-Z0-9_!#$%&'*+/=?`{|}~^.-]+@[a-zA-Z0-9.-]+$");
            if (!(regex.IsMatch(email))|!(regex2.IsMatch(email))|!(regex4.IsMatch(email)) | !(regex3.IsMatch(email)))
            {
                ans = false;
            }
          
            var trimmedEmail = email.Trim();

            if (ans)
            {
                if (trimmedEmail.EndsWith("."))
                {
                    return false; // suggested by @TK-421
                }
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == trimmedEmail;
                }
                catch
                {
                    return false;
                }
            }
        return ans;
        }

        /// <summary>
        /// Checks if this email already exists in the user dictionary
        /// </summary>
        /// <param name="email">Email to check if exsits</param>
        /// <returns>True if the email exists, False if email doesn't exsits</returns>
        public bool emailExists(string email)
        {
            if (email == null || email.Equals(""))
            {
                throw new ArgumentException("email is invalid");
            }
            if (users.ContainsKey(email))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get a user object according to the given email
        /// </summary>
        /// <param name="email">Email of the user we want</param>
        /// <returns>A user object with the given email</returns>
        public User getUser(string email)
        {
            if (email == null || email.Equals(""))
            {
                throw new ArgumentException("email is invalid");
            }
            if (emailExists(email) == false)
            {
                throw new Exception("user doesn't exist");
            }
            return users[email];  
        }

        /// <summary>
        /// Logging out the user from the system
        /// </summary>
        /// <param name="email">email of logged in user</param>
        public void logOut(string email)
        {
            if (email == null || email.Equals(""))
            {
                log.Error("error in logOut - email is empty or null");
                throw new ArgumentException("email is invalid");
            }
            if (isLoggedIn(email) == false)
            {
                log.Error($"error in logOut - email {email} is already logged out");
                throw new Exception("user is already logged out");
            }
            this.getUser(email).loggedIn = false;
        }

        /// <summary>
        /// Checks if the user that has the given email is logged in to the system
        /// </summary>
        /// <param name="email">email of logged in user</param>
        /// <returns>True if user is indeed logged in, False if not</returns>
        public bool isLoggedIn(string email)
        {
            if (email == null || email.Equals(""))
            {
                throw new ArgumentException("email is invalid");
            }
            return this.getUser(email).loggedIn;
        }
        
    }
}
