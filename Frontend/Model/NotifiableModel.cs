using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    public abstract class NotifiableModel : Notifiable
    {
        public BackendController Controller { get; private set; }
        protected NotifiableModel(BackendController controller)
        {
            this.Controller = controller;
        }
    }
}