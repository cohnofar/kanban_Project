using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer.Objects
{
    public class UserToFront
    {
        private readonly string email;

        public UserToFront(string email)
        {
            this.email = email;
        }
    }
}
