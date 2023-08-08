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
    ///</summary>
    public class UserService
    {
        private UserController uc;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UserService(UserController uc)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            this.uc = uc;
        }

        /// <summary>
        /// This method registers a new user to the system.
        /// </summary>
        /// <param name="email">The user email address, used as the username for logging the system.</param>
        /// <param name="Password">The user password.</param>
        /// <returns>The standard json string format, original ReturnValue - null </returns>
        public string createUser(string email, string Password)
        {
            try
            {
                uc.createUser(email, Password);
                Response<string> res = new Response<string>(null, null);
                log.Info("user " + email + " created successfully");
                return res.ResponseToJson();
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }

        /// <summary>
        ///  This method logs in an existing user.
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="Password">The password of the user to login</param>
        /// <returns>The standard json string format, original ReturnValue - string email </returns>
        public string login(string email, string Password)
        {
            try
            {
                uc.getUser(email).logIn(Password);
                Response<string> res = new Response<string>(null, email);
                log.Info("user " +email+ " logged in successfully");
                return res.ResponseToJson();
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }

        /// <summary>
        /// This method logs out a logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>The standard json string format, original ReturnValue - null </returns>
        public string logout(string email)
        {
            try
            {
                if (!uc.getUser(email).loggedIn)
                {
                    Response<string> res = new Response<string>("user is not logged in", null);
                    log.Error("user that is not logged in, can't log out");
                    return res.ResponseToJson();
                }
                else
                {
                    uc.logOut(email);
                    Response<string> res = new Response<string>(null, null);
                    log.Info("user" +email+ " logged out successfully");
                    return res.ResponseToJson();

                }
            }
            catch(Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error(ex.Message);
                return res.ResponseToJson();
            }
        }

        /// <summary>
        /// This method loads all users to the system from DB. 
        /// </summary>
        /// <returns>The standard Response format, original ReturnValue - null </returns>
        internal Response<string> LoadUsers()
        {
            try
            {
                uc.LoadUsers();
                Response<string> res = new Response<string>(null, null);
                log.Info("loading users data done successfully");
                return res;
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error("failed to load users" + ex.Message);
                return res;
            }
        }

        /// <summary>
        /// This method deletes all data from the system. 
        /// </summary>
        /// <returns>The standard Response format, original ReturnValue - null </returns>
        internal Response<string> DeleteData()
        {
            try
            {
                uc.DeleteData();
                Response<string> res = new Response<string>(null, null);
                log.Info("Delete Data done successfully");
                return res;
            }
            catch (Exception ex)
            {
                Response<string> res = new Response<string>(ex.Message, null);
                log.Error("error in deletiting data");
                return res;
            }

        }
    }
}
