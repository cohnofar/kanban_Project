using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer.Objects
{
    public class BoardToFront
    {
        public readonly string boardName;
        public Dictionary<int, ColumnToFront> columns;

        public string BoardName { get { return boardName; } }
        public Dictionary<int, ColumnToFront> Columns { get { return columns; } }



        public BoardToFront( string boardName, Dictionary<int, ColumnToFront> columns)
        {
            this.boardName = boardName;
            this.columns = columns;
        }

        public BoardToFront(Board busiBoard)
        {
            Dictionary<int, ColumnToFront> columns = new Dictionary<int, ColumnToFront>();
            foreach (int clm in busiBoard.Columns.Keys)
            {
                columns[clm] = new ColumnToFront(busiBoard.Columns[clm]);
            }
            this.boardName = busiBoard.Name;
            this.columns = columns;

        }
        
        /// <summary>
        /// This method sets the board information for each column of the board
        /// </summary>
        public void setColBoard ()
        {
            foreach (int clm in this.columns.Keys)
            {
                columns[clm].Board = this;
                columns[clm].BoardName = boardName;
            }
        }

        /// <summary>
        /// This method returnes a desired ColumnToFront object
        /// </summary>
        /// <param name="columnNum">The column ordinal of the desired column</param>
        /// <returns>the desired ColumnToFront oject </returns>
        public ColumnToFront GetColumn(int columnNum){ 
            return columns[columnNum]; 
        }

        /// <summary>
        /// This method returnes a list of all the board's columns
        /// </summary>
        /// <returns>A list of ColumnToFronts </returns>
        public List<ColumnToFront> getColumnsList()
        {
            List<ColumnToFront> columnsList = new List<ColumnToFront>();
            foreach (int clm in this.columns.Keys)
            {
               columnsList.Add(this.columns[clm]);
            }
            return columnsList;
        }
    }
}
