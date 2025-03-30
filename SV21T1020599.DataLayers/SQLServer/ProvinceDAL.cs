using Dapper;
using SV21T1020599.DomainModels;

namespace SV21T1020599.DataLayers.SQLServer
{
    public class ProvinceDAL : BaseDAL, ISimpleQueryDAL<Province>
    {
        public ProvinceDAL(string connectionString) : base(connectionString)
        {
        }

        public List<Province> List()
        {
            List<Province> data = new List<Province>();
            using (var connection = Openconnection())
            {
                var sql = @"select * from Provinces";
                data = connection.Query<Province>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
            }


            return data;
        }

    }
}