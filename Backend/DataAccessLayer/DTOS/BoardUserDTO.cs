using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOS
{
    class BoardUserDTO : DTO
    {
        public const string BoardIDTable = "BoardID";
        public const string UserNameTable = "Email";
        private int boardID;
        private string email;

        internal BoardUserDTO (int id, int boardID, string email) : base(new BoardUserMapper())
        {
            Id = id;
            this.boardID = boardID;
            this.email = email;
        }
        internal int BoardID
        {
            get { return boardID; }
        }
        internal string Email
        {
            get { return email; }
            set
            {
                bool res = _controller.Update(Id, UserNameTable, value);
                if (!res)
                    throw new Exception($"Failed to set the email of user '{email}' to '{value}' in DB");
                email = value;
            }
        }

    }
}
