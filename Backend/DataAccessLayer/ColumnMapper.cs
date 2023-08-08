using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOS;
using System.Data.SQLite;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    class ColumnMapper : Mapper
    {
        private const string ColumnTableName = "Column";
        private TaskMapper _taskMapper;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ColumnMapper() : base(ColumnTableName)
        {
            _taskMapper = new TaskMapper();
        }

        public TaskMapper TaskMapper { get { return _taskMapper; } }

        /// <summary>
        /// Connects to DB and creates an insert query that inserts a column to DB
        /// </summary>
        /// <param name="column"> The ColumnDTO object to insert as a line in the table in DB</param>
        /// <returns> True if information was inserted succesfully to DB, else False</returns>
        public bool Insert(ColumnDTO column)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    if (column.IsPersistent)
                        return false;
                    connection.Open();
                    command.CommandText = $"INSERT INTO {ColumnTableName} ({DTO.IDTable},{ColumnDTO.ColumnBoardIDTable} ,{ColumnDTO.ColumnNumberTable},{ColumnDTO.ColumnNameTable} ,{ColumnDTO.ColumnTaskLimitTable}) " +
                        $"VALUES (@ID,@BoardID,@ColumnNumber,@ColumnName,@TaskLimit);";

                    SQLiteParameter id = new SQLiteParameter(@"ID", column.Id);
                    SQLiteParameter boardID = new SQLiteParameter(@"BoardID", column.BoardID);
                    SQLiteParameter columnNum = new SQLiteParameter(@"ColumnNumber", column.ColumnNumber);
                    SQLiteParameter columnName = new SQLiteParameter(@"ColumnName", column.ColumnName);
                    SQLiteParameter taskLimit = new SQLiteParameter(@"TaskLimit", column.TaskLimit);
                    command.Parameters.Add(id);
                    command.Parameters.Add(boardID);
                    command.Parameters.Add(columnNum);
                    command.Parameters.Add(columnName);
                    command.Parameters.Add(taskLimit);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (e != null)
                        log.Error("error in data accsses layer" +e);
                    res = -1;
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                log.Info("Insert new column successfully in the accsses layer");
                return res > 0;
            }
        }

        /// <summary>
        /// Load all columns of a specified board from DB to a list so we can use it in our system
        /// </summary>
        /// <param name="boardId"> The ID of the board to load it's columns from DB </param>
        /// <returns> A List of ColumnDTO</returns>
        internal List<ColumnDTO> LoadColumns(int boardID)
        {
            List<ColumnDTO> results = new List<ColumnDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {ColumnTableName} WHERE {ColumnDTO.ColumnBoardIDTable}=@BoardID";
                SQLiteParameter boardIDParam = new SQLiteParameter(@"BoardID", boardID);
                command.Parameters.Add(boardIDParam);
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                        results.Add((ColumnDTO)(ConvertReaderToObject(dataReader)));
                    List<TaskDTO> list0 = _taskMapper.LoadTasks(0, boardID);
                    foreach(TaskDTO task in list0)
                    {
                        results[0].Tasks.Add(task);
                    }
                    List<TaskDTO> list1 = _taskMapper.LoadTasks(1, boardID);
                    foreach (TaskDTO task in list1)
                    {
                        results[1].Tasks.Add(task);
                    }
                    List<TaskDTO> list2 = _taskMapper.LoadTasks(2, boardID);
                    foreach (TaskDTO task in list2)
                    {
                        results[2].Tasks.Add(task);
                    }
                    log.Info("Load Columns Data successfully in data access layer");
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
        /// Recieves SQLite column data that was recieved from DB and converts it to ColumnDO object so it can be functional in our system
        /// </summary>
        /// <param name="reader"> SQLite data </param>
        /// <returns> DTO object (ColumnDTO) </returns>
        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            ColumnDTO result = new ColumnDTO(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetString(3), reader.GetInt32(4));
            return result;
        }

        /// <summary>
        /// Connects to DB and creates a query that deletes all board's columns from the relevant table in DB.
        /// <param name="boardId"> The ID of the board to delete it's columns from DB </param>
        /// </summary>
        internal void DeleteBoardColumns(int boardId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    command.CommandText = $"DELETE FROM {ColumnTableName} WHERE {ColumnDTO.ColumnBoardIDTable} = @boardId";
                    SQLiteParameter BoardId = new SQLiteParameter(@"boardId", boardId);
                    command.Parameters.Add(BoardId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    log.Error("error in delete column data");
                    throw new Exception("error in delete column data");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                log.Info("delete data from column done successfully");
            }
        }
    }
}
