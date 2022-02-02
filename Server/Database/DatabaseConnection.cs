using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;
using VacationManagerLibrary;

namespace VacationManagerServer.Database
{
    public static class DatabaseConnection
    {
        private static string ConnectionString { get => ConfigurationManager.ConnectionStrings["DB"].ConnectionString; }
        public static bool CreateUser(Person user, byte[] password)
        {
            byte[]? salt = null;
            password = Security.HashPassword(password, ref salt);
            string query = "INSERT INTO credentials VALUES (@Username, @Password);";
            string query2 = "INSERT INTO salts VALUES (@Username, @Salt)";
            string query3s = "SELECT positionID FROM positions WHERE position = @Name";
            string query3 = "INSERT INTO data VALUES (@Username, @Firstname, @Lastname, @Position)";
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var command = new MySqlCommand(query, connection);
                command.Parameters.Add("@Username", MySqlDbType.VarChar, 50);
                command.Parameters.Add("@Password", MySqlDbType.VarBinary, 32);
                command.Parameters["@Username"].Value = user.Username;
                command.Parameters["@Password"].Value = password;

                var command2 = new MySqlCommand(query2, connection);
                command2.Parameters.Add("@Username", MySqlDbType.VarChar, 50);
                command2.Parameters.Add("@Salt", MySqlDbType.VarBinary, 8);
                command2.Parameters["@Username"].Value = user.Username;
                command2.Parameters["@Salt"].Value = salt;

                var command3s = new MySqlCommand(query3s, connection);
                command3s.Parameters.Add("@Name", MySqlDbType.VarChar, 30);
                command3s.Parameters["@Name"].Value = (user.Position == null) ? 0 : user.Position;

                var command3 = new MySqlCommand(query3, connection);
                command3.Parameters.Add("@Username", MySqlDbType.VarChar, 50);
                command3.Parameters.Add("@Firstname", MySqlDbType.VarChar, 30);
                command3.Parameters.Add("@Lastname", MySqlDbType.VarChar, 30);
                command3.Parameters.Add("@Position", MySqlDbType.Int32);
                command3.Parameters["@Username"].Value = user.Username;
                command3.Parameters["@Firstname"].Value = user.Firstname;
                command3.Parameters["@Lastname"].Value = user.Lastname;

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    command2.ExecuteNonQuery();
                    int positionId = Convert.ToInt32(command3s.ExecuteScalar());
                    command3.Parameters["@Position"].Value = positionId;
                    command3.ExecuteNonQuery();
                    Console.WriteLine("Created account for " + user.Username);
                }
                catch(MySqlException e)
                {
                    if(e.Number == 1062)
                            throw new UserAlreadyExistsException(user.Username);
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Couldn\'t create account for user " + user.Username);
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
            return true;
        }
        public static bool CheckPassword(string username, byte[] password)
        {
            bool success = false;
            string query = "SELECT salt FROM salts WHERE username = @Username";
            string query2 = "SELECT password FROM credentials WHERE username = @Username";
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var command = new MySqlCommand(query, connection);
                command.Parameters.Add("@Username", MySqlDbType.VarChar, 50);
                command.Parameters["@Username"].Value = username;

                var command2 = new MySqlCommand(query2, connection);
                command2.Parameters.Add("@Username", MySqlDbType.VarChar, 50);
                command2.Parameters["@Username"].Value = username;

                try
                {
                    connection.Open();
                    byte[] salt = (byte[])command.ExecuteScalar();
                    byte[] savedPassword = (byte[])command2.ExecuteScalar();

                    password = Security.HashPassword(password, ref salt);

                    success = Enumerable.SequenceEqual(password, savedPassword);
                    if (success)
                        Console.WriteLine(username + " logged in");
                }
                catch(MySqlException)
                {
                    Console.WriteLine("Couldn\'t connect to database");
                }
                catch(ArgumentNullException)
                {
                    return false;
                }
                finally
                {
                    connection.Close();
                }
                return success;
            }
        }
        public static Person GetData(string username)
        {
            string query = "SELECT name, surname, positions.position as position FROM data LEFT JOIN positions ON data.position = positions.positionID WHERE username = @Username";
            using(var connection = new MySqlConnection(ConnectionString))
            {
                var command = new MySqlCommand(query, connection);
                command.Parameters.Add("@Username", MySqlDbType.VarChar, 50);
                command.Parameters["@Username"].Value = username;
                Person person;
                try
                {
                    connection.Open();
                    using(var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        string firstname = (string)reader["name"];
                        string lastname = (string)reader["surname"];
                        string position = (string)reader["position"];
                        person = new(username, firstname, lastname, position);
                    }
                }
                catch(MySqlException e)
                {
                    Console.WriteLine(e.Number);
                    Console.WriteLine(e.Message);
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
                return person;
            }
        }
        public static bool SendEvent(VacationEvent vacationEvent)
        {
            string query = "INSERT INTO events(sender, recipient, start, end, code, type) VALUES (@Sender, @Recipient, @Start, @End, @Code, @Type)";
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var command = new MySqlCommand(query, connection);
                command.Parameters.Add("@Sender", MySqlDbType.VarChar, 50);
                command.Parameters.Add("@Recipient", MySqlDbType.VarChar, 50);
                command.Parameters.Add("@Start", MySqlDbType.Date);
                command.Parameters.Add("@End", MySqlDbType.Date);
                command.Parameters.Add("@Code", MySqlDbType.Int32);
                command.Parameters.Add("@Type", MySqlDbType.Int32);
                command.Parameters["@Sender"].Value = vacationEvent.Sender;
                command.Parameters["@Recipient"].Value = vacationEvent.Recipient;
                command.Parameters["@Start"].Value = vacationEvent.Start;
                command.Parameters["@End"].Value = vacationEvent.Stop;
                command.Parameters["@Code"].Value = vacationEvent.CodeId;
                command.Parameters["@Type"].Value = vacationEvent.TypeId;

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.WriteLine("Added new event");
                    Console.WriteLine(vacationEvent.ToString());
                }
                catch(MySqlException e)
                {
                    if (e.Number == 1452)
                    {
                        ArgumentException exception = new ArgumentException(e.Message);
                        throw exception;
                    }
                    throw e;
                    return false;
                }
                finally
                {
                    connection.Close();
                }
                return true;

            }
        }
        public static bool ChangeEventCode(int id, VacationEvent.Code code)
        {
            string checkQuery = "SELECT EXISTS(SELECT codeID FROM codes WHERE codeID = @Code)";
            string query = "UPDATE events SET code = @Code WHERE ID = @Id";
            using(var connection = new MySqlConnection(ConnectionString))
            {
                var checkCommand = new MySqlCommand(checkQuery, connection);
                checkCommand.Parameters.Add("@Code", MySqlDbType.Int32);
                checkCommand.Parameters["@Code"].Value = code;

                var command = new MySqlCommand(query, connection);
                command.Parameters.Add("@Code", MySqlDbType.Int32);
                command.Parameters.Add("@Id", MySqlDbType.Int32);
                command.Parameters["@Code"].Value = code;
                command.Parameters["@Id"].Value = id;

                try
                {
                    connection.Open();
                    bool codeExists = Convert.ToBoolean(checkCommand.ExecuteScalar());
                    if (!codeExists)
                        throw new CodeDoesNotExistException(code);
                    int affected = command.ExecuteNonQuery();
                    if (affected == 0)
                        throw new EventDoesNotExist(id);
                    else
                        Console.WriteLine("Changed code for " + id);
                }
                catch(MySqlException e)
                {
                    throw e;
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
            return true;
        }
        public static bool AddSupervisor(string worker, string supervisor)
        {
            string query = "INSERT INTO supervisors VALUES (@Worker, @Supervisor)";
            string checkQuery = "SELECT EXISTS(SELECT username FROM data WHERE username = @Worker)";
            using(var connection = new MySqlConnection(ConnectionString))
            {
                var checkCommand = new MySqlCommand(checkQuery, connection);
                checkCommand.Parameters.Add("@Worker", MySqlDbType.VarChar, 50);
                checkCommand.Parameters["@Worker"].Value = worker;
                var command = new MySqlCommand(query, connection);
                command.Parameters.Add("@Worker", MySqlDbType.VarChar, 50);
                command.Parameters.Add("@Supervisor", MySqlDbType.VarChar, 50);
                command.Parameters["@Worker"].Value = worker;
                command.Parameters["@Supervisor"].Value = supervisor;
                try
                {
                    connection.Open();
                    bool exists = Convert.ToBoolean(checkCommand.ExecuteScalar());
                    if (!exists)
                        throw new UserDoesNotExistException(worker);
                    checkCommand.Parameters["@Worker"].Value = supervisor;
                    exists = Convert.ToBoolean(checkCommand.ExecuteScalar());
                    if (!exists)
                        throw new UserDoesNotExistException(supervisor);
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Added new supervisor: { worker } -> { supervisor }");
                }
                catch(MySqlException e)
                {
                    throw e;
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
            return true;
        }
        public static bool RemoveSupervisor(string worker, string supervisor)
        {
            string query = "DELETE FROM supervisors WHERE worker = @Worker AND supervisor = @Super";
            using(var connection = new MySqlConnection(ConnectionString))
            {
                var command = new MySqlCommand(query, connection);
                command.Parameters.Add("@Worker", MySqlDbType.VarChar, 50);
                command.Parameters.Add("@Super", MySqlDbType.VarChar, 50);
                command.Parameters["@Worker"].Value = worker;
                command.Parameters["@Super"].Value = supervisor;

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Supervisor has been removed: {worker} -> {supervisor}");
                }
                catch(MySqlException e)
                {
                    throw e;
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
            return true;
        }
        public static List<VacationEvent> GetEvents(string worker, bool supervisor = false)
        {
            var events = new List<VacationEvent>();
            string query = "SELECT ID, sender, recipient, start, end, code, description, events.type, eventtypes.type as typedesc " +
                "FROM events LEFT JOIN supervisors ON sender = worker AND recipient = supervisor " +
                "LEFT JOIN codes ON codeID = code " +
                "LEFT JOIN eventtypes ON events.type = eventtypes.typeID WHERE ";
            if (supervisor)
                query += "supervisor = @Worker";
            else
                query += "worker = @Worker";
            using(var connection = new MySqlConnection(ConnectionString))
            {
                var command = new MySqlCommand(query, connection);
                command.Parameters.Add("@Worker", MySqlDbType.VarChar, 50);
                command.Parameters["@Worker"].Value = worker;

                try
                {
                    connection.Open();
                    using(var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            var newEvent = new VacationEvent();
                            newEvent.Sender = (string)reader["sender"];
                            newEvent.Recipient = (string)reader["recipient"];
                            newEvent.Start = (DateTime)reader["start"];
                            newEvent.Stop = (DateTime)reader["end"];
                            newEvent.CodeId = (VacationEvent.Code)reader["code"];
                            newEvent.TypeId = (VacationEvent.Type)reader["type"];
                            newEvent.ID = (int)reader["ID"];
                            newEvent.CodeDesc = (string)reader["description"];
                            newEvent.TypeDesc = (string)reader["typedesc"];
                            events.Add(newEvent);
                        }
                    }
                }
                catch(MySqlException e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
                return events;
            }
        }
        public static List<string> GetWorkers(string worker, bool supervisors = false)
        {
            var workers = new List<string>();
            string query;
            if (supervisors)
                query = "SELECT supervisor FROM supervisors WHERE worker = @Worker";
            else
                query = "SELECT worker FROM supervisors WHERE supervisor = @Worker";
            using(var connection = new MySqlConnection(ConnectionString))
            {
                var command = new MySqlCommand(query, connection);
                command.Parameters.Add("@Worker", MySqlDbType.VarChar, 50);
                command.Parameters["@Worker"].Value = worker;

                try
                {
                    connection.Open();
                    using(var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            workers.Add((string)reader[0]);
                        }
                    }
                }
                catch(MySqlException e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
            return workers;
        }
    }
}
