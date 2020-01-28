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
    /// 
    /// Interfaces are always public
    /// </summary>
    public interface IDataConnection
    {
        PrizeModel CreatePrize(PrizeModel model);

        PersonModel CreatePerson(PersonModel model);
        
        TeamModel CreateTeam(TeamModel model);

        void CreateTournament(TournamentModel model);

        List<PersonModel> GetPerson_All();

        List<TeamModel> GetTeam_All();
    }
}
