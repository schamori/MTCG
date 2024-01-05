using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.Interface
{
    public interface ITradingService
    {
        bool CreateTrade(Trade trade);

        string? GetallTrades();

        Trade? GetTrade(string tradeId);

        void DeleteTrade(string tradeId);

        bool isCardMeetingRequirements(Trade trade, string cardToTradeId);

        void ExecuteTrade(Trade trade, string cardToTradeId);
    }
}
