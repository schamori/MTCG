using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MTCG.DAL
{
    public static class DatabaseTables
    {
        private const string DropCards = @"DROP TABLE cards;";
        private const string DropUsers = @"DROP TABLE users;";
        private const string DropTrades = @"DROP TABLE trades;";



        private const string CreateUserTableCommand = @"
        CREATE TABLE IF NOT EXISTS ""users"" (
            id SERIAL PRIMARY KEY, 
            username VARCHAR(100) NOT NULL UNIQUE, 
            password VARCHAR(200) NOT NULL, 
            token VARCHAR(50) NOT NULL UNIQUE, 
            coins INT DEFAULT 20, 
            elo INT DEFAULT 100, 
            is_admin BOOLEAN DEFAULT false, 
            wins INT DEFAULT 0, 
            losses INT DEFAULT 0,
            name VARCHAR(100), 
            bio VARCHAR(255), 
            image VARCHAR(255)
        );";
        private const string CreateCardTableCommand = @"CREATE TABLE IF NOT EXISTS ""cards"" ( id VARCHAR(255) PRIMARY KEY, name VARCHAR(255) NOT NULL, damage FLOAT NOT NULL, type VARCHAR(50) NOT NULL, element VARCHAR(50) NOT NULL, packageId INT NOT NULL, inDeck bool DEFAULT false, userId INT, FOREIGN KEY (userId) REFERENCES users(id) );";
        private const string CreatePackageTableCommand = @"CREATE TABLE IF NOT EXISTS trades ( Id VARCHAR(255) PRIMARY KEY, CardToTrade VARCHAR(255), Type VARCHAR(50), MinimumDamage INT, FOREIGN KEY (CardToTrade) REFERENCES cards (id) );";

        public static void EnsureTables(NpgsqlConnection connection)
        {
       
            connection.Open();
            using (NpgsqlCommand cmd = new NpgsqlCommand(DropTrades, connection))
            {
                cmd.ExecuteNonQuery();
            }
            using (NpgsqlCommand cmd = new NpgsqlCommand(DropCards, connection))
            {
              cmd.ExecuteNonQuery();
            }


            using (NpgsqlCommand cmd = new NpgsqlCommand(DropUsers, connection))
            {
                cmd.ExecuteNonQuery();
            }
            using (NpgsqlCommand cmd = new NpgsqlCommand(CreateUserTableCommand, connection))
            {
                cmd.ExecuteNonQuery();
            }


            using (NpgsqlCommand cmd = new NpgsqlCommand(CreateCardTableCommand, connection))
            {
                cmd.ExecuteNonQuery();
            }
            using (NpgsqlCommand cmd = new NpgsqlCommand(CreatePackageTableCommand, connection))
            {
                cmd.ExecuteNonQuery();
            }
             
        }
    }
    }

