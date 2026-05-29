using Battleship.BusinessLogic;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;

namespace Battleship
{
    public static class ImageButtonExtensions
    {
        public static string GetState(this ImageButton imageButton)
        {
            if (imageButton.Source is FileImageSource fileSource)
            {
                string source = fileSource.File;
                return Path.GetFileNameWithoutExtension(source);
            }
            return null;
        }

        public static void SetState(this ImageButton imageButton, string state)
        {
            if (Board.validStateValues.Contains(state))
            {
                imageButton.Source = state + ".png";
            }
            else
            {
                throw new Exception("State value is invalid");
            }
        }

        public static int GetRow(this ImageButton imageButton)
        {
            return Grid.GetRow(imageButton) - 1;
        }

        public static int GetCol(this ImageButton imageButton)
        {
            return Grid.GetColumn(imageButton) - 1;
        }
    }
}