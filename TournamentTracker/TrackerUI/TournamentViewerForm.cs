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
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel tournament;
        BindingList<int> rounds = new BindingList<int>();
        BindingList<MatchupModel> selectedMatchups = new BindingList<MatchupModel>();

        /// <summary>
        /// Represents the form to view a tournament
        /// and all of its elements, like rounds, teams,
        /// scores, etc.
        /// </summary>
        public TournamentViewerForm(TournamentModel tournamentModel)
        {
            InitializeComponent();

            tournament = tournamentModel;

            WireUpLists();

            LoadFormData();

            LoadRounds();

        }

        private void WireUpLists()
        {
            roundDropdown.DataSource = null;
            roundDropdown.DataSource = rounds;

            matchupListBox.DataSource = null;
            matchupListBox.DataSource = selectedMatchups;
            matchupListBox.DisplayMember = "DisplayName";
        }

        /// <summary>
        /// Set the name of the tournament
        /// </summary>
        private void LoadFormData()
        {
            tournamentName.Text = tournament.TournamentName;
        }

        /// <summary>
        /// Get all the rounds in the tournament and put them in the
        /// dropdown list box
        /// </summary>
        private void LoadRounds()
        {
            // Clear out the the rounds at each call
            rounds.Clear();

            rounds.Add(1);
            int currentRound = 1;

            foreach (List<MatchupModel> matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound > currentRound)
                {
                    currentRound = matchups.First().MatchupRound;
                    rounds.Add(currentRound);
                }
            }
            LoadMatchups(1);
        }


        /// <summary>
        /// Load the matches belonging to the selected round
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropdown.SelectedItem);
        }

        /// <summary>
        /// Load the matchups in the selected round
        /// </summary>
        private void LoadMatchups(int round)
        {

            foreach (List<MatchupModel> matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound == round)
                {
                    // Calling the clear on a binding list used in a 
                    // listbox will call a SelectedItems event change,
                    // causing the list to be empty and causing a 
                    selectedMatchups.Clear();
                    foreach (MatchupModel m in matchups)
                    {
                        if (m.Winner == null || !unplayedOnlyCheckBox.Checked)
                        {
                            selectedMatchups.Add(m);
                        }
                    }
                }
            }

            if (selectedMatchups.Count > 0)
            {
                LoadMatchup(selectedMatchups.First());
            }

            DisplayMatchupInfo();
        }

        /// <summary>
        /// Check if there are values in the selected matchups and set visibility 
        /// based on the outcome of that check
        /// </summary>
        private void DisplayMatchupInfo()
        {
            bool isVisible = (selectedMatchups.Count > 0);

            teamOneName.Visible = isVisible;
            teamOneScoreLabel.Visible = isVisible;
            teamOneScoreValue.Visible = isVisible;
            teamTwoName.Visible = isVisible;
            teamTwoScoreLabel.Visible = isVisible;
            teamTwoScoreValue.Visible = isVisible;
            versusLabel.Visible = isVisible;
            scoreButton.Visible = isVisible;
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchup((MatchupModel)matchupListBox.SelectedItem);
        }

        /// <summary>
        /// Load the team names for the selected matchup
        /// </summary>
        private void LoadMatchup(MatchupModel m)
        {
            //MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;

            // When the clear method is called on the list, this event is triggered
            // which causes a null reference error
            if (m != null)
            {
                for (int i = 0; i < m.Entries.Count; i++)
                {
                    if (i == 0)
                    {
                        if (m.Entries[0].TeamCompeting != null)
                        {
                            teamOneName.Text = m.Entries[0].TeamCompeting.TeamName;
                            teamOneScoreValue.Text = m.Entries[0].Score.ToString();

                            teamTwoName.Text = "<Bye>"; // presetting the bye week
                            teamTwoScoreValue.Text = "";
                        }
                        else
                        {
                            teamOneName.Text = "Not yet set";
                            teamOneScoreValue.Text = "";
                        }
                    }

                    if (i == 1)
                    {
                        if (m.Entries[1].TeamCompeting != null)
                        {
                            teamTwoName.Text = m.Entries[1].TeamCompeting.TeamName;
                            teamTwoScoreValue.Text = m.Entries[1].Score.ToString();
                        }
                        else
                        {
                            teamTwoName.Text = "Not yet set";
                            teamTwoScoreValue.Text = "";
                        }
                    }
                }
            }
        }

        private void unplayedOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropdown.SelectedItem);
        }

        private void scoreButton_Click(object sender, EventArgs e)
        {
            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;
            double teamOneScore = 0;
            double teamTwoScore = 0;

            for (int i = 0; i < m.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        bool scoreValid = double.TryParse(teamOneScoreValue.Text, out teamOneScore);
                        if (scoreValid)
                        {
                            m.Entries[0].Score = teamOneScore;
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 1");
                            return;
                        }
                    }
                }

                if (i == 1)
                {
                    if (m.Entries[1].TeamCompeting != null)
                    {
                        bool scoreValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);
                        if (scoreValid)
                        {
                            m.Entries[1].Score = teamTwoScore;
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 2");
                            return;
                        }
                    }
                }
            }

            if (teamOneScore > teamTwoScore || teamTwoName.Text == "<Bye>") // High score wins, so e.g. golf is not supported
            {
                // Team one wins or has the bye
                m.Winner = m.Entries[0].TeamCompeting;
            }
            else if (teamTwoScore > teamOneScore)
            {
                // Team two wins
                m.Winner = m.Entries[1].TeamCompeting;
            }
            else
            {
                MessageBox.Show("This is a tie game, marking team 2 (the away team) as the winner");
                m.Winner = m.Entries[1].TeamCompeting;
            }
            // update the winner in the matchup and matchupentries
            foreach (List<MatchupModel> rounds in tournament.Rounds)
            {
                foreach (MatchupModel rm in rounds)
                {
                    foreach (MatchupEntryModel me in rm.Entries)
                    {
                        if (me.ParentMatchup != null)
                        {
                            if (me.ParentMatchup.Id == m.Id)
                            {
                                me.TeamCompeting = m.Winner;
                                GlobalConfig.Connection.UpdateMatchup(rm);
                            } 
                        }
                    }
                }
            }

            // update the dropdown
            LoadMatchups((int)roundDropdown.SelectedItem);

            GlobalConfig.Connection.UpdateMatchup(m);
        }
    }
}
