using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;

namespace TrackerUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Initialize the data source connections
            // Choose Sql or TextFile
            GlobalConfig.InitializeConnections(DatabaseType.TextFile);            
            Application.Run(new CreateTournamentForm());
            //Application.Run(new TournamentDashboardForm());
        }
    }
}
