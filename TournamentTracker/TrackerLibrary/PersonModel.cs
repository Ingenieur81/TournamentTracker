﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents one person
    /// </summary>
    public class PersonModel
    {
        /// <summary>
        /// Represents the first name of the player
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Represents the last name of the player
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Represents the email address of the player
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Represents the cellphone number of the player
        /// Explicitly not a number as no mathematical 
        /// operations are performed on the number
        /// </summary>
        public string CellphoneNumber { get; set; }
    }
}
