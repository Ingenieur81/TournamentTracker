using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    /// <summary>
    /// Represents the interface for the dataconnection for each model
    /// Refactored, returning the model is not required
    /// Interfaces are always public
    /// </summary>
    public interface IDataConnection
    {
        void CreatePrize(PrizeModel model);

        void CreatePerson(PersonModel model);

        void CreateTeam(TeamModel model);

        void CreateTournament(TournamentModel model);

        void UpdateMatchup(MatchupModel model);

        List<PersonModel> GetPerson_All();

        List<TeamModel> GetTeam_All();

        List<TournamentModel> GetTournament_All();
    }
}
