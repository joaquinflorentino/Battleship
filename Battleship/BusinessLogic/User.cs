using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.BusinessLogic
{
    public class User
    {
        private Board _boardObj;
        private Cell[,] _board;
        private List<Ship> _ships = new List<Ship>();

        public Cell[,] Board => _board;

        public List<Ship> Ships => _ships;

        public User()
        {
            _boardObj = new Board();
            _board = _boardObj.Cells;
        }

        public int GetCountOfShipsLeft()
        {
            int count = 0;

            foreach (Ship ship in _ships)
            {
                if (!ship.IsSunk)
                {
                    count++;
                }
            }
            return count;
        }

        public bool GetCoordinatesOfShipIfPlacementIsValid(ImageButton firstCell, ImageButton secondCell, int shipLength)
        {
            int row1 = firstCell.GetRow();
            int row2 = secondCell.GetRow();
            int col1 = firstCell.GetCol();
            int col2 = secondCell.GetCol();
            List<(int, int)> coordinates = new List<(int, int)>();

            if (row1 == row2 && Math.Abs(col2 - col1) == shipLength - 1)
            {
                for (int i = 0; i < shipLength; i++)
                {
                    (int, int) coordinate = (row1, Math.Min(col1, col2) + i);
                    Cell cell = _board[coordinate.Item1, coordinate.Item2];

                    if (cell.State == "empty")
                    {
                        coordinates.Add(coordinate);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (col1 == col2 && Math.Abs(row2 - row1) == shipLength - 1)
            {
                for (int i = 0; i < shipLength; i++)
                {
                    (int, int) coordinate = (Math.Min(row1, row2) + i, col1);
                    Cell cell = _board[coordinate.Item1, coordinate.Item2];

                    if (cell.State == "empty")
                    {
                        coordinates.Add(coordinate);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            DeployShip(coordinates);
            return true;
        }

        public bool IsCellPlacementValid(ImageButton uiCell)
        {
            List<Cell> cellsAroundCell = GetCellsAroundCell(_board[uiCell.GetRow(), uiCell.GetCol()]);
            
            foreach (Cell cell in cellsAroundCell)
            {
                if (cell.State == "ship")
                {
                    return false;
                }
            }
            return true;
        }


        public void DeployShip(List<(int, int)> coordinates)
        {
            List<Cell> cells = new List<Cell>();
            Ship ship = new Ship(cells);
            foreach ((int, int) coordinate in coordinates)
            {
                Cell cell = _board[coordinate.Item1, coordinate.Item2];
                cell.State = "ship";
                ship.AddCell(cell);
            }
            _ships.Add(ship);
        }

        public string HitCell(ImageButton uiCell)
        {
            Cell hitCell = _board[uiCell.GetRow(), uiCell.GetCol()];

            if (uiCell.GetState() == "ship")
            {
                hitCell.State = "hit";

                if (isShipFullyHit(hitCell.Ship))
                {
                    hitCell.Ship.Sink();
                    
                    foreach (Cell shipCell in hitCell.Ship.Cells)
                    {
                        List<Cell> cellsAround = GetCellsAroundCell(shipCell);

                        foreach (Cell cell in cellsAround)
                        {
                            if (cell.State != "sunk")
                            {
                                cell.State = "miss";
                            }
                        }
                    }
                }
            }
            else if (uiCell.GetState() == "empty")
            {
                _board[uiCell.GetRow(), uiCell.GetCol()].State = "miss";
            }
            else
            {
                throw new Exception("Select an empty cell");
            }
            return hitCell.State;
        }

        public bool DidILose()
        {
            foreach (Ship ship in _ships)
            {
                if (!ship.IsSunk)
                {
                    return false;
                }
            }
            return true;
        }

        private List<Cell> GetCellsAroundCell(Cell cell)
        {
            int row = cell.Row;
            int col = cell.Col;
            int[] rowOffset = new int[] { 0, -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] colOffset = new int[] { 0, -1, 0, 1, -1, 1, -1, 0, 1 };
            List<Cell> cellsAroundCell = new List<Cell>();

            for (int i = 0; i < rowOffset.Length; i++)
            {
                int newRow = row + rowOffset[i];
                int newCol = col + colOffset[i];

                if (newRow >= 0 && newRow < _board.GetLength(0) && newCol >= 0 && newCol < _board.GetLength(1))
                {
                    cellsAroundCell.Add(_board[newRow, newCol]);
                }
            }
            return cellsAroundCell;
        }

        private bool isShipFullyHit(Ship ship)
        {
            foreach (Cell cell in ship.Cells)
            {
                if (cell.State != "hit")
                {
                    return false;
                }
            }
            return true;
        }
    }
}
