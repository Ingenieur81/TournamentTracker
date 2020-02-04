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
        /// <param name="model">The tournament model</param>
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

        /// <summary>
        /// Represents the update of the tournaments with scores of played matches
        /// or the bye week
        /// And sends messages to users
        /// </summary>
        /// <param name="model">The tournament model</param>
        public static void UpdateTournamentResults(TournamentModel model)
        {
            int startingRound = model.CheckCurrentRound();
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
            // TODO: Update the data source and get back the updated ID's from the datasource

            AdvanceWinners(toScore, model);

            // Update the data source
            // oneliner for a foreach
            toScore.ForEach(x => GlobalConfig.Connection.UpdateMatchup(x));

            int endingRound = model.CheckCurrentRound();
            if (endingRound > startingRound)
            {
                // Email users
                model.AlertUsersToNewRound();
            }
        }

        public static void AlertUsersToNewRound(this TournamentModel model)
        {
            int currentRoundNumber = model.CheckCurrentRound();
            List<MatchupModel> currentRound = model.Rounds.Where(x => x.First().MatchupRound == currentRoundNumber).First();

            foreach (MatchupModel matchup in currentRound)
            {
                foreach (MatchupEntryModel me in matchup.Entries)
                {
                    foreach (PersonModel p in me.TeamCompeting.TeamMembers)
                    {
                        AlertPersonToNewRound(p, me.TeamCompeting.TeamName, matchup.Entries.Where(x => x.TeamCompeting != me.TeamCompeting).FirstOrDefault());
                    }
                }
            }
        }

        private static void AlertPersonToNewRound(PersonModel p, string teamName, MatchupEntryModel competitor)
        {
            // If there is no emailaddress, just return
            if (p.EmailAddress.Length == 0)
            {
                return;
            }
            
            string toAddress = "";
            string subject = "";
            StringBuilder body = new StringBuilder();

            if (competitor != null)
            {
                subject = $"You have a new matchup with: { competitor.TeamCompeting.TeamName }";
                body.AppendLine("<h1>You have a new matchup</h1>");
                body.AppendLine("<p><strong>Competitor: ");
                body.AppendLine(competitor.TeamCompeting.TeamName);
                body.AppendLine("</strong></p>");
                body.AppendLine();
                body.AppendLine("<p>Have a great time!</p>");
                body.AppendLine("<h2>~Tournament Tracker<h2/>");
            }
            else
            {
                subject = $"You have a bye week this round";
                body.AppendLine("<h1>Enjoy your round off</h1>");
                body.AppendLine();
                body.AppendLine("<h2>~Tournament Tracker<h2/>");
            }

            toAddress = p.EmailAddress;

            EmailLogic.SendEmail(toAddress, subject, body.ToString());
        }

        private static int CheckCurrentRound(this TournamentModel model)
        {
            int output = 1;

            foreach (List<MatchupModel> round in model.Rounds)
            {
                if (round.All(x => x.Winner != null))
                {
                    output += 1;
                }
                else
                {
                    return output;
                }
            }

            // Tournament is complete
            CompleteTournament(model);

            return output - 1; //this returns only in the last round therefore -1
        }

        private static void CompleteTournament(TournamentModel model)
        {
            GlobalConfig.Connection.CompleteTournament(model);
            TeamModel winners = model.Rounds.Last().First().Winner;
            TeamModel runnerUp = model.Rounds.Last().First().Entries.Where(x => x.TeamCompeting != winners).First().TeamCompeting;
            // TODO: Limit the number of prizes to first and second
            // because third place is not easy to determine, probably use scores
            decimal winnerPrize = 0;
            decimal runnerUpPrize = 0;

            if (model.Prizes.Count > 0)
            {
                decimal totalIncome = model.EnteredTeams.Count * model.EntryFee;
                PrizeModel firstPlacePrize = model.Prizes.Where(x => x.PlaceNumber == 1).FirstOrDefault();
                PrizeModel secondPlacePrize = model.Prizes.Where(x => x.PlaceNumber == 2).FirstOrDefault();

                if (firstPlacePrize != null)
                {
                    winnerPrize = firstPlacePrize.CalculatePrizePayout(totalIncome);
                }

                if (secondPlacePrize != null)
                {
                    runnerUpPrize = secondPlacePrize.CalculatePrizePayout(totalIncome);
                }
            }

            // Send email at completion of tournament with prize information
            string subject = "";
            StringBuilder body = new StringBuilder();

            subject = $"In { model.TournamentName }, { winners.TeamName } has won!";
            body.AppendLine("<h1>We have a winner!</h1>");
            body.AppendLine("<p>Congratulations to our winner on a great tournament.</p>");
            body.AppendLine("<br />");
            
            if (winnerPrize > 0)
            {
                body.AppendLine($"<p>{ winners.TeamName } will receive \u20AC{ winnerPrize }</p>"); // The Euro sign hex code is u20AC
            }

            if (runnerUpPrize > 0)
            {
                body.AppendLine($"<p>{ runnerUp.TeamName } will receive \u20AC{ runnerUpPrize }</p>");
            }

            body.AppendLine("<p>Thanks for a great tournament everyone!</p>");
            body.AppendLine("<h2>~Tournament Tracker<h2/>");

            List<string> bccAddress = new List<string>();

            foreach (TeamModel t in model.EnteredTeams)
            {
                foreach (PersonModel p in t.TeamMembers)
                {
                    if (p.EmailAddress.Length > 0)
                    {
                        bccAddress.Add(p.EmailAddress);
                    }
                }
            }

            EmailLogic.SendEmail(new List<string>(), bccAddress, subject, body.ToString());

            model.CompleteTournament();
        }

        private static decimal CalculatePrizePayout(this PrizeModel prize, decimal totalIncome)
        {

            decimal output = 0;

            if (prize.PrizeAmount > 0)
            {
                output = prize.PrizeAmount;
            }
            else
            {
                output = decimal.Multiply(totalIncome, Convert.ToDecimal(prize.PrizePercentage / 100));
            }

            return output;
        }

        private static void AdvanceWinners(List<MatchupModel> models, TournamentModel tournament)
        {
            // Update the winner in the matchup and matchupentries
            // The ID's are not known yet, therefore when more than 1 bye is present
            // the first bye team is the winner for all byes
            foreach (MatchupModel m in models)
            {                
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
                        currentRound.Add(currentMatchup); // add the matchup entry to the round
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
            /*
             * 3 Teams = 4 minus 3(1 bye)
             * 5 Teams = 8 minus 5(3 byes)
             * 5 Teams = 8 minus 5(3 byes)
             * 9 Teams = 16 minus 9(7 byes)
             * 20 Teams = 32 minus 20(12 byes)
             */
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
