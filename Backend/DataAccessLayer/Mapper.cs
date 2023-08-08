using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOS;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal abstract class Mapper
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected readonly string _connectionString;
        private readonly string _tableName;
        public Mapper(string tableName)
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            this._connectionString = $"Data Source={path}; Version=3;";
            this._tableName = tableName;
        }


        /// <summary>
        /// Connects to DB and creates an Update query that gets a string argument to update
        /// </summary>
        /// <param name="id"> The ID of the line in DB to update</param>
        /// <param name="attributeName"> The name of the field to update</param>
        /// <param name="attributeValue"> The string value to update</param>
        /// <returns> True if information was updated succesfully in DB, else False</returns>
        public bool Update(int id, string attributeName, string attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where id={id}"
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    
                }
                catch (Exception e)
                {
                    if (e != null)
                        log.Error("error accured in data access layer"+ e);
                    res = -1;
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                log.Info("update done successfully in data accsses layer");

            }
            return res > 0;
        }


        /// <summary>
        /// Connects to DB and creates an Update query that gets an int argument to update
        /// </summary>
        /// <param name="id"> The ID of the line in DB to update</param>
        /// <param name="attributeName"> The name of the field to update</param>
        /// <param name="attributeValue"> The int value to update</param>
        /// <returns> True if information was updated succesfully in DB, else False</returns>
        public bool Update(int id, string attributeName, int attributeValue)
        {

            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where id={id}"
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    
                }
                catch (Exception e)
                {
                    if (e != null)
                        log.Error("error accured in data access layer" + e);
                    res = -1;
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                log.Info("update done successfully in data accsses layer");
            }
            return res > 0;
        }


        /// <summary>
        /// Connects to DB and creates a select query that returns a list of lines from DB
        /// </summary>
        /// <returns> A list of DTO objects </returns>
        protected List<DTO> Select()
        {
            List<DTO> results = new List<DTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {_tableName};";
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                        results.Add(ConvertReaderToObject(dataReader));
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
        /// Recieves SQLite data that was recieved from DB and converts it to an DTO object so it can be functional in our system
        /// </summary>
        /// <param name="reader"> SQLite data </param>
        /// <returns> DTO object </returns>
        protected abstract DTO ConvertReaderToObject(SQLiteDataReader reader);


        /// <summary>
        /// Connects to DB and creates a Delete query.
        /// </summary>
        /// <param name="DTOObj"> The DTO object to delee from DB</param>
        /// <returns> True if information was deleted succesfully from DB, else False</returns>
        public bool Delete(DTO DTOObj)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where id={DTOObj.Id}"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                log.Info("delete done successfully in data accsses layer");

            }
            return res > 0;
        }


        /// <summary>
        /// Connects to DB and creates a query that deletes all data from a table.
        /// </summary>
        internal void DeleteAll()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"DELETE FROM {_tableName}";

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                }
                catch (Exception)
                {
                    log.Error("error in delete data");
                    throw new Exception("error in delete data");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                log.Info("delete all data done successfully");

            }
        }


    }
}
