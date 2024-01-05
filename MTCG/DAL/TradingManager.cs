using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
using Npgsql;

namespace MTCG.DAL
{
    internal class TradingManager: ITradingManager
    {
        private const string GetTradeByIdCommand = "SELECT * FROM trades WHERE id=@tradeId";
        private const string insertCommandText = "INSERT INTO trades (Id, CardToTrade, Type, MinimumDamage) VALUES (@Id, @CardToTrade, @Type, @MinimumDamage)";
        private const string GetAllTradesCommand = "SELECT * FROM trades";
        private const string DeleteTradeCommand = "DELETE FROM trades WHERE id = @tradeId;";

        private string _connectionString;

        public TradingManager(string connectionString)
        {
            _connectionString = connectionString;
            using var connection = new NpgsqlConnection(_connectionString);

            DatabaseTables.EnsureTables(connection);
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
                jsonBuilder.AppendFormat("\"Id\":\"{0}\",\n", reader["id"]);
                jsonBuilder.AppendFormat("\"CardToTrade\":{0},\n", reader["cardToTrade"]);
                jsonBuilder.AppendFormat("\"Type\":{0},\n", reader["type"]);
                jsonBuilder.AppendFormat("\"MinimumDamage\":{0}\n", reader["minimumdamage"]);
                jsonBuilder.Append("}\n");

                first = false;
            }

            jsonBuilder.Append("]");

            return jsonBuilder.ToString();

        }

        public string? GetallTrades()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(GetAllTradesCommand, connection);
            // take the first row, if any
            using var reader = cmd.ExecuteReader();
            if (!reader.HasRows) return null;
            return ReaderToString(reader);
        }

        public Trade? GetTrade(string tradeÌd)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(GetTradeByIdCommand, connection);
            cmd.Parameters.AddWithValue("tradeId", tradeÌd);
            // take the first row, if any
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Trade(
                    (string)reader["id"],
                    (string)reader["cardtotrade"],
                    (MonsterOrSpell)Enum.Parse(typeof(MonsterOrSpell), (string)reader["type"]),
                    (int)reader["minimumdamage"]
                    );
            }
            return null;

        }

        public bool InsertTrade(Trade trade)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(insertCommandText, connection);
            cmd.Parameters.AddWithValue("Id", trade.Id);
            cmd.Parameters.AddWithValue("CardToTrade", trade.CardToTrade);
            cmd.Parameters.AddWithValue("Type", trade.Type.ToString());
            cmd.Parameters.AddWithValue("MinimumDamage", trade.MinimumDamage);
            // take the first row, if any
            int affectedRows = cmd.ExecuteNonQuery();

            return affectedRows > 0;
        }

        public void DeleteTrade(string tradeId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(DeleteTradeCommand, connection);
            cmd.Parameters.AddWithValue("tradeId", tradeId);
            int affectedRows = cmd.ExecuteNonQuery();
        }
    }
}
