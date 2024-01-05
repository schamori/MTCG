using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Interface;
using MTCG.DAL;
using MTCG.Models;

namespace MTCG.Services
{
    internal class TradingService: ITradingService
    {
        private ITradingManager _tradingManager;
        private ICardManager _cardManager;

        public TradingService(ITradingManager tradingManager, ICardManager cardManager)
        {
            _tradingManager = tradingManager;
            _cardManager = cardManager;
        }


        
        public bool CreateTrade(Trade trade)
        {
            if (_tradingManager.GetTrade(trade.Id) is not null) return false;

            _tradingManager.InsertTrade(trade);
            return true;
        }

        public void DeleteTrade(string tradeId)
        {
            _tradingManager.DeleteTrade(tradeId);
        }

        public void ExecuteTrade(Trade trade, string cardToTradeId)
        {

            int tradeUserId = _cardManager.GetCardOwner(trade.CardToTrade);
            int acceptedUserId = _cardManager.GetCardOwner(cardToTradeId);
            _cardManager.UpdateCardOwner(trade.CardToTrade, acceptedUserId);
            _cardManager.UpdateCardOwner(cardToTradeId, tradeUserId);
            _tradingManager.DeleteTrade(trade.Id);
        }

        public string? GetallTrades()
        {
            return _tradingManager.GetallTrades();
        }

        public Trade? GetTrade(string tradeId)
        {
            return _tradingManager.GetTrade(tradeId);
        }

        public bool isCardMeetingRequirements(Trade trade, string cardToTradeId)
        {
            Card card = _cardManager.GetCard(cardToTradeId)!;
            // The dont have the same datatype since CardType does not have a Monster Type, but Types for Knights...
            if ((trade.Type == MonsterOrSpell.Spell && card.Type != CardType.Spell) ||
                (trade.Type == MonsterOrSpell.Monster && card.Type == CardType.Spell)) return false;
            if (card.Damage < trade.MinimumDamage) return false;
            return true;
        }

    }
}
