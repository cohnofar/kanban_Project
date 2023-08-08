using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{   public class UserModel : NotifiableModel
    {
        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                RaisePropertyChanged("Email");
            }
        }

        //Constructor
        public UserModel(BackendController controller, string email) : base(controller)
        {
            this.Email = email;
        }
    }
}
