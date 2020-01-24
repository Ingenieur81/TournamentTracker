using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one team in the tournament
    /// </summary>
    public class TeamModel
    {
        /// <summary>
        /// Represents the members in the team
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();

        /// <summary>
        /// Represents the name of the team
        /// </summary>
        public string TeamName { get; set; }

    }
}
