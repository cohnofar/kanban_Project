using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frontend.Model;

namespace Frontend.ViewModel
{
    internal class LoginViewModel: Notifiable
    {
        private BackendController backendController;
        private string userEmail = "";
        private string password = "";
        private string message;

        public string UserEmail {
            get { return userEmail; }
            set
            {
                userEmail = value;
                RaisePropertyChanged("Email");
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                RaisePropertyChanged("Password");
            }
        }
        public string Message
        {
            set
            {
                message = value;
                RaisePropertyChanged("Message");
            }
            get { return message; }
        }


        public LoginViewModel() { this.backendController = new BackendController(); }

        /// <summary>
        /// Login the user to the system
        /// </summary>
        /// <returns>UserModel object if succeed, otherwise returns null</returns>
        public UserModel login()
        {
            Message = "";
            try
            {
                return backendController.login(UserEmail, Password);
            }
            catch (Exception e)
            {
                Message = e.Message;
                return null;
            }
        }

        /// <summary>
        /// register a user to the system
        /// </summary>
        public UserModel register()
        {
            Message = "";
            try
            {
                return backendController.register(UserEmail, Password);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
            return null;
        }

        /// <summary>
        /// Loading the data from DB
        /// </summary>
        public void LoadData()
        {
            Message = "";
            try
            {
                backendController.LoadData();
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
        }
    }
}
