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
        // using 'const' for the files to always use the same file 
        // and not allow overwriting the variable
        private const string PrizesFile = "PrizeModels.csv";
        private const string PeopleFile = "PersonModels.csv";
        private const string TeamsFile = "TeamModels.csv";
        private const string TournamentsFile = "TournamentModels.csv";

        /// <summary>
        /// Saves a prize to the text file
        /// </summary>
        /// <param name="model">The prize information model</param>
        /// <returns>The prize information, including the unique identifier</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            // Load the text file and convert text to list<PrizeModel>
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

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
            prizes.SaveToPrizeFile(PrizesFile);

            return model;
        }

        /// <summary>
        /// Saves a person to the text file
        /// </summary>
        /// <param name="model">The person information model</param>
        /// <returns>The person information, including the unique identifier</returns>
        public PersonModel CreatePerson(PersonModel model)
        {
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPeopleModels();

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
            people.SaveToPeopleFile(PeopleFile);

            return model;
        }

        /// <summary>
        /// Get the person data from the text file
        /// </summary>
        /// <returns>The PersonModel data</returns>
        public List<PersonModel> GetPerson_All()
        {
            return PeopleFile.FullFilePath().LoadFile().ConvertToPeopleModels();
        }

        /// <summary>
        /// Saves a team to the text file
        /// </summary>
        /// <param name="model">The team information model</param>
        /// <returns>The team information, including the unique identifier</returns>
        public TeamModel CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
            
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
            teams.SaveToTeamsFile(TeamsFile);

            return model;
        }

        /// <summary>
        /// Get the team data from the text file
        /// </summary>
        /// <returns>The TeamModel data</returns>
        public List<TeamModel> GetTeam_All()
        {
            return TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
        }

        /// <summary>
        /// Save a tournament to the text file
        /// </summary>
        /// <param name="model"></param>
        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = TournamentsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels(TeamsFile, PeopleFile, PrizesFile);

            // Find the max ID and add 1 to it
            int currentId = 1;
            if (tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;

            tournaments.Add(model);

            tournaments.SaveToTournamentsFile(TournamentsFile);
            
        }
    }
}
