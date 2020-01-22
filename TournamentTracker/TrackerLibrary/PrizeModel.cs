using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents one prize
    /// </summary>
    public class PrizeModel
    {
        /// <summary>
        /// Represents the number of the place has finished in
        /// i.e. 1, 2, 3, etc
        /// </summary>
        public int PlaceNumber { get; set; }

        /// <summary>
        /// Represents the name of the place number
        /// i.e. 1st = Champion, 2nd = Runner up, etc
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// Represents the amount of the prize won
        /// If specified PrizePercentage is not allowed
        /// </summary>
        public decimal PrizeAmount { get; set; }

        /// <summary>
        /// Represents the percentage of the total prizemoney won
        /// If specified PrizeAmount is not allowed
        /// </summary>
        public double PrizePercentage { get; set; }
    }
}
