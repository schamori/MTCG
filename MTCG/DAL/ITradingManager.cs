using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.DAL
{
    public interface ITradingManager
    {
        bool InsertTrade(Trade trade);
        Trade? GetTrade(string tradeÌd);
        string? GetallTrades();

        void DeleteTrade(string tradeId);
    }
}
