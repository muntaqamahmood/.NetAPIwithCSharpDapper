using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotNetAPI.Data
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

        public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
        {
            // setting up the sql command
            SqlCommand commandWithParams = new SqlCommand(sql);
            // add params to the sql command
            foreach (SqlParameter param in parameters)
            {
                commandWithParams.Parameters.Add(param);
            }
            SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            sqlConnection.Open();
            commandWithParams.Connection = sqlConnection;
            int rowsAffected = commandWithParams.ExecuteNonQuery();
            sqlConnection.Close();
            return rowsAffected > 0;
        }
    }
}