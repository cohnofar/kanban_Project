using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using System.IO;
using log4net.Config;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Password
    {
        private string code;
        readonly int MIN_LENGTH=6;
        readonly int MAX_LENGTH=20;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal string Code { get { return code; } }

        public Password(string password)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            if (!validPass(password))
            {
                log.Error($"error in validPass {password} is invalid");
                throw new Exception("password is invalid");
            }
            this.code = password;
            
        }

        /// <summary>
        /// Check if the password answer the requirements of password structure
        /// </summary>
        /// <param name="password">A password we should check</param>
        /// <returns>True if the password answer the requirements of password structure, False if not</returns>
        private bool validPass(string password) {
            bool ans = true;
            if (password == null || password.Equals(""))
            {
                ans = false;
            }
            if (ans&&password.Length > MAX_LENGTH | password.Length < MIN_LENGTH)
            {
                ans= false;
            }
            int countUp = 0;
            int countLow = 0;
            int numCount = 0;
            for (int i = 0; ans&&(i < password.Length); i++)
            {
                if (password[i] >= 'A' & password[i] <= 'Z')
                    countUp++;
                if (password[i] >= 'a' & password[i] <= 'z')
                    countLow++;
                if (password[i] >= '0' & password[i] <= '9')
                    numCount++;
            }
            if ((countUp == 0) | (countLow == 0) | (numCount == 0))
            {
                ans= false;
            }
            return ans;
        }

        /// <summary>
        /// Check if the given password equals to this password
        /// </summary>
        /// <param name="pass">The password we should match with</param>
        /// <returns>True if the password is equal to an input password, False if not</returns>
        public bool equals(string pass)
        {
            if (pass == null || pass.Equals(""))
            {
                return false;
            }
            return code.Equals(pass);
        }
    }
}
