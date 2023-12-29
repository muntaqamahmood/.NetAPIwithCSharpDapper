using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotNetAPI
{
    class DataContextDapper
    {
        // inject configuration
        private readonly IConfiguration _configuration;

        public DataContextDapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql);
        }

        public T LoadDataSingle<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql); // returns of type T (T can be string, object, etc)
        }

        public int ExecuteSql(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql);
        }

        public bool ExecuteSqlWithRowCount(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql) > 0;
        }
    }
}