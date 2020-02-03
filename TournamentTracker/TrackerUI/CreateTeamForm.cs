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
    public partial class CreateTeamForm : Form
    {
        /// <summary>
        /// Get the people from the datasource and put it in the available members 
        /// dropdown list
        /// </summary>
        private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
        private List<PersonModel> selectedTeamMembers = new List<PersonModel>();
        private ITeamRequester callingForm;

        /// <summary>
        /// Represents the form to create a new team
        /// </summary>
        public CreateTeamForm(ITeamRequester caller)
        {
            InitializeComponent();

            callingForm = caller;
            //CreateSampleData();

            WireUpLists();
        }

        /// <summary>
        /// Sample data, not used in production
        /// </summary>
        private void CreateSampleData()
        {
            availableTeamMembers.Add(new PersonModel { FirstName = "Gertjan", LastName = "Horlings" });
            availableTeamMembers.Add(new PersonModel { FirstName = "Sue", LastName = "Storm" });

            selectedTeamMembers.Add(new PersonModel { FirstName = "Jane", LastName = "Doe" });
            selectedTeamMembers.Add(new PersonModel { FirstName = "Bill", LastName = "Jones" });
        }

        /// <summary>
        /// Fill the textboxes with the available and selected data
        /// </summary>
        private void WireUpLists()
        {
            // The datasource needs to be set to null first so the data 
            // will be refreshed when setting in the next step
            selectTeamMemberDropdown.DataSource = null;
            selectTeamMemberDropdown.DataSource = availableTeamMembers;
            selectTeamMemberDropdown.DisplayMember = "FullName";

            // The datasource needs to be set to null first so the data 
            // will be refreshed when setting in the next step
            teamMembersListbox.DataSource = null;
            teamMembersListbox.DataSource = selectedTeamMembers;
            teamMembersListbox.DisplayMember = "FullName";
            
        }

        /// <summary>
        /// Represents the form to add a new member 
        /// when he is not in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel p = new PersonModel();

                p.FirstName = firstNameValue.Text;
                p.LastName = lastNameValue.Text;
                p.EmailAddress = emailValue.Text;
                p.CellphoneNumber = cellphoneValue.Text;

                GlobalConfig.Connection.CreatePerson(p);

                selectedTeamMembers.Add(p);

                WireUpLists();

                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellphoneValue.Text = "";
            }
            else
            {
                MessageBox.Show("You need to fill in all of the fields!");
            }
        }

        /// <summary>
        /// Validates if the values in the form are in the correct format
        /// </summary>
        /// <returns>true or false</returns>
        private bool ValidateForm()
        {
            // Just checking if there is something in the fields
            // TODO: make validation more robust, checking @ and . in the email address
            // TODO: Add fields with error messages behind person input text boxes
            if (firstNameValue.Text.Length == 0)
            {
                return false;
            }

            if (lastNameValue.Text.Length == 0)
            {
                return false;
            }

            if (emailValue.Text.Length == 0)
            {
                return false;
            }

            if (cellphoneValue.Text.Length == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add the selected member to the selected list and
        /// remove it from the available list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addTeamMemberButton_Click(object sender, EventArgs e)
        {
            /* Casting the SelectedItem object to the specific PersonModel, this is correct
             * because the selected item will be shoehorned into a PersonModel but if it does
             * not work the program will crash. But it WILL work, we filled in the dropdown with
             * the PersonModel
            */
            PersonModel p = (PersonModel)selectTeamMemberDropdown.SelectedItem;

            // Clicking the button while no member is selected will cause problems
            if (p != null)
            {
                // Taken the person object, removed from available list and added to selected list
                availableTeamMembers.Remove(p);
                selectedTeamMembers.Add(p);

                // Recall the wireup method to refresh the dropdown and listbox.
                WireUpLists(); 
            }
        }

        /// <summary>
        /// Remove the selected member from the selected list and
        /// add it back to the available list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)teamMembersListbox.SelectedItem;

            // Clicking the button while no member is selected will cause problems
            if (p != null)
            {
                // Taken the person object, removed from available list and added to selected list
                selectedTeamMembers.Remove(p);
                availableTeamMembers.Add(p);

                // Recall the wireup method to refresh the dropdown and listbox.
                WireUpLists(); 
            }
        }

        /// <summary>
        /// Add the team and its members to the tournament
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = new TeamModel();

            t.TeamName = teamNameValue.Text;
            t.TeamMembers = selectedTeamMembers;

            GlobalConfig.Connection.CreateTeam(t);

            callingForm.TeamComplete(t);

            this.Close();
        }
    }
}
