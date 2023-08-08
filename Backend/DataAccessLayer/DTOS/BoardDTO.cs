using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOS
{
    class BoardDTO : DTO
    {
        public const string BoardNameTable = "Name";
        public const string BoardOwnerTable = "OwnerEmail";
        private string owner;
        private string name;
        private List<BoardUserDTO> users;
        private List<ColumnDTO> columns;


        internal string Name
        {
            get { return name; }
            set
            {
                bool res = _controller.Update(Id, BoardNameTable, value);
                if (!res)
                    throw new Exception($"Failed in renaming board '{Id}' in Data access layer");
                name = value;
            }
        }
        public List<string> UsersString { get {
                List<string> result = new List<string>();
                foreach (BoardUserDTO boardUser in users)
                {
                    result.Add(boardUser.Email);
                }
                return result;
            }
        }
        public List<ColumnDTO> Columns
        {
            get { return columns; }
            set
            {
                columns = value;
            }
        }
        public List<BoardUserDTO> Users
        {
            get { return users; }
            set
            {
                users = value;
            }
        }
        internal string Owner
        {
            get { return owner; }
            set
            {
                bool res = _controller.Update(Id, BoardOwnerTable, value);
                if (!res)
                    throw new Exception($"Failed in transfer ownership for board '{Id}' in Data access layer");
                owner = value;
            }
        }

        internal BoardDTO(int id, string name, string email, List<ColumnDTO> columnDTOs) : base(new BoardMapper())
        {
            Id = id;
            this.name = name;
            this.owner = email;
            columns = columnDTOs;
            users = new List<BoardUserDTO>();
            users.Add(new BoardUserDTO(-1, id, email));
        }

        /// <summary>
        /// Calls Mapper to insert the new board to its proper location in DB 
        /// </summary>
        public void addBoardToDAL()
        {
            bool resBoard = ((BoardMapper)(_controller)).Insert(this);
            if (!resBoard)
                throw new Exception($"Failed in insert a the board '{Id}' to DB");
            foreach (ColumnDTO columnDTO in columns)
            {
                bool resColumn = ((ColumnMapper)columnDTO.Controller).Insert(columnDTO);
                if (!resColumn)
                    throw new Exception($"Failed in insert to board '{Id}' the column '{columnDTO.ColumnName}' in the DB");
            }
            foreach (BoardUserDTO memberDTO in users)
            {
                bool resBoardMapper = ((BoardUserMapper)memberDTO.Controller).Insert(memberDTO);
                if (!resBoardMapper)
                    throw new Exception($"Failed in insert to board '{Id}' the user '{memberDTO.Email}' as a member in the board in the DB");
            }
        }


        /// <summary>
        /// Calls Mapper to update board's owner in BD 
        /// </summary>
        /// <param name="newOwner">Email of the desired new owner</param>
        public void transferOwnershipInDAL(string newOwner)
        {
            Owner = newOwner;
        }


        /// <summary>
        /// Calls the relevant methods in Task and Column DTOs to advance task in DB
        /// </summary>
        /// <param name="column">Current column of the task</param>
        /// <param name="taskID">ID of the task</param>
        internal void advanceInDAL(int column, int taskID)
        {
            TaskDTO taskDTOToAdvance = columns[column].removeFromTasksList(taskID);
            columns[column + 1].addToTasksList(taskDTOToAdvance);
        }


        /// <summary>
        /// gets the maximum ID of the task in an existing board to continue giving unique IDs after shutting down and reloading the system
        /// </summary>
        /// <param name="boardID">ID of the board</param>
        /// <returns>int - the maximum ID</returns>
        public int taskMaxId(int boardID)
        {
            int max = 0;
            foreach (ColumnDTO column in columns)
            {
                max = max + column.Tasks.Count();
            }
            return max;
        }


        /// <summary>
        /// Calls Mapper to insert a user email as a member of board in DB 
        /// </summary>
        /// <param name="email">email of the user who joined the board</param>
        internal void joinBoardUserToDAL(string email)
        {
            BoardUserDTO buDTO = new BoardUserDTO(-1, Id, email);
            bool res = ((BoardMapper)Controller).BoardUserMapper.Insert(buDTO);
            if (!res)
                throw new Exception($"Failed to insert user '{email}' as a member in board '{Id}' to the DB");
            else
            {
                users.Add(buDTO);
            }
        }

        /// <summary>
        /// Calls Mapper to delete a board from DB
        /// </summary>
        internal void deleteBoardFromDAL()
        {
            ((BoardMapper)Controller).DeleteAll(Id);
        }
    }
}
    