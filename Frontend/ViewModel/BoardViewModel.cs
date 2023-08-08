using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frontend.Model;

namespace Frontend.ViewModel
{
    public class BoardViewModel: Notifiable
    {
        public UserModel User { get; private set; }
        public BackendController Controller { get; private set; }
        public BoardModel Board { get; private set; }

        private string error;
        public string Error
        {
            get => error;
            set
            {
                error = value;
                RaisePropertyChanged("Error");
            }
        }

        //Constructor
        internal BoardViewModel(UserModel userM, BoardModel boardM)
        {
            this.User = userM;
            this.Board = boardM;
            Controller = boardM.Controller;
        }
    }
}
