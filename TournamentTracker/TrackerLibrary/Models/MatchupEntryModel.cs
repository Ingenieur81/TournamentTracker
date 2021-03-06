﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one team in the matchup
    /// </summary>
    public class MatchupEntryModel
    {
        /// <summary>
        /// Represents the unique identifier for the matchup entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the unique identifier for the competing team
        /// Only used when bringing it in from the SQL database
        /// </summary>
        public int TeamCompetingID { get; set; }

        /// <summary>
        /// Represents the competing team
        /// </summary>
        public TeamModel TeamCompeting { get; set; }

        /// <summary>
        /// Represents the score for this particular team
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Represents the unique identifier for the parent matchup
        /// Only used when bringing it in from the SQL database
        /// </summary>
        public int ParentMatchupID { get; set; }

        /// <summary>
        /// Represents the matchup that this team came 
        /// from as the winner
        /// </summary>
        public MatchupModel ParentMatchup { get; set; }
        
    }
}
