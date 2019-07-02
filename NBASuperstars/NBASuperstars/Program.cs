using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using CsvHelper;
using LINQtoCSV;

namespace NBASuperstars
{

    public class dataNBAPlayer
    {
        public string Name { get; set; }
        public int PlayingSince { get; set; }
        public string Position { get; set; }
        public int Rating { get; set; }
    }

    public class csvNBAPlayer
    {
        [CsvColumn(Name = "Name", FieldIndex = 1)]
        public string Name { get; set; }
        [CsvColumn(Name = "Rating", FieldIndex = 2)]
        public int Rating { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int tempInt;

            Console.WriteLine("Type path for the file with NBA players: ");
            // First argument - the path to the JSON file with the basketball players.
            string path = Console.ReadLine();
            while (!File.Exists(path))
            {
                Console.WriteLine("Not a valid path, try again.");
                path = Console.ReadLine();
            }

            // Second argument - the maximum number of years the player has played in the league to qualify.
            Console.WriteLine("Enter the maximum number of play years you want to filter by: ");
            string maxYears = Console.ReadLine();
            while (!Int32.TryParse(maxYears, out tempInt))
            {
                Console.WriteLine("Not a valid number, try again.");
                maxYears = Console.ReadLine();
            }

            // Third argument - the minimum rating tha player should have to qualify.
            Console.WriteLine("Enter the minimum rating for player to qualify: ");
            string minRating = Console.ReadLine();
            while (!Int32.TryParse(minRating, out tempInt))
            {
                Console.WriteLine("Not a valid number, try again.");
                maxYears = Console.ReadLine();
            }

            // Fourth argument - the path to the CSV file with the result.
            Console.WriteLine("Enter the path to the CSV file, that should be generated as a result: ");
            string resultCSVpath = Console.ReadLine();

            // Printing the user arguments 
            Console.WriteLine($"The path you want us to look for the file is: " + path);
            Console.WriteLine($"The max years ïs: " + maxYears);
            Console.WriteLine($"The minimum rating is: " + minRating);
            Console.WriteLine($"The path for the CSV file with the result is: " + resultCSVpath);

            Console.WriteLine();
            Console.WriteLine("Just reading all the results from the parsed json file: ");

            List<dataNBAPlayer> players;
            string jsonRead;

            using (StreamReader r = new StreamReader(path))
            {
                jsonRead = r.ReadToEnd();

            }

            players = JsonConvert.DeserializeObject<List<dataNBAPlayer>>(jsonRead);

            foreach (dataNBAPlayer player in players)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}", player.Name, player.PlayingSince, player.Position, player.Rating);
            }

            var shrtLstdPlrs = from evrPlr in players
                              where DateTime.Now.Year - evrPlr.PlayingSince <= Convert.ToInt32(maxYears) & evrPlr.Rating >= Convert.ToInt32(minRating)
                               select evrPlr;

            Console.WriteLine($"Here are the players with years of play lower or equal to: " + maxYears);
            foreach (var myplayer in shrtLstdPlrs)
                Console.WriteLine("{0}, {1}, {2}, {3}", myplayer.Name, myplayer.PlayingSince, myplayer.Position, myplayer.Rating);

            Console.WriteLine();
            IEnumerable<dataNBAPlayer> sortedPlayers = shrtLstdPlrs.OrderByDescending(x => x.Rating);
            foreach (var myplayer in sortedPlayers)
                Console.WriteLine("{0}, {1}, {2}, {3}", myplayer.Name, myplayer.PlayingSince, myplayer.Position, myplayer.Rating);

            List<csvNBAPlayer> csvSortedPlayer = new List<csvNBAPlayer>();
            foreach (var myplayer in sortedPlayers)
            {
                csvSortedPlayer.Add(new csvNBAPlayer()
                {
                    Name = myplayer.Name,
                    Rating = myplayer.Rating
                });
            }

            Console.WriteLine();

            string resultCSVfile = resultCSVpath + "\\resultCsv.csv";
            using (var csv = new CsvWriter(new StreamWriter(resultCSVfile)))
            {
                csv.WriteRecords(csvSortedPlayer);
            }
        }
    }
}
