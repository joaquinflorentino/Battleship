using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.BusinessLogic
{
    public class Cell
    {
        private int _row;
        private int _col;
        private string _state;
        private Ship _ship;

        public int Row
        {
            get { return _row; }
            init
            {
                if (value < 0 || value > Board.GRID_SIZE)
                {
                    throw new Exception("Row value is out of bounds");
                }
                else
                {
                    _row = value;
                }
            }
        }

        public int Col
        {
            get { return _col; }
            init
            {
                if (value < 0 || value > Board.GRID_SIZE)
                {
                    throw new Exception("Column value is out of bounds");
                }
                else
                {
                    _col = value;
                }
            }
        }

        public string State
        {
            get { return _state; }
            set
            {
                if (!Board.validStateValues.Contains(value))
                {
                    throw new Exception("State value is invalid");
                }
                else
                {
                    _state = value;
                }
            }
        }

        public Ship Ship
        {
            get { return _ship; }
            set { _ship = value; }
        }

        public Cell(int row, int col, string state)
        {
            Row = row;
            Col = col;
            State = state;
        }
    }
}