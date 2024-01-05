using MTCG.Models;
using MTCG.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using Npgsql;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;



namespace MTCG.DAL
{
    public class UserManager : IUserManager
    {
        private const string InsertUserCommand = @"INSERT INTO users(username, password, token, is_admin) VALUES (@username, @password, @token, @is_admin)";
        private const string GetUserByAuthTokenCommand = "SELECT * FROM users WHERE token=@token";
        private const string SelectTokenByCredentialsCommand = "SELECT token FROM users WHERE username=@username AND password=@password";
        private const string UpdateCoinsCommand = @"UPDATE users SET coins = @coins WHERE id=@userId";
        private const string GetUserCommand = "SELECT * FROM users WHERE id=@userId";
        private const string GetGameScoreboardCommand = "SELECT * FROM users ORDER BY elo DESC";


        private const string GetUserEloCommand = "SELECT elo FROM users WHERE id=@userId";
        private const string UpdateEloCommand = @"UPDATE users SET elo = @elo WHERE id=@userId";

        private const string GetUserWinsCommand = "SELECT wins FROM users WHERE id=@userId";
        private const string UpdateUserWinsCommand = @"UPDATE users SET wins = @wins WHERE id=@userId";
        private const string GetUserLossesCommand = "SELECT losses FROM users WHERE id=@userId";
        private const string UpdateUserLossesCommand = @"UPDATE users SET losses = @losses WHERE id=@userId";
        private const string UpateUserDataCommand = @"UPDATE users SET name = @name, bio = @bio, image = @image WHERE username = @username;";
        private const string GetUserByNameCommand = @"SELECT * FROM users WHERE username = @username;";
        private readonly string _connectionString;

        public UserManager(string connectionString)
        {
            _connectionString = connectionString;
            using var connection = new NpgsqlConnection(_connectionString);

            DatabaseTables.EnsureTables(connection);

        }


        public User? GetUserByAuthToken(string authToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(GetUserByAuthTokenCommand, connection);
            cmd.Parameters.AddWithValue("token", authToken);
            // take the first row, if any
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User(Convert.ToInt32(reader["id"]),
                    Convert.ToString(reader["username"])!,
                    Convert.ToString(reader["token"])!,
                    Convert.ToInt32(reader["coins"]),
                    Convert.ToInt32(reader["elo"]),
                    Convert.ToBoolean(reader["is_admin"]));
            }
            return null;

        }

