using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOS;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;
using System.Data;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    class BoardMapper : Mapper
    {
        private ColumnMapper columnMapper;
        private BoardUserMapper boardUserMapper;
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string boardTableName = "Board";

        public BoardMapper() : base(boardTableName)
        {
            columnMapper = new ColumnMapper();
            boardUserMapper = new BoardUserMapper();
        }
        public ColumnMapper ColumnMapper { get { return columnMapper; } }
        public BoardUserMapper BoardUserMapper { get { return boardUserMapper; } }

        /// <summary>
        /// Connects to DB and creates an insert query that inserts a board to DB
        /// </summary>
        /// <param name="board"> The board to insert to DB</param>
        /// <returns> True if information was inserted succesfully to DB, else False</returns>
        internal bool Insert(BoardDTO board)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    if (board.IsPersistent)
                        return false;
                    connection.Open();
                    command.CommandText = $"INSERT INTO {boardTableName} ({DTO.IDTable} ,{BoardDTO.BoardNameTable},{BoardDTO.BoardOwnerTable}) " +
                        $"VALUES (@ID,@Name,@CreatorEmail);";
                    SQLiteParameter id = new SQLiteParameter(@"ID", board.Id);
                    SQLiteParameter name = new SQLiteParameter(@"Name", board.Name);
                    SQLiteParameter creatorEmail = new SQLiteParameter(@"CreatorEmail", board.Owner);
                    command.Parameters.Add(id);
                    command.Parameters.Add(name);
                    command.Parameters.Add(creatorEmail);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (e.Message != null)
                        log.Error("error in Data Accsses Layer" + e);
                    res = -1;
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }
                log.Info("Insert new board successfully in Data Accsses Layer");
                return res > 0;
            }
        }


        /// <summary>
        /// Recieves SQLite board data that was recieved from DB and converts it to boardDTO object so it can be functional in our system
        /// </summary>
        /// <param name="reader"> SQLite data </param>
        /// <returns> DTO object (BoardDTO) </returns>
        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            List<ColumnDTO> columns = columnMapper.LoadColumns(reader.GetInt32(0));
            DTO result = new BoardDTO(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), columns);
            return result;
        }


        /// <summary>
        /// Load all boards from DB to a boards dictionary so we can use it in our system
        /// </summary>
        /// <returns> A dictionary that holds the boardID as a key and the BoardDTO object as value</returns>
        public Dictionary<int, BoardDTO> LoadBoards()
        {
            List<BoardDTO> result = SelectAllBoards();
            Dictionary<int, BoardDTO> boards = new Dictionary<int, BoardDTO>();
            foreach (BoardDTO board in result)
            {
                boards[board.Id] = board;
                board.Columns = columnMapper.LoadColumns(board.Id);
                board.Users = boardUserMapper.LoadBoardUser(board.Id);
            }
            log.Info("Load boards Data successfully in data access layer");
            return boards;
        }


        /// <summary>
        /// Select all boards from DB and creates a list of all boards in the system as boardDTOs.
        /// </summary>
        /// <returns> A list of BoardDTOs </returns>
        public List<BoardDTO> SelectAllBoards()
        {
            List<BoardDTO> result = Select().Cast<BoardDTO>().ToList();
            log.Info("Select boards Data successfully in data access layer");
            return result;
        }


        /// <summary>
        /// Returnes the Maximum int in ID column in a board.
        /// </summary>
        /// <returns> The maximum ID </returns>
        public int BoardMaxId()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand($"SELECT Max(ID) FROM {boardTableName}", connection);
                SQLiteDataReader reader = null;
                int max = 0;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();
                    if (reader.Read())
                        max = reader.GetInt32(0);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    command.Dispose();
                    connection.Close();
                }

                return max;
            }
        }


        /// <summary>
        /// Connects to DB and creates a query that deletes a board from DB and all the related data.
        /// <param name="boardId"> The ID of the board to delete from DB </param>
        /// </summary>
        internal void DeleteAll(int boardId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    command.CommandText = $"DELETE FROM {boardTableName} WHERE {DTO.IDTable} = @boardId";
                    SQLiteParameter BoardId = new SQLiteParameter(@"boardId", boardId);
                    command.Parameters.Add(BoardId);
                    connection.Open();
                    command.ExecuteNonQuery();
                    boardUserMapper.DeleteBoardUsers(boardId);
                    columnMapper.DeleteBoardColumns(boardId);

                }
                catch (Exception)
                {
                    log.Error("error in delete data Board");
                    throw new Exception("error in delete data Board");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
               
                log.Info("delete all in board data done successfully");
            }
        }
    }
}

