using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer.Objects
{
    public class ColumnToFront
    {

        private string name;
        private BoardToFront board;
        private string boardName;
        private List<TaskToFront> taskList;
        private int columnNum; 
        
        public string Name { get { return name; } }
        public BoardToFront Board { get { return board; } set { board = value; } }
        public string BoardName { get { return boardName; } set { boardName = value; } }
        public List<TaskToFront> TaskList { get { return taskList; } }
        public int ColumnNum { get { return columnNum; } }

        public ColumnToFront(Column BusiCol)
        {
            this.columnNum = BusiCol.COLUMN_NUM1;
            if (columnNum == 0)
                this.name = "backlog";
            if (columnNum == 1)
                this.name = "in progress";
            if (columnNum == 2)
                this.name = "done";
            List<TaskToFront> tasks = new List<TaskToFront>();
            foreach (int tskId in BusiCol.Tasks.Keys)
            {
                TaskToFront task = new TaskToFront(BusiCol.Tasks[tskId]);
                task.BoardName = this.boardName;
                task.ColumnNum = this.ColumnNum;
                tasks.Add(task);
            }
            this.taskList = tasks;
        }
    }
}
