using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents one match in the tournament
    /// </summary>
    public class MatchupModel
    {
        /// <summary>
        /// Represents the set of teams in the match
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

        /// <summary>
        /// Represents the winner of the match
        /// </summary>
        public TeamModel Winner { get; set; }

        /// <summary>
        /// Represents the round the match is played in
        /// </summary>
        public int MatchupRound { get; set; }
    }
}
