using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.BusinessLogic
{
    public class Board
    {
        public static readonly string[] validStateValues = { "empty", "ship", "hit", "miss", "first_cell_clicked", "sunk" };
        public const int GRID_SIZE = 10;
        private Cell[,] _cells;

        public Cell[,] Cells => _cells;


        public Board()
        {
            _cells = new Cell[Board.GRID_SIZE, Board.GRID_SIZE];

            for (int row = 0; row < Board.GRID_SIZE; row++)
            {
                for (int col = 0; col < Board.GRID_SIZE; col++)
                {
                    _cells[row, col] = new Cell(row, col, "empty");
                }
            }
        }
    }
}