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
using TrackerLibrary.Models;

namespace TrackerUI
{
    
    public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester
    {
        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        List<TeamModel> selectedTeams = new List<TeamModel>();
        List<PrizeModel> selectedPrizes = new List<PrizeModel>();
        
        /// <summary>
        /// Represents the form to create a new tournament
        /// This should inititalize all methods used for the 
        /// textboxes and buttons
        /// </summary>
        public CreateTournamentForm()
        {
            InitializeComponent();

            WireUpLists();
        }

        /// <summary>
        /// Fill the textboxes with the available and selected data
        /// </summary>
        private void WireUpLists()
        {
            // The datasource needs to be set to null first so the data 
            // will be refreshed when setting in the next step
            selectTeamDropdown.DataSource = null;
            selectTeamDropdown.DataSource = availableTeams;
            selectTeamDropdown.DisplayMember = "TeamName";

            tournamentTeamsListBox.DataSource = null;
            tournamentTeamsListBox.DataSource = selectedTeams;
            tournamentTeamsListBox.DisplayMember = "TeamName";

            tournamentPrizesListbox.DataSource = null;
            tournamentPrizesListbox.DataSource = selectedPrizes;
            tournamentPrizesListbox.DisplayMember = "PlaceIdentifier";
        }

        /// <summary>
        /// Add the selected team to the selected list and
        /// remove it from the available list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)selectTeamDropdown.SelectedItem;

            // Clicking the button while no member is selected will cause problems
            if (t != null)
            {
                // remove from available and add to selected
                availableTeams.Remove(t);
                selectedTeams.Add(t);
                
                // Recall the wireup method to refresh the dropdown and listbox.
                WireUpLists();
            }
        }

        /// <summary>
        /// Go to the create new prize form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createNewPrizeLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Call the createPrize form
            CreatePrizeForm frm = new CreatePrizeForm(this);
            frm.Show();
                        
        }

        /// <summary>
        /// Get back a PrizeModel from the create prize form
        /// Then take the model and put it into the list of 
        /// selected prizes
        /// </summary>
        /// <param name="model">The PrizeModel</param>
        public void PrizeComplete(PrizeModel model)
        {
            selectedPrizes.Add(model);

            // Recall the wireup method to refresh the dropdown and listbox.
            WireUpLists();
        }

        /// <summary>
        /// Get back a TeamModel from the create team form
        /// Then take the model and put it into the list of 
        /// selected teams
        /// </summary>
        /// <param name="model">The TeamModel</param>
        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);

            // Recall the wireup method to refresh the dropdown and listbox.
            WireUpLists();
        }

        /// <summary>
        /// Go to the create new team form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm frm = new CreateTeamForm(this);
            frm.Show();
        }

        /// <summary>
        /// Remove the selected team from the selected list and
        /// add it back to the available list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)tournamentTeamsListBox.SelectedItem;
            if (t != null)
            {
                selectedTeams.Remove(t);
                availableTeams.Add(t);

                // Recall the wireup method to refresh the dropdown and listbox.
                WireUpLists();
            }
        }

        /// <summary>
        /// Remove the selected prize from the selected list 
        /// There is no available prizes list to add it back to
        /// 
        /// To keep the prize id's logical the prize is not deleted
        /// from the data source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removePrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)tournamentPrizesListbox.SelectedItem;
            if (p != null)
            {
                selectedPrizes.Remove(p);

                // Recall the wireup method to refresh the dropdown and listbox.
                WireUpLists();
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            // Validate form data
            decimal fee = 0;
            bool feeAcceptable = decimal.TryParse(entryFeeValue.Text, out fee);

            if (!feeAcceptable)
            {
                // TODO: This is a template for the other message boxes 
                MessageBox.Show("Entry fee is not a valid number",
                    "Invalid fee",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            // Create tournament model
            TournamentModel tm = new TournamentModel();

            tm.TournamentName = tournamentNameValue.Text;
            tm.EntryFee = fee;

            /* The selectedPrizes is a list and the tm.prizes expects a list
             * a foreach could be used to put everything form one list
             * into the other or ...
             *foreach (PrizeModel prize in selectedPrizes)
             *{
             *    tm.Prizes.Add(prize);
             *}
             * ... just set the list equal to the each other
            */
            tm.Prizes = selectedPrizes;
            tm.EnteredTeams = selectedTeams;

            // Wire up matchups
            TournamentLogic.CreateRounds(tm);

            // Create Tournament entry
            // Create all of the prize entries
            // Create all of the team entries
            GlobalConfig.Connection.CreateTournament(tm);

            // Email on the creation of the first round
            tm.AlertUsersToNewRound();

            TournamentViewerForm frm = new TournamentViewerForm(tm);
            frm.Show();
            this.Close();

        }
    }
}
