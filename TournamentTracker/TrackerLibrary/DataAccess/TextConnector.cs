using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector: IDataConnection
    {
        // Refactored
        
        /// <summary>
        /// Saves a prize to the text file
        /// </summary>
        /// <param name="model">The prize information model</param>
        /// <returns>The prize information, including the unique identifier</returns>
        public void CreatePrize(PrizeModel model)
        {
            // Load the text file and convert text to list<PrizeModel>
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // Find the max ID and add 1 to it
            int currentId = 1;
            if (prizes.Count > 0)
            {
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            // if adding more records in one pass add "currentId += 1;"
            // Add the new record with new ID
            prizes.Add(model);

            // Convert the prizes to list<string>
            // Save the list<string> to the text file
            prizes.SaveToPrizeFile();
        }

        /// <summary>
        /// Saves a person to the text file
        /// </summary>
        /// <param name="model">The person information model</param>
        /// <returns>The person information, including the unique identifier</returns>
        public void CreatePerson(PersonModel model)
        {
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPeopleModels();

            // Find the max ID and add 1 to it
            int currentId = 1;
            if (people.Count > 0)
            {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            // if adding more records in one pass add "currentId += 1;"
            // Add the new record with new ID
            people.Add(model);

            // Convert the people to list<string>
            // Save the list<string> to the text file
            people.SaveToPeopleFile();
        }

        /// <summary>
        /// Get the person data from the text file
        /// </summary>
        /// <returns>The PersonModel data</returns>
        public List<PersonModel> GetPerson_All()
        {
            return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPeopleModels();
        }

        /// <summary>
        /// Saves a team to the text file
        /// </summary>
        /// <param name="model">The team information model</param>
        /// <returns>The team information, including the unique identifier</returns>
        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
            
            // Find the max ID and add 1 to it
            int currentId = 1;
            if (teams.Count > 0)
            {
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            // if adding more records in one pass add "currentId += 1;"
            // Add the new record with new ID
            teams.Add(model);

            // Convert the teams to list<string>
            // Save the list<string> to the text file
            teams.SaveToTeamsFile();
        }

        /// <summary>
        /// Get the team data from the text file
        /// </summary>
        /// <returns>The TeamModel data</returns>
        public List<TeamModel> GetTeam_All()
        {
            return GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
        }

        /// <summary>
        /// Save a tournament to the text file
        /// </summary>
        /// <param name="model"></param>
        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();

            // Find the max ID and add 1 to it
            int currentId = 1;
            if (tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;

            // Populate matchup ids
            model.SaveRoundsToFile();

            tournaments.Add(model);

            tournaments.SaveToTournamentsFile();

            TournamentLogic.UpdateTournamentResults(model);

        }

        public List<TournamentModel> GetTournament_All()
        {
            return GlobalConfig.TournamentsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();
        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }

        public void CompleteTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();
            
            tournaments.Remove(model);

            tournaments.SaveToTournamentsFile();

            TournamentLogic.UpdateTournamentResults(model);
        }
    }
}
