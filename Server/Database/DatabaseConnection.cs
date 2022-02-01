using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace VacationManagerServer.Database
{
    public class DatabaseConnection
    {
        private static string ConnectionString { get => ConfigurationManager.ConnectionStrings["DB"].ConnectionString; }
        public static void CreateUser(Person user, byte[] password, byte[] salt)
        {
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
                    Console.WriteLine("Utworzono konto dla " + user.Username);
                }
                catch(MySqlException e)
                {
                    if(e.Number == 1062)
                            throw new UserAlreadyExistsException(user.Username);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(": Nie udalo sie utworzyc konta pracownika");
                }
                finally
                {
                    connection.Close();
                }
            }
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
                    Console.WriteLine("Spodziewane: " + Encoding.UTF8.GetString(password));
                    Console.WriteLine("Sol: " + Encoding.UTF8.GetString(salt));
                    Console.WriteLine("Odczytane: " + Encoding.UTF8.GetString(savedPassword));

                    success = Enumerable.SequenceEqual(password, savedPassword);
                }
                catch(MySqlException)
                {
                    Console.WriteLine("Couldn\'t connect to database");
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
    }
}
