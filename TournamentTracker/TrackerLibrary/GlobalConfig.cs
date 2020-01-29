using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents the global variables
    /// Unable to be instantiated, is always available to
    /// everyone
    /// </summary>
    public static class GlobalConfig
    {
        // using 'const' for the files to always use the same file 
        // and not allow overwriting the variable
        public const string PrizesFile = "PrizeModels.csv";
        public const string PeopleFile = "PersonModels.csv";
        public const string TeamsFile = "TeamModels.csv";
        public const string TournamentsFile = "TournamentModels.csv";
        public const string MatchupsFile = "MatchupModels.csv";
        public const string MatchupEntriesFile = "MatchupEntryModels.csv";

        public static IDataConnection Connection { get; private set; }

        public static void InitializeConnections(DatabaseType db)
        {
            // This could also be implemented as a switch
            if (db == DatabaseType.Sql)
            {
                // This is the sql connection implemented using dapper
                SqlConnector sql = new SqlConnector();
                Connection = sql;
            }
            else if (db == DatabaseType.TextFile)
            {
                // This is the text file connection
                TextConnector text = new TextConnector();
                Connection = text;
            }
        }

        public static string CnnString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
