using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.BusinessLogic
{
    public class Ship
    {
        private List<Cell> _cells;
        private bool _isSunk;

        public int Length => _cells.Count;
        public List<Cell> Cells => _cells;
        public bool IsSunk => _isSunk;

        public void AddCell(Cell cell)
        {
            _cells.Add(cell);
            cell.Ship = this;
        }

        public Ship(List<Cell> cells)
        {
            _cells = cells;
        }

        public void Sink()
        {
            foreach (Cell cell in _cells)
            {
                cell.State = "sunk";
            }
            _isSunk = true;
        }
    }
}
