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
using System.Threading.Tasks;

namespace MTCG.DAL
{
    public class CardsManager : ICardManager
    {
        private const string InsertCardCommand = "INSERT INTO cards (id, name, damage, type, element, packageId) VALUES (@id, @name, @damage, @type, @element, @packageId);";
        private const string GetCardByIdCommand = @"SELECT * FROM cards WHERE id = @cardId";
        private const string GetHighestPackageIdCommand = "SELECT MAX(packageId) AS highestPackageId FROM cards WHERE packageId IS NOT NULL;";
        private const string GetFreePackageCommand = "SELECT * from cards WHERE userId is NULL ORDER BY packageId ASC";
        private const string AssignUserToPackageCommand = @"UPDATE cards SET userId = @userId WHERE packageId = @packageId;";
        private const string GetUserCardsCommand = "SELECT * from cards WHERE userId = @userId";
        private const string GetUserDeckCommand = "SELECT * from cards WHERE userId = @userId and inDeck";
        private const string SetCardsToDeckCommand = "UPDATE cards SET indeck = true WHERE id = @cardId";
        private const string GetAvaliableUserCardsCommand = "SELECT c.* FROM cards c LEFT JOIN trades t ON c.id = t.CardToTrade WHERE c.userId = @userId AND t.CardToTrade IS NULL AND c.inDeck = false;";
        private const string UpdateCardOwnerCommand = "UPDATE cards SET userId = @userId WHERE id = @cardId;";
        private const string GetCardOwnerCommand = "SELECT userId from cards where id = @id";


        private readonly string _connectionString;

        public CardsManager(string connectionString)
        {
            _connectionString = connectionString;
            using var connection = new NpgsqlConnection(_connectionString);

            DatabaseTables.EnsureTables(connection);
        }


        private int GetHighestPackageId(NpgsqlConnection connection)
        {
            using var cmd = new NpgsqlCommand(GetHighestPackageIdCommand, connection);
            using var readerPackage = cmd.ExecuteReader();
            int newPackageId = 0;
            if (readerPackage.Read() && !readerPackage.IsDBNull(readerPackage.GetOrdinal("highestPackageId")))
            {
                newPackageId = Convert.ToInt32(readerPackage["highestPackageId"]) + 1;
            }
            return newPackageId;
        }
        public bool InsertCards(List<Card> cards)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            // Make sure all ids are free!
            foreach (var card in cards)
            {
                using var cmd = new NpgsqlCommand(GetCardByIdCommand, connection);
                cmd.Parameters.AddWithValue("cardId", card.Id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return false;
                }
            }


            int newPackageId = GetHighestPackageId(connection);

            foreach (var card in cards)
            {
                using var cmd = new NpgsqlCommand(InsertCardCommand, connection);
                cmd.Parameters.AddWithValue("id", card.Id);
                cmd.Parameters.AddWithValue("name", card.Name);
                cmd.Parameters.AddWithValue("damage", card.Damage);
                cmd.Parameters.AddWithValue("type", card.Type.ToString());
                cmd.Parameters.AddWithValue("element", card.Element.ToString());
                cmd.Parameters.AddWithValue("packageId", newPackageId);
                var affectedRows = cmd.ExecuteNonQuery();
            }
            return true;
        }

        private void AssignUserToPackage(NpgsqlConnection connection, int packageId, int userId)
        {
            using var cmd = new NpgsqlCommand(AssignUserToPackageCommand, connection);

            cmd.Parameters.AddWithValue("userId", userId);
            cmd.Parameters.AddWithValue("packageId", packageId);
            cmd.ExecuteReader();

        }

        private Card SqlReturnToCard(NpgsqlDataReader reader)
        {
            return new Card
                (
                    (string)reader["id"],
                     (string)reader["name"],
                     Convert.ToSingle(reader["damage"]),
                    (CardType)Enum.Parse(typeof(CardType), (string)reader["type"]),
                    (Element)Enum.Parse(typeof(Element), (string)reader["element"])
                );
        }
        public List<Card>? GetFreePackage(int? userId = null)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(GetFreePackageCommand, connection);
            using var reader = cmd.ExecuteReader();
            var cards = new List<Card>();
            int? packageId = null;
            if (reader.Read()) { 
                cards.Add(SqlReturnToCard(reader));
                packageId = (int)reader["packageId"];
            }
            reader.Close();
            if (userId != null && packageId != null)
            {
                AssignUserToPackage(connection, (int)packageId, (int)userId);
            }

            return cards.Count != 0 ? cards: null;

        }

        public List<Card>? GetUserCards(int userId, bool onlyDeck = false)
        {
            string sqlCommand = onlyDeck ? GetUserDeckCommand : GetUserCardsCommand;
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(sqlCommand, connection);
            cmd.Parameters.AddWithValue("userId", userId);

            using var reader = cmd.ExecuteReader();
            var cards = new List<Card>();
            while (reader.Read())
            {
                cards.Add(SqlReturnToCard(reader));
            }
            return cards.Count != 0 ? cards : null;


        }

        public bool SetCardsToDeck(List<string> cardIds)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            foreach(string cardId in cardIds)
            {
                using var cmd = new NpgsqlCommand(SetCardsToDeckCommand, connection);
                cmd.Parameters.AddWithValue("cardId", cardId);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0) return false;
            }

            return true;
        }

        public List<Card>? GetAvaliableUserCards(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(GetAvaliableUserCardsCommand, connection);
            cmd.Parameters.AddWithValue("userId", userId);
            using var reader = cmd.ExecuteReader();
            var cards = new List<Card>();
            while (reader.Read())
            {
                cards.Add(SqlReturnToCard(reader));
            }
            return cards.Count != 0 ? cards : null;
        }

        public Card GetCard(string cardId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(GetCardByIdCommand, connection);
            cmd.Parameters.AddWithValue("cardId", cardId);
            using var reader = cmd.ExecuteReader();
            reader.Read();
            return SqlReturnToCard(reader);
        }

        public bool UpdateCardOwner( string cardId, int newUserId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(UpdateCardOwnerCommand, connection);
            cmd.Parameters.AddWithValue("userId", newUserId);
            cmd.Parameters.AddWithValue("cardId", cardId);

            int affectedRows = cmd.ExecuteNonQuery();
            return affectedRows > 0;
        }

        public int GetCardOwner(string cardId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(GetCardOwnerCommand, connection);
            cmd.Parameters.AddWithValue("id", cardId);
            using var reader = cmd.ExecuteReader();
            reader.Read();
            return (int)reader["userId"];
        }
    }
    }

