using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreatePrizeForm : Form
    {
        /// <summary>
        /// Represents a form to create a new prize
        /// for the tournament and add it to the prizes
        /// list in the Create Tournament Form
        /// </summary>
        public CreatePrizeForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event to do when the Add prize button is clicked
        /// </summary>
        private void addPrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PrizeModel model = new PrizeModel(
                    placeNumberValue.Text,
                    placeNameValue.Text, 
                    prizeAmountValue.Text, 
                    prizePercentageValue.Text);

                GlobalConfig.Connection.CreatePrize(model);

                placeNumberValue.Text = "";
                placeNameValue.Text = "";
                prizeAmountValue.Text = "0";
                prizePercentageValue.Text = "0";

            }
            else
            {
                MessageBox.Show("The inputs are invalid.");
            }

        }
        /// <summary>
        /// Validates if the values in the form are in the correct format
        /// </summary>
        /// <returns>true or false</returns>
        private bool ValidateForm()
        {
            bool output = true;

            int placeNumber = 0;
            bool placeNumberValidNumber = int.TryParse(placeNumberValue.Text, out placeNumber);

            if (placeNumberValidNumber == false)
            {
                // TODO: Add fields with error messages behind input text boxes
                output = false;
            }

            if (placeNumber < 1)
            {
                output = false;
            }

            if (placeNameValue.Text.Length == 0)
            {
                output = false;
            }

            decimal prizeAmount = 0;
            double prizePercentage = 0;
            bool prizeAmountValid = decimal.TryParse(prizeAmountValue.Text, out prizeAmount);
            bool prizePercentageValid = double.TryParse(prizePercentageValue.Text, out prizePercentage);

            if (prizeAmountValid == false || prizePercentageValid== false)
            {
                output = false;
            }

            if (prizeAmount <= 0 && prizePercentage <= 0)
            {
                output = false;
            }

            if (prizePercentage < 0 || prizePercentage > 100)
            {
                output = false;
            }

            return output;
        }
    }
}
