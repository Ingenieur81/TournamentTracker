using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

//@PlaceNumber int,
//@PlaceName nvarchar(50),
//@PrizeAmount money,
//@PrizePercentage float,
//@id int = 0 output

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        private const string dbName = "Tournaments";

        /// <summary>
        /// Saves a prize to the database
        /// </summary>
        /// <param name="model">The prize information model</param>
        /// <returns>The prize information, including the unique identifier</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            /* 
             * Using 'using' ensures that the closing curly bracket means that the 
             * connections are properly closed
             * This makes sure there is no memery leakage and thus no program crashes
             * due to connections not being closed
             * */
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(dbName)))
            {
                //Set the values to put into the database
                var p = new DynamicParameters();
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output); //Int32 is the standard int, standard direction is Input, therefore not specified

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                //Get the value of Id created by the database
                model.Id = p.Get<int>("@Id");

                return model;
            }
        }

        public PersonModel CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(dbName)))
            {
                //Set the values to put into the database
                var p = new DynamicParameters();
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@CellphoneNumber", model.CellphoneNumber);
                p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output); //Int32 is the standard int, standard direction is Input, therefore not specified

                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

                //Get the value of Id created by the database
                model.Id = p.Get<int>("@Id");

                return model;
            }
        }

        /// <summary>
        /// Get all people from the database
        /// </summary>
        /// <returns>Return all people as a list</returns>
        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(dbName)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }

        public TeamModel CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(dbName)))
            {
                //Set the values to put into the database
                var p = new DynamicParameters();
                p.Add("@TeamName", model.TeamName);
                p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output); //Int32 is the standard int, standard direction is Input, therefore not specified

                connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);

                //Get the value of Id created by the database
                model.Id = p.Get<int>("@Id");

                foreach (PersonModel tm in model.TeamMembers)
                {
                    //Set the values to put into the database
                    // p can be used again because it is a dynamic parameter with variable definition
                    p = new DynamicParameters();
                    p.Add("@TeamId", model.Id);
                    p.Add("@personId", tm.Id);

                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }

                return model;
            }
        }
    }
}