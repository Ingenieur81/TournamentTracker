using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one tournament
    /// </summary>
    public class TournamentModel
    {
        /// <summary>
        /// Fire when the tournament is marked complete
        /// </summary>
        public event EventHandler<DateTime> OnTournamentComplete;

        /// <summary>
        /// Represents the unique identifier for the tournament
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the name of the tournament
        /// </summary>
        public string TournamentName { get; set; }

        /// <summary>
        /// Represents the fee teams need to 
        /// pay to enter the tournament
        /// </summary>
        public decimal EntryFee { get; set; }

        /// <summary>
        /// Represents the teams which have entered
        /// in the tournament
        /// </summary>
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();

        /// <summary>
        /// Represents the prizes to be won in 
        /// the tournament
        /// </summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();

        /// <summary>
        /// Represents the matchups of teams for 
        /// all rounds in this tournament
        /// </summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();

        /// <summary>
        /// Invoke the event
        /// </summary>
        public void CompleteTournament()
        {
            // The '?' means if it is available (i.e. there are subscribers to the event) do the extension. 
            // If not, don't do anything
            OnTournamentComplete?.Invoke(this, DateTime.Now);
        }
    }
}
