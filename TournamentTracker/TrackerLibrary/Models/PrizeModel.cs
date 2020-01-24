using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one prize
    /// </summary>
    public class PrizeModel
    {
        /// <summary>
        /// Represents the unique identifier for the prize
        /// </summary>
        public int Id { get; set; }

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

        public PrizeModel()
        {

        }
        /// <summary>
        /// Convert all values from the form to the model
        /// </summary>
        /// <param name="placeNumber">The converted place number</param>
        /// <param name="placeName">The place name, no need to be converted</param>
        /// <param name="prizeAmount">The converted prize amount</param>
        /// <param name="prizePercentage">The converted prize percentage</param>
        public PrizeModel(string placeNumber, string placeName, string prizeAmount, string prizePercentage)
        {
            
            int placeNumberValue = 0;
            int.TryParse(placeNumber, out placeNumberValue);
            PlaceNumber = placeNumberValue;
            
            PlaceName = placeName;

            decimal prizeAmountValue = 0;
            decimal.TryParse(prizeAmount, out prizeAmountValue);
            PrizeAmount = prizeAmountValue;

            double prizePercentageValue = 0;
            double.TryParse(prizePercentage, out prizePercentageValue);
            PrizePercentage = prizePercentageValue;

        }
    }
}
