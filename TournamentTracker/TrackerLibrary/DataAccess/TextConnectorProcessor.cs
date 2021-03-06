﻿using System;
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
        public static List<TeamModel> ConvertToTeamModels(this List<string> lines)
        {
            //id,team name, list of ids separated by the pipe character
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPeopleModels();

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

        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines)
        {
            // Id,TournamentName,EntryFee,(id|id|id - Entered Teams),(id|id|id - Prizes),(Rounds - id^id^id^id^id^id|id^id^id^id|id^id)
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();

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
                if (cols[4].Length > 0)
                {
                    string[] prizeIds = cols[4].Split('|');
                    foreach (string id in prizeIds)
                    {
                        tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                    } 
                }

                // Get the list of lists of matchups
                string[] rounds = cols[5].Split('|');

                // Loop through the array of lists of matchups
                foreach (string round in rounds)
                {
                    string[] ms = round.Split('^');
                    List<MatchupModel> mm = new List<MatchupModel>();

                    // Loop through the array of matchups
                    foreach (string matchupModelTextId in ms)
                    {
                        // Get the matchup id which matches the id of the matchup model
                        mm.Add(matchups.Where(x => x.Id == int.Parse(matchupModelTextId)).First());
                    }

                    tm.Rounds.Add(mm);
                }

                output.Add(tm);
            }

            return output;
        }

        /// <summary>
        /// Saving the prizes data to the file
        /// </summary>
        /// <param name="models">The PrizeModel</param>
        public static void SaveToPrizeFile(this List<PrizeModel> models)
        {
            List<string> lines = new List<string>();
            foreach (PrizeModel p in models)
            {
                lines.Add($"{ p.Id },{ p.PlaceNumber },{ p.PlaceName },{ p.PrizeAmount },{ p.PrizePercentage }");
            }

            File.WriteAllLines(GlobalConfig.PrizesFile.FullFilePath(), lines);
        }

        /// <summary>
        /// Saving the people data to the file
        /// </summary>
        /// <param name="models">The PersonModel</param>
        public static void SaveToPeopleFile(this List<PersonModel> models)
        {
            List<string> lines = new List<string>();
            foreach (PersonModel p in models)
            {
                lines.Add($"{ p.Id },{ p.FirstName },{ p.LastName },{ p.EmailAddress },{ p.CellphoneNumber }");
            }

            File.WriteAllLines(GlobalConfig.PeopleFile.FullFilePath(), lines);
        }

        /// <summary>
        /// Saving the team data to the file
        /// </summary>
        /// <param name="models">The TeamModel</param>
        public static void SaveToTeamsFile(this List<TeamModel> models)
        {
            List<string> lines = new List<string>();
            foreach (TeamModel t in models)
            {

                lines.Add($"{ t.Id },{ t.TeamName },{ ConvertPeopleListToString(t.TeamMembers) }");
            }

            File.WriteAllLines(GlobalConfig.TeamsFile.FullFilePath(), lines);
        }

        /// <summary>
        /// Saving the tournaments data to the file
        /// </summary>
        /// <param name="models">The TournamentModel</param>
        public static void SaveToTournamentsFile(this List<TournamentModel> models)
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

            File.WriteAllLines(GlobalConfig.TournamentsFile.FullFilePath(), lines);
        }

        /// <summary>
        /// This method represents the process of saving the matchups to file
        /// It takes input from several different places, which differs from the
        /// other SaveTo methods
        /// </summary>
        /// <param name="model">The TournamentModel, which contains all matchups</param>
        public static void SaveRoundsToFile(this TournamentModel model)
        {
            // Loop trnough each round
            // Loop through each matchup
            // Get the ID for the new matchup and save the matchup record
            // Loop through each entry, get the ID and save the entry

            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    // Load all of the matchups from file
                    // Get the top ID and add one
                    // Store the ID
                    // Save the matchup record
                    matchup.SaveMatchupToFile();

                }
            }
        }

        private static List<MatchupEntryModel> ConvertStringToMatchupEntryModel(string input)
        {
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<string> entries = GlobalConfig
                .MatchupEntriesFile
                .FullFilePath()
                .LoadFile(); // Not all matches need to be loaded, just the ones we need
            List<string> matchingEntries = new List<string>();

            foreach (string id in ids)
            {
                foreach (string entry in entries)
                {
                    string[] cols = entry.Split(',');

                    if (cols[0] == id)
                    {
                        matchingEntries.Add(entry);
                    }
                }
            }

            // Only convert the actual needed matching entries to a MatchupEntryModel
            output = matchingEntries.ConvertToMatchupEntryModel();

            return output;
        }

        public static List<MatchupEntryModel> ConvertToMatchupEntryModel(this List<string> lines)
        {
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                MatchupEntryModel me = new MatchupEntryModel();

                me.Id = int.Parse(cols[0]);
                if (cols[1].Length == 0)
                {
                    me.TeamCompeting = null;
                }
                else
                {
                    me.TeamCompeting = LookupTeamById(int.Parse(cols[1]));
                }
                
                me.Score = double.Parse(cols[2]);
                int parentId = 0;
                if (int.TryParse(cols[3], out parentId)) // Parent Matchup is null in the first round
                {
                    me.ParentMatchup = LookupMatchupById(parentId);
                }
                else
                {
                    me.ParentMatchup = null;
                }

                output.Add(me);
            }

            return output;
        }

        private static TeamModel LookupTeamById(int id)
        {
            List<string> teams = GlobalConfig
                .TeamsFile.FullFilePath()
                .LoadFile(); // Not all matches need to be loaded, just the ones we need

            foreach (string team in teams)
            {
                string[] cols = team.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);
                    return matchingTeams.ConvertToTeamModels().First();
                }
            }

            // No team found, i.e. error
            return null;
        }

        private static MatchupModel LookupMatchupById(int id)
        {
            List<string> matchups = GlobalConfig
                .MatchupsFile
                .FullFilePath()
                .LoadFile(); // Not all matches need to be loaded, just the ones we need

            foreach (string matchup in matchups)
            {
                string[] cols = matchup.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }

            // No matchup found, i.e. error
            return null;
        }

        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            List<MatchupModel> output = new List<MatchupModel>();

            // loop through all lines in the file
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                MatchupModel p = new MatchupModel();
                // Allow the program to crash if the first column is not an integer
                // Error handling will have to be done higher up the chain
                p.Id = int.Parse(cols[0]);
                p.Entries = ConvertStringToMatchupEntryModel(cols[1]); //Pipe delimited by ID
                if (cols[2].Length == 0)
                {
                    p.Winner = null;
                }
                else
                {
                    p.Winner = LookupTeamById(int.Parse(cols[2]));
                }
                
                p.MatchupRound = int.Parse(cols[3]);

                output.Add(p);

            }

            return output;
        }

        public static void SaveMatchupToFile(this MatchupModel matchup)
        {
            List<MatchupModel> matchups = GlobalConfig
                .MatchupsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToMatchupModels();

            int currentId = 1;
            if (matchups.Count > 0)
            {
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            }

            matchup.Id = currentId;

            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.SaveMatchupEntryToFile();
            }

            // Save to file
            List<string> lines = new List<string>();
            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null) // winner id can be null
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{ m.Id },{ ConvertMatchupEntryListToString(m.Entries) },{ winner },{ m.MatchupRound }");
            }

            File.WriteAllLines(GlobalConfig.MatchupsFile.FullFilePath(), lines);
        }

        public static void SaveMatchupEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModel();

            int currentId = 1;
            if (entries.Count > 0)
            {
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
            }

            entry.Id = currentId;
            entries.Add(entry);

            List<string> lines = new List<string>();
            foreach (MatchupEntryModel e in entries)
            {
                string parent = "";
                if (e.ParentMatchup != null) // parent matchup id can be null
                {
                    parent = e.ParentMatchup.Id.ToString();
                }
                string teamCompeting = "";
                if (e.TeamCompeting != null)
                {
                    teamCompeting = e.TeamCompeting.Id.ToString();
                }
                lines.Add($"{ e.Id },{ teamCompeting },{ e.Score },{ parent }");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);

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

            // Remove the trailing ^
            output = output.Substring(0, output.Length - 1);
            return output;
        }

        private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
        {
            string output = "";

            if (entries.Count == 0)
            {
                return "";
            }

            foreach (MatchupEntryModel e in entries)
            {
                output += $"{ e.Id }|";
            }

            // Remove the trailing |
            output = output.Substring(0, output.Length - 1);
            return output;
        }

        public static void UpdateMatchupToFile(this MatchupModel matchup)
        {
            List<MatchupModel> matchups = GlobalConfig
                .MatchupsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToMatchupModels();

            MatchupModel oldMatchup = new MatchupModel();
            foreach (MatchupModel m in matchups)
            {
                if (m.Id == matchup.Id)
                {
                    oldMatchup = m;
                }
            }

            matchups.Remove(oldMatchup);
            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.UpdateMatchupEntryToFile();
            }

            // Save to file
            List<string> lines = new List<string>();
            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null) // winner id can be null
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{ m.Id },{ ConvertMatchupEntryListToString(m.Entries) },{ winner },{ m.MatchupRound }");
            }

            File.WriteAllLines(GlobalConfig.MatchupsFile.FullFilePath(), lines);
        }

        public static void UpdateMatchupEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModel();

            MatchupEntryModel oldMatchupEntry = new MatchupEntryModel();

            foreach (MatchupEntryModel me in entries)
            {
                if (me.Id == entry.Id)
                {
                    oldMatchupEntry = me;
                }
            }

            entries.Remove(oldMatchupEntry);
            entries.Add(entry);

            List<string> lines = new List<string>();
            foreach (MatchupEntryModel e in entries)
            {
                string parent = "";
                if (e.ParentMatchup != null) // parent matchup id can be null
                {
                    parent = e.ParentMatchup.Id.ToString();
                }
                string teamCompeting = "";
                if (e.TeamCompeting != null)
                {
                    teamCompeting = e.TeamCompeting.Id.ToString();
                }
                lines.Add($"{ e.Id },{ teamCompeting },{ e.Score },{ parent }");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);

        }
    }
}
