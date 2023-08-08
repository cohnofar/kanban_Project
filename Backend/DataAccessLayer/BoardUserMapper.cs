using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOS;
using log4net.Config;
using System.IO;
using System.Reflection;
using System.Data;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOS
{
    internal class BoardUserMapper :Mapper
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string boardUserTableName = "BoardUser";

        public BoardUserMapper() : base(boardUserTableName)
        {


        }

        /// <summary>
        /// Connects to DB and creates an insert query that inserts a board and UserService to the BoardUser table in DB
        /// </summary>
        /// <param name="boardUser"> The boardUser DTO to insert as a line in the table in DB</param>
        /// <returns> True if information was inserted succesfully to DB, else False</returns>
        public bool Insert(BoardUserDTO boardUser)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    if (boardUser.IsPersistent)
                        return false;
                    connection.Open();
                    command.CommandText = $"INSERT INTO {boardUserTableName} ({DTO.IDTable} ,{BoardUserDTO.BoardIDTable},{BoardUserDTO.UserNameTable}) " +
                        $"VALUES (@ID,@BoardID,@Email);";
                    SQLiteParameter id = new SQLiteParameter(@"ID", boardUser.Id);
                    SQLiteParameter boardId = new SQLiteParameter(@"BoardID", boardUser.BoardID);
                    SQLiteParameter UserName = new SQLiteParameter(@"Email", boardUser.Email);
                    command.Parameters.Add(id);
                    command.Parameters.Add(boardId);
                    command.Parameters.Add(UserName);
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
                log.Info("Insert new boardUser successfully in Data Accsses Layer");
                return res > 0;
            }
        }

        /// <summary>
        /// Recieves SQLite boardUser data that was recieved from DB and converts it to boardUserDTO object so it can be functional in our system
        /// </summary>
        /// <param name="reader"> SQLite data </param>
        /// <returns> DTO object (BoardUserDTO) </returns>
        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            DTO result = new BoardUserDTO(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2));
            return result;
        }

        /// <summary>
        /// Load all board users from DB to a list so we can use it in our system
        /// </summary>
        /// <returns> A List of BoardUserDTO</returns>
        internal List<BoardUserDTO> LoadBoardUser(int boardID)
        {
            List<BoardUserDTO> results = new List<BoardUserDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {boardUserTableName} WHERE {BoardUserDTO.BoardIDTable}=@BoardID";
                SQLiteParameter boardIDParam = new SQLiteParameter(@"BoardID", boardID);
                command.Parameters.Add(boardIDParam);
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                        results.Add((BoardUserDTO)(ConvertReaderToObject(dataReader)));
                    log.Info("Load Board User Data successfully in data access layer");
                }
                finally
                {
                    if (dataReader != null)
                        dataReader.Close();
                    command.Dispose();
                    connection.Close();
                }

            }
            return results;
        }

        /// <summary>
        /// Connects to DB and creates a query that deletes all board's users from the relevant table in DB.
        /// <param name="boardId"> The ID of the board to delete it's Users from DB </param>
        /// </summary>
        internal void DeleteBoardUsers(int boardId)
        {
             using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    command.CommandText = $"DELETE FROM {boardUserTableName} WHERE {BoardUserDTO.BoardIDTable} = @boardId";
                    SQLiteParameter BoardId = new SQLiteParameter(@"boardId", boardId);
                    command.Parameters.Add(BoardId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    log.Error("error in delete Board user data");
                    throw new Exception("error in delete Board user data");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                log.Info("delete data from board User done successfully");
            }
        }

    }
}
