using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.BusinessLogic;

namespace Battleship.DataAccess
{
    public class GameRecordCsvManager
    {
        string _filePath;

        public GameRecordCsvManager(string filePath)
        {
            _filePath = Path.Combine(filePath, "gameRecords.csv");
        }

        public void SaveGameRecords(GameRecord gameRecord)
        {
            using (StreamWriter writer = new StreamWriter(_filePath, true))
            {
                writer.WriteLine(gameRecord.ToString());
            }
        }

        public string? ReadGameRecords()
        {
            try
            {
                return File.ReadAllText(_filePath);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found: " + _filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return null;
        }
    }
}
