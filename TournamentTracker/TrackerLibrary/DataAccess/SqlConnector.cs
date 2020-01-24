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
        // TODO: Make the CreatePrize method actually save to the database.
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
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
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
    }
}