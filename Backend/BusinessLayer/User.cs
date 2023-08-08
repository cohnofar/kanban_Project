using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOS;
using log4net;
using System.Reflection;
using System.IO;
using log4net.Config;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class User
    {
        private string email;
        private Password password;
        public bool loggedIn;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal UserDTO userDalDTO;

        public bool LoggedIn { get { return loggedIn; } set { loggedIn = value; } }
        public string Email { get { return email; } }



        public User(string email, Password password)
        {
            this.email = email;
            this.password = password;
            this.loggedIn = false;
            this.userDalDTO = new UserDTO(-1, email, password.Code);
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        internal User(UserDTO dto)
        {
            this.userDalDTO = dto;
            this.email = dto.Email;
            this.password = new Password(dto.Password);
            this.loggedIn = false;
            this.userDalDTO.IsPersistent = true;
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        /// <summary>
        /// Logging in the user to the system
        /// </summary>
        /// <param name="password">The password of this user</param>
        public void logIn (string password)
        {
            if (password == null || password.Equals(""))
            {
                log.Error($"error in logIn {email} with password {password}");
                throw new Exception("password is invalid");
            }
            if (loggedIn)
            {
                log.Error($"error in logIn because {email} is already logged in");
                throw new Exception("user is already logged in");
            }
            if (!this.password.equals(password))
            {
                log.Error($"error in logIn because {password} is not the user password");
                throw new Exception("password is invalid");
            }
            LoggedIn = true;
            log.Info("user loged in");
        } 

    }
}
