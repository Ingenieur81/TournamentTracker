using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TrackerLibrary.Models;

// Separate namespace to keep the process clean for users that
// do not use this namespace
namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        /// <summary>
        /// Gets the full path of the file
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <returns>The full path</returns>
        //using 'this' extension method with static to only provide this method to this class
        public static string FullFilePath(this string fileName) 
        {
            // '$' allows concatenating strings together, which is more intuitive and perfomant
            // AppSettings is a dictionary from App.config, "filePath" is the key to lookup
            return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName }";
        } 

        /// <summary>
        /// Load the text file
        /// </summary>
        /// <param name="file">file is the full path name + the file name</param>
        /// <returns>A list of the contents of the file</returns>
        public static List<string> LoadFile(this string file)
        {
            // using '!' is accepted more in production than '== false'
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        /// <summary>
        /// Convert the loaded file into a PrizeModel
        /// </summary>
        /// <param name="lines">The contents of the file</param>
        /// <returns>The converted data</returns>
        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            // loop through all lines in the file
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PrizeModel p = new PrizeModel();
                // Allow the program to crash if the first column is not an integer
                // Error handling will have to be done higher up the chain
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = int.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);

                output.Add(p);

            }

            return output;
        }

        /// <summary>
        /// Convert the loaded file into a PersonModel
        /// </summary>
        /// <param name="lines">The contents of the file</param>
        /// <returns>The converted data</returns>
        public static List<PersonModel> ConvertToPeopleModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PersonModel p = new PersonModel();
                // Allow the program to crash if the first column is not an integer
                // Error handling will have to be done higher up the chain
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellphoneNumber = cols[4];

                output.Add(p);
            }

            return output;
        }

        /// <summary>
        /// Convert the loaded file into a TeamModel
        /// </summary>
        /// <param name="lines">The contents of the file</param>
        /// <returns>The converted data</returns>
        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
        {
            //id,team name, list of ids separated by the pipe character
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPeopleModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TeamModel t = new TeamModel();
                // Allow the program to crash if the first column is not an integer
                // Error handling will have to be done higher up the chain
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];
                string[] personIds = cols[2].Split('|');
                foreach (string id in personIds)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }

                output.Add(t);
            }

            return output;
        }

        public static List<TournamentModel> ConvertToTournamentModels(
            this List<string> lines, 
            string teamsFileName, 
            string peopleFileName,
            string prizeFileName)
        {
            // Id,TournamentName,EntryFee,(id|id|id - Entered Teams),(id|id|id - Prizes),(Rounds - id^id^id^id^id^id|id^id^id^id|id^id)
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = teamsFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<PrizeModel> prizes = prizeFileName.FullFilePath().LoadFile().ConvertToPrizeModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TournamentModel tm = new TournamentModel();
                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);
                string[] teamIds = cols[3].Split('|');
                foreach (string id in teamIds)
                {
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }
                string[] prizeIds = cols[4].Split('|');
                foreach (string id in prizeIds)
                {
                    tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                }

                // TODO: Capture Rounds information

                output.Add(tm);
            }

            return output;
        }

        /// <summary>
        /// Saving the prizes data to the file
        /// </summary>
        /// <param name="models">The PrizeModel</param>
        /// <param name="fileName">The PrizeModels.csv</param>
        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach (PrizeModel p in models)
            {
                lines.Add($"{ p.Id },{ p.PlaceNumber },{ p.PlaceName },{ p.PrizeAmount },{ p.PrizePercentage }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        /// <summary>
        /// Saving the people data to the file
        /// </summary>
        /// <param name="models">The PersonModel</param>
        /// <param name="fileName">The PersonModels.csv</param>
        public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach (PersonModel p in models)
            {
                lines.Add($"{ p.Id },{ p.FirstName },{ p.LastName },{ p.EmailAddress },{ p.CellphoneNumber }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        /// <summary>
        /// Saving the team data to the file
        /// </summary>
        /// <param name="models">The TeamModel</param>
        /// <param name="fileName">The TeamModels.csv</param>
        public static void SaveToTeamsFile(this List<TeamModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach (TeamModel t in models)
            {

                lines.Add($"{ t.Id },{ t.TeamName },{ ConvertPeopleListToString(t.TeamMembers) }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        /// <summary>
        /// Saving the tournaments data to the file
        /// </summary>
        /// <param name="models">The TournamentModel</param>
        /// <param name="fileName">The TournamentModels.csv</param>
        public static void SaveToTournamentsFile(this List<TournamentModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach (TournamentModel tm in models)
            {                
                lines.Add($"{ tm.Id }," +
                    $"{ tm.TournamentName }," +
                    $"{ tm.EntryFee }," +
                    $"{ ConvertTeamListToString(tm.EnteredTeams) }," +
                    $"{ ConvertPrizeListToString(tm.Prizes) }," +
                    $"{ ConvertRoundListToString(tm.Rounds) }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        /// <summary>
        /// Converting the list of teammembers to a pipe separated string
        /// </summary>
        /// <param name="people">The list of teammembers</param>
        /// <returns>The converted list</returns>
        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";

            if (people.Count == 0)
            {
                return "";
            }

            foreach (PersonModel p in people)
            {
                output += $"{ p.Id }|";
            }

            // Remove the trailing |
            output = output.Substring(0, output.Length - 1);
            return output;
        }

        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";

            if (teams.Count == 0)
            {
                return "";
            }

            foreach (TeamModel t in teams)
            {
                output += $"{ t.Id }|";
            }

            // Remove the trailing |
            output = output.Substring(0, output.Length - 1);
            return output;
        }

        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";

            if (prizes.Count == 0)
            {
                return "";
            }

            foreach (PrizeModel p in prizes)
            {
                output += $"{ p.Id }|";
            }

            // Remove the trailing |
            output = output.Substring(0, output.Length - 1);
            return output;
        }

        private static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            string output = "";

            if (rounds.Count == 0)
            {
                return "";
            }

            foreach (List<MatchupModel> r in rounds)
            {
                output += $"{ ConvertMatchupListToString(r) }|";
            }

            // Remove the trailing |
            output = output.Substring(0, output.Length - 1);
            return output;
        }

        private static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            string output = "";

            if (matchups.Count == 0)
            {
                return "";
            }

            foreach (MatchupModel m in matchups)
            {
                output += $"{ m.Id }^";
            }

            // Remove the trailing |
            output = output.Substring(0, output.Length - 1);
            return output;
        }
    }
}
