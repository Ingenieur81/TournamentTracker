using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    /// <summary>
    /// Contains all logic for the determining matchups, winners, scoring, etc
    /// </summary>
    public static class TournamentLogic
    {
        // Order team list randomly
        // Check if the list is big enough - if not, add in byes - i.e. 2*2*2*2 => 2^4
        // Create the first round of matchups
        // Create every round after that -> 8 matchups -> 4 matchups -> 2 matchups -> 1 matchup

            /// <summary>
            /// Determine the matchups for the first round
            /// </summary>
            /// <param name="model"></param>
        public static void CreateRounds(TournamentModel model)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds, randomizedTeams.Count);

            model.Rounds.Add(CreateFirstRound(byes, randomizedTeams));

            CreateRemainingRounds(model, rounds);
            // pass-back to the model is not necessary because we are working on the
            // instance of this model directly

            UpdateTournamentResults(model);
        }

        public static void UpdateTournamentResults(TournamentModel model)
        {
            List<MatchupModel> toScore = new List<MatchupModel>();

            foreach (List<MatchupModel> rounds in model.Rounds)
            {
                foreach (MatchupModel rm in rounds)
                {
                    if (rm.Winner == null && (rm.Entries.Any(x => x.Score != 0) || rm.Entries.Count == 1))
                    {
                        toScore.Add(rm);
                    }
                }
            }

            markWinnerInMatchups(toScore);

            AdvanceWinners(toScore, model);

            // Update the data source
            // oneliner for a foreach
            toScore.ForEach(x => GlobalConfig.Connection.UpdateMatchup(x));
        }

        private static void AdvanceWinners(List<MatchupModel> models, TournamentModel tournament)
        {
            foreach (MatchupModel m in models)
            {
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
                                    // Update the data source
                                    GlobalConfig.Connection.UpdateMatchup(rm);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private static void markWinnerInMatchups(List<MatchupModel> models)
        {
            string greaterWins = ConfigurationManager.AppSettings["greaterWins"];

            foreach (MatchupModel m in models)
            {
                // Checks for bye week entry
                if (m.Entries.Count == 1)
                {
                    m.Winner = m.Entries[0].TeamCompeting;
                    continue;
                }
                if (greaterWins == "0")
                {
                    // 0 means false, or lower score wins
                    if (m.Entries[0].Score < m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else if (m.Entries[1].Score < m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("We do not allow tied games in this application!");
                    }
                }
                else
                {
                    // 1 means true, or high score wins
                    if (m.Entries[0].Score > m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else if (m.Entries[1].Score > m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("We do not allow tied games in this application!");
                    }
                } 
            }
            // High score wins, so e.g. golf is not supported
            //if (teamOneScore > teamTwoScore || teamTwoName.Text == "<Bye>")
            //{
            // Team one wins or has the bye
            //    m.Winner = m.Entries[0].TeamCompeting;
            //}
            //else if (teamTwoScore > teamOneScore)
            //{
            //    // Team two wins
            //    m.Winner = m.Entries[1].TeamCompeting;
            //}
            //else
            //{
            //    MessageBox.Show("This is a tie game, marking team 2 (the away team) as the winner");
            //    m.Winner = m.Entries[1].TeamCompeting;
            //}
        }

        private static void CreateRemainingRounds(TournamentModel model, int rounds)
        {
            int round = 2; // round 1 is already created by the CreateFirstRound method
            List<MatchupModel> previousRound = model.Rounds[0]; //populated with the first round initially
            List<MatchupModel> currentRound = new List<MatchupModel>();
            MatchupModel currentMatchup = new MatchupModel();

            while (round <= rounds)
            {
                foreach (MatchupModel match in previousRound)
                {
                    currentMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });

                    if (currentMatchup.Entries.Count > 1)
                    {
                        currentMatchup.MatchupRound = round; // set the current round we are in
                        currentRound.Add(currentMatchup); // added the matchup entry to the round
                        currentMatchup = new MatchupModel();
                    }
                }

                model.Rounds.Add(currentRound); // Add the current round to the model
                previousRound = currentRound; // set the previous round to the current round to go further
                currentRound = new List<MatchupModel>(); // clean up current round for the next loop
                round += 1;

            }
            
        }

        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            MatchupModel currentMatchup = new MatchupModel();

            foreach (TeamModel team in teams)
            {
                currentMatchup.Entries.Add(new MatchupEntryModel { TeamCompeting = team });
                // When there is a bye there will only be 1 entry in the MatchupEntryModel
                if (byes > 0 || currentMatchup.Entries.Count > 1)
                {
                    currentMatchup.MatchupRound = 1; // This is the first round, thus hardcoded 1
                    output.Add(currentMatchup);
                    currentMatchup = new MatchupModel();
                    if (byes > 0)
                    {
                        byes -= 1;
                    }
                }
            }

            return output;
        }
        
        private static int NumberOfByes(int rounds, int numberOfTeams)
        {
            int output = 0;
            int totalTeams = 1;

            for (int i = 1; i <= rounds; i++)
            {
                totalTeams *= 2; // determine the power to get 2*2*2*2 => 2^4 for example
            }

            output = totalTeams - numberOfTeams;

            return output;
        }

        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1; // There is always 1 round ...
            int val = 2; // for 2 teams, otherwise there is no tournament

            while (val < teamCount)
            {

                output += 1; // Add another round

                val *= 2; // Always needs to be an even number
            }

            return output;
        }

        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            // This ordering should be random enough
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }
    }
}
