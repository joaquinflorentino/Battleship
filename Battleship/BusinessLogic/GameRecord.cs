using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.BusinessLogic
{
    public class GameRecord
    {
        private string _winner;
        private int _countOfShipsLeft;
        private int _turns;

        public GameRecord(string winner, int countOfShipsLeft, int turns)
        {
            _winner = winner;
            _countOfShipsLeft = countOfShipsLeft;
            _turns = turns;
        }

        public override string ToString()
        {
            return $"Winner: {_winner}, Winner's Ships Left: {_countOfShipsLeft}, Turns: {_turns}";
        }
    }
}