        public string? GetTokenByCredentials(Credentials credentials)
        {

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(SelectTokenByCredentialsCommand, connection);
            cmd.Parameters.AddWithValue("username", credentials.Username);
            cmd.Parameters.AddWithValue("password", credentials.Password);

            // take the first row, if any
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Convert.ToString(reader["token"])!;
            }
            return null;
        }

        public bool InsertUser(Credentials credentials, string authToken)
        {

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                using var cmd = new NpgsqlCommand(InsertUserCommand, connection);
                cmd.Parameters.AddWithValue("username", credentials.Username);
                cmd.Parameters.AddWithValue("password", credentials.Password);
                cmd.Parameters.AddWithValue("token", authToken);
                cmd.Parameters.AddWithValue("is_admin", credentials.Username == "admin");

                var affectedRows = cmd.ExecuteNonQuery();

                return affectedRows > 0;
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "23505") // Duplicate keys 
                    return false;
                throw;
            }
        }

        private int GetUserCoins(NpgsqlConnection connection, int userId)
        {
            using var cmd = new NpgsqlCommand(GetUserCommand, connection);
            cmd.Parameters.AddWithValue("userId", userId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Convert.ToInt32(reader["coins"]);
            }
            else
                throw new UserNotfoundException();
        }

        public bool DecreaseCoinsAmound(int userId, int amount)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            int coins = GetUserCoins(connection, userId);
            if (coins - amount < 0)
            {
                return false;
            }
            using var cmd = new NpgsqlCommand(UpdateCoinsCommand, connection);
            int newCoins = coins - amount;
            cmd.Parameters.AddWithValue("coins", newCoins);
            cmd.Parameters.AddWithValue("userId", userId);
            // No need for error handling since we just selected the coins
            using var reader = cmd.ExecuteReader();
            return true;
        }

        private string ReaderToString(NpgsqlDataReader reader)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");

            bool first = true;

            while (reader.Read())
            {
                if (!first)
                {
                    jsonBuilder.Append(",");
                }

                jsonBuilder.Append("\n{\n");
                jsonBuilder.AppendFormat("\"Name\":\"{0}\",\n", reader["username"]);
                jsonBuilder.AppendFormat("\"Elo\":{0},\n", reader["elo"]);
                jsonBuilder.AppendFormat("\"Wins\":{0},\n", reader["wins"]);
                jsonBuilder.AppendFormat("\"Losses\":{0}\n", reader["losses"]);
                jsonBuilder.Append("}\n");

                first = false;
            }

            jsonBuilder.Append("]");

            return jsonBuilder.ToString();
        }
        public string GetGameScoreboard()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(GetGameScoreboardCommand, connection);
            using var reader = cmd.ExecuteReader();
            return ReaderToString(reader);
        }

        public string GetUserScore(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(GetUserCommand, connection);
            cmd.Parameters.AddWithValue("userId", userId);
            using var reader = cmd.ExecuteReader();
            return ReaderToString(reader);

        }
        private int GetUserElo(NpgsqlConnection connection, int userId)
        {
            using var cmd = new NpgsqlCommand(GetUserEloCommand, connection);
            cmd.Parameters.AddWithValue("userId", userId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Convert.ToInt32(reader["elo"]);
            }
            else
                throw new UserNotfoundException();
        }
        public void changeUserElo(int userId, int amount)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            int elo = GetUserElo(connection, userId);
            int newElo;

            if (elo + amount > 0)
                newElo = elo + amount;
            else
                newElo = 0;

            using var cmd = new NpgsqlCommand(UpdateEloCommand, connection);
            cmd.Parameters.AddWithValue("elo", newElo);
            cmd.Parameters.AddWithValue("userId", userId);
            using var reader = cmd.ExecuteReader();
        }

        private int GetUserLosses(NpgsqlConnection connection, int userId)
        {
            using var cmd = new NpgsqlCommand(GetUserLossesCommand, connection);
            cmd.Parameters.AddWithValue("userId", userId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Convert.ToInt32(reader["losses"]);
            }
            else
                throw new UserNotfoundException();
        }
        private int GetUserWins(NpgsqlConnection connection, int userId)
        {
            using var cmd = new NpgsqlCommand(GetUserWinsCommand, connection);
            cmd.Parameters.AddWithValue("userId", userId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Convert.ToInt32(reader["wins"]);
            }
            else
                throw new UserNotfoundException();
        }

        public void changeWinLosses(int userId, bool win)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            if (win)
            {
                int newWins = GetUserWins(connection, userId) + 1;
                using var cmd = new NpgsqlCommand(UpdateUserWinsCommand, connection);
                cmd.Parameters.AddWithValue("wins", newWins);
                cmd.Parameters.AddWithValue("userId", userId);
                using var reader = cmd.ExecuteReader();

            }
            else
            {
                int newLosses = GetUserLosses(connection, userId) + 1;
                using var cmd = new NpgsqlCommand(UpdateUserLossesCommand, connection);
                cmd.Parameters.AddWithValue("losses", newLosses);
                cmd.Parameters.AddWithValue("userId", userId);
                using var reader = cmd.ExecuteReader();
            }
        }

        public void UpdateUserData(string username, UserData userData)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(UpateUserDataCommand, connection);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("name", userData.Name);
            cmd.Parameters.AddWithValue("bio", userData.Bio);
            cmd.Parameters.AddWithValue("image", userData.Image);
            int affectedRows = cmd.ExecuteNonQuery();
            if (affectedRows != 0)
                throw new UserNotfoundException();
        }

        public string? GetUserData(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(GetUserByNameCommand, connection);
            cmd.Parameters.AddWithValue("username", username);

            using var reader = cmd.ExecuteReader();
            if (reader.Read()) {
                bool isNameNull = reader.IsDBNull(reader.GetOrdinal("name"));
                bool isBioNull = reader.IsDBNull(reader.GetOrdinal("bio"));
                bool isImageNull = reader.IsDBNull(reader.GetOrdinal("image"));

                if (isNameNull || isBioNull || isImageNull)
                {
                    return null;
                }
                var user = new
                {
                    Name = reader["name"] as string,
                    Bio = reader["bio"] as string,
                    Image = reader["image"] as string
                };
                return JsonConvert.SerializeObject(user, Formatting.Indented);
            } else {
                throw new UserNotfoundException();
            }
        }

        public int GetUserElo(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(GetUserEloCommand, connection);
            cmd.Parameters.AddWithValue("userId", userId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Convert.ToInt32(reader["elo"]);
            }
            else
            {
                throw new UserNotfoundException();
            }
        }
       
        
    }
}
