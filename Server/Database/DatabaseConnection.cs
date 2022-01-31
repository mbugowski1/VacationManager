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
        private static string ConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
        public static void CreateUser(string username, byte[] password)
        {
            string query = "INSERT INTO credentials VALUES (@Username, @Password);";
            using (var connection = new MySqlConnection(ConnectionString("DB")))
            {
                var command = new MySqlCommand(query, connection);
                command.Parameters.Add("@Username", MySqlDbType.VarChar, 50);
                command.Parameters.Add("@Password", MySqlDbType.VarChar, 128);
                command.Parameters["@Username"].Value = username;
                command.Parameters["@Password"].Value = password;

                try
                {
                    connection.Open();
                    int affected = command.ExecuteNonQuery();
                    Console.WriteLine("Utworzono konto dla " + username);
                }
                catch(MySqlException e)
                {
                    if(e.Number == 1062)
                            throw new UserAlreadyExistsException(username);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(": Nie udalo sie utworzyc konta pracownika");
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public static async Task<bool> CheckPassword(string username, byte[] password)
        {
            bool success = false;
            password = Encoding.UTF8.GetBytes("test");
            string query = "SELECT EXISTS( SELECT * FROM credentials WHERE username = @Username AND password = @Password)";
            using (var connection = new MySqlConnection(ConnectionString("DB")))
            {
                var command = new MySqlCommand(query, connection);
                command.Parameters.Add("@Username", MySqlDbType.VarChar, 50);
                command.Parameters.Add("@Password", MySqlDbType.VarChar, 128);
                command.Parameters["@Username"].Value = username;
                command.Parameters["@Password"].Value = password;

                try
                {
                    connection.Open();
                    Console.WriteLine(command.CommandText);
                    Console.WriteLine(await command.ExecuteScalarAsync());
                    success = true;
                }
                catch(MySqlException e)
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
    }
}
