using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Interface
{
    public interface IGameService
    {
        Winner BattleCards(Card card1, Card card2);

        string GetUserScore(int userId);
        string GetScoreboard();

        string? WaitOrStartBattle(User user);
    }
}
