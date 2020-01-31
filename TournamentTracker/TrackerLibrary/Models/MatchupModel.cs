using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one match in the tournament
    /// </summary>
    public class MatchupModel
    {
        /// <summary>
        /// Represents the unique identifier for the matchup
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the set of teams in the match
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

        /// <summary>
        /// Represents the unique identifier for the Winner
        /// Only used when bringing it in from the SQL database
        /// </summary>
        public int WinnerId { get; set; }

        /// <summary>
        /// Represents the winner of the match
        /// </summary>
        public TeamModel Winner { get; set; }

        /// <summary>
        /// Represents the round the match is played in
        /// </summary>
        public int MatchupRound { get; set; }

        /// <summary>
        /// Represent the display name of the matchup
        /// </summary>
        public string DisplayName
        {
            get
            {
                string output = "";
                foreach (MatchupEntryModel me in Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        if (output.Length == 0)
                        {
                            output = me.TeamCompeting.TeamName;
                        }
                        else
                        {
                            output += $" vs. { me.TeamCompeting.TeamName }";
                        } 
                    }
                    else
                    {
                        output = "Matchup not yet determined";
                        break;
                    }
                }

                return output;
            }
        }
    }
}
