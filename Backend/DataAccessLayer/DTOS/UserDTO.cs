using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOS
{
    class UserDTO : DTO
    {
        public const string UserEmailTable = "Email";
        public const string UserPasswordTable = "Password";
        private string email;
        private string password;

        public string Email
        {
            get { return email; }

        }
        internal string Password
        {
            get { return password; }
            set
            {
                _controller.Update(Id, UserPasswordTable, value);
                email = value;
            }
        } 
      
        public UserDTO(int id, string email, string pass) : base(new UserMapper()) // check creating new mapper
        {
            Id = id;
            this.email = email;
            password = pass;

        }

        /// <summary>
        /// Calls mapper to insert a new UserService to DB
        /// </summary>
        internal void addUserToDAL()
        {
            bool isInsert = ((UserMapper)Controller).Insert(this);
            if (!isInsert)
                throw new Exception("Error in data access layer: failed to insert userDTO to User table in DB");
        }
    }
}
