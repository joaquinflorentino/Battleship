using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.BusinessLogic
{
    public class ShipItemModel
    {
        public string ImageSource { get; }
        public int Length { get; }

        public ShipItemModel(string imageSource, int length)
        {
            ImageSource = imageSource;
            Length = length;
        }
    }
}
