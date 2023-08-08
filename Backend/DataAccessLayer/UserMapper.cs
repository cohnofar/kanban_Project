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
    class UserMapper : Mapper
    {
        private const string UserTableName = "User";
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public UserMapper() : base(UserTableName)
        {

        }


        /// <summary>
        /// Connects to DB and creates an insert query that inserts a UserService to the UserService table in DB
        /// </summary>
        /// <param name="userDal"> The UserService DTO to insert as a line in the table in DB</param>
        /// <returns> True if information was inserted succesfully to DB, else False</returns>
        public bool Insert(UserDTO userDal)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    if (userDal.IsPersistent)
                        return false;
                    command.CommandText = $"INSERT INTO '{UserTableName}' ('{DTO.IDTable}' ,'{UserDTO.UserEmailTable}' ,'{UserDTO.UserPasswordTable}') " +
                        $"VALUES (@ID,@Email,@Password);";
                    SQLiteParameter id = new SQLiteParameter(@"ID", userDal.Id);
                    SQLiteParameter email = new SQLiteParameter(@"Email", userDal.Email);
                    SQLiteParameter pass = new SQLiteParameter(@"Password", userDal.Password);
                    command.Parameters.Add(id);
                    command.Parameters.Add(email);
                    command.Parameters.Add(pass);
                    command.Prepare();
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (e != null)
                    {
                        log.Error("error in Data accsses layer" + e);
                    }
                    res = -1;
                    
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                log.Info("Insert new user successfully");
                return res > 0;
            }

        }


        /// <summary>
        /// Select all Users from DB and creates a list of all Users in the system as UserDTOs.
        /// </summary>
        /// <returns> A list of UserDTOs </returns>
        public List<UserDTO> SelectAllUsers()
        {
            log.Info("Select all users successfully in data layer");
            return Select().Cast<UserDTO>().ToList();
        }


        /// <summary>
        /// Load all users from DB to a Users dictionary so we can use it in our system
        /// </summary>
        /// <returns> A dictionary that holds the user Email as a key and the UserDTO object as value</returns>
        public Dictionary<string, UserDTO> LoadUsers()
        {
            List<UserDTO> users = SelectAllUsers();
            Dictionary<string, UserDTO> dictionary = new Dictionary<string, UserDTO>();
            foreach (UserDTO user in users)
                dictionary.Add(user.Email, user);
            log.Info("Loads all users successfully in data access layer");
            return dictionary;
        }


        /// <summary>
        /// Recieves SQLite UserService data that was recieved from DB and converts it to UserDTO object so it can be functional in our system
        /// </summary>
        /// <param name="reader"> SQLite data </param>
        /// <returns> DTO object (TaskDTO) </returns>
        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            UserDTO user = new UserDTO(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
            return user;

        }

    }

    }

