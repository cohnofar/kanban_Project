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
    class TaskMapper : Mapper
    {
        private const string TaskTableName = "Task";
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public TaskMapper() : base(TaskTableName)
        {

        }

        /// <summary>
        /// Connects to DB and creates an insert query that inserts a task to DB
        /// </summary>
        /// <param name="task"> The TaskDTO object to insert as a line in the table in DB</param>
        /// <returns> True if information was inserted succesfully to DB, else False</returns>
        public bool Insert(TaskDTO task)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    if (task.IsPersistent)
                        return false;

                    connection.Open();
                    command.CommandText = $"INSERT INTO {TaskTableName} ({DTO.IDTable},{TaskDTO.BoardIDTable},{TaskDTO.TitleTable},{TaskDTO.DescriptionTable} ,{TaskDTO.CreationDateTable},{TaskDTO.DueDateTable},{TaskDTO.AssigneeTable},{TaskDTO.ColumnNumberTable})" +
                        $"VALUES (@ID,@BoardID,@Title,@Description,@CreationDate,@DueDate,@Assignee,@ColumnNumber);";

                    SQLiteParameter idParam = new SQLiteParameter(@"ID", task.Id);
                    SQLiteParameter boardIDParam = new SQLiteParameter(@"BoardID", task.BoardID);
                    SQLiteParameter titleParam = new SQLiteParameter(@"Title", task.Title);
                    SQLiteParameter descriptionParam = new SQLiteParameter(@"Description", task.Description);
                    SQLiteParameter creationTimeParam = new SQLiteParameter(@"CreationDate", task.CreationDate.ToString());
                    SQLiteParameter dueDateParam = new SQLiteParameter(@"DueDate", task.DueDate.ToString());
                    SQLiteParameter asigneeParam = new SQLiteParameter(@"Assignee", task.Assignee);
                    SQLiteParameter coulmnOrdinalParam = new SQLiteParameter(@"ColumnNumber", task.ColumnNumber);

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(boardIDParam);
                    command.Parameters.Add(titleParam);
                    command.Parameters.Add(descriptionParam);
                    command.Parameters.Add(creationTimeParam);
                    command.Parameters.Add(dueDateParam);
                    command.Parameters.Add(asigneeParam);
                    command.Parameters.Add(coulmnOrdinalParam);


                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (e.Message != null)
                        log.Error("error in data access Layer" + e);
                    res = -1;
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }
                log.Info("Insert new task done successfully in data access layer");
                return res > 0;
            }
        }


        /// <summary>
        /// Recieves SQLite task data that was recieved from DB and converts it to TaskDTO object so it can be functional in our system
        /// </summary>
        /// <param name="reader"> SQLite data </param>
        /// <returns> DTO object (TaskDTO) </returns>
        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            TaskDTO result = new TaskDTO(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3), (DateTime)DateTime.Parse(reader.GetString(4)), (DateTime)DateTime.Parse(reader.GetString(5)), reader.GetString(6), reader.GetInt32(7));
            return result;
        }


        /// <summary>
        /// Load all task of a specified column in a specified board from DB to a list so we can use it in our system
        /// </summary>
        /// <param name="boardId"> The ID of the board to load it's columns tasks from DB </param>
        /// <param name="boardId"> The number of the column to load it's tasks from DB </param>
        /// <returns> A List of TaskDTO</returns>
        internal List<TaskDTO> LoadTasks(int columnNum, int boardID)
        {
            List<TaskDTO> results = new List<TaskDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {TaskTableName} WHERE {TaskDTO.ColumnNumberTable}=@ColumnNumber AND {TaskDTO.BoardIDTable}=@BoardID";
                SQLiteParameter coulmnNumberParam = new SQLiteParameter(@"ColumnNumber", columnNum);
                SQLiteParameter boardIDParam = new SQLiteParameter(@"BoardID", boardID);
                command.Parameters.Add(coulmnNumberParam);
                command.Parameters.Add(boardIDParam);
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                        results.Add((TaskDTO)(ConvertReaderToObject(dataReader)));
                    log.Info("Load Tasks Data successfully");
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
        /// Connects to DB and creates an Update query that gets a string argument to update
        /// </summary>
        /// <param name="id"> The ID of the line in DB to update</param>
        /// <param name="id"> The ID of the board that contains the task</param>
        /// <param name="attributeName"> The name of the field to update</param>
        /// <param name="attributeValue"> The string value to update</param>
        /// <returns> True if information was updated succesfully in DB, else False</returns>
        public bool Update(int id, int boardID, string attributeName, string attributeValue)
        {

            using (var connection = new SQLiteConnection(_connectionString))

            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command = new SQLiteCommand($"update {TaskTableName} set [{attributeName}]=@{attributeName} where id={id} AND {TaskDTO.BoardIDTable}=@BoardID ", connection);
                    SQLiteParameter BoardId = new SQLiteParameter(@"boardId", boardID);
                    command.Parameters.Add(BoardId);
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    command.Prepare();
                    res = command.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    if (e.Message != null)
                        log.Error("error in data access Layer" + e);
                    res = -1;
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }

                return res > 0;
            }
        }


        /// <summary>
        /// Connects to DB and creates an Update query that gets an int argument to update
        /// </summary>
        /// <param name="id"> The ID of the line in DB to update</param>
        /// <param name="boardID"> The ID of the board that contains the task</param>
        /// <param name="attributeName"> The name of the field to update</param>
        /// <param name="attributeValue"> The int value to update</param>
        /// <returns> True if information was updated succesfully in DB, else False</returns>
        public bool Update(int id, int boardID, string attributeName, int attributeValue)
        {

            using (var connection = new SQLiteConnection(_connectionString))

            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command = new SQLiteCommand($"update {TaskTableName} set [{attributeName}]=@{attributeName} where id={id} AND {TaskDTO.BoardIDTable}=@BoardID ", connection);
                    SQLiteParameter BoardId = new SQLiteParameter(@"boardId", boardID);
                    command.Parameters.Add(BoardId);
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    command.Prepare();
                    res = command.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    if (e.Message != null)
                        log.Error("error in data access Layer" + e);
                    res = -1;
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }

                return res > 0;
            }
        }



        /// <summary>
        /// Connects to DB and creates a delete query that gets an int argument to delete
        /// </summary>
        /// <param name="id"> The ID of the line in DB to delete</param>
        /// <param name="boardID"> The ID of the board that contains the task</param>
        /// <returns> True if information was deleted succesfully in DB, else False</returns>
        public bool delete(int id, int boardID)
        {

            using (var connection = new SQLiteConnection(_connectionString))

            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command = new SQLiteCommand($"delete from {TaskTableName} where id={id} AND {TaskDTO.BoardIDTable}=@BoardID ", connection);
                    SQLiteParameter BoardId = new SQLiteParameter(@"boardId", boardID);
                    command.Parameters.Add(BoardId);
                    command.Prepare();
                    res = command.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    if (e.Message != null)
                        log.Error("error in data access Layer" + e);
                    res = -1;
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }

                return res > 0;
            }
        }


        /// <summary>
        /// Returnes the Maximum int in ID column in a task.
        /// </summary>
        /// <returns> The maximum ID </returns>
        public int TaskMaxId(int boardID)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand($"SELECT Max(ID) FROM {TaskTableName} WHERE {TaskDTO.BoardIDTable} =@boardID", connection);
                SQLiteParameter BoardId = new SQLiteParameter(@"boardId", boardID);
                SQLiteDataReader reader = null;
                int max = -1;
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
    }
}


    
    