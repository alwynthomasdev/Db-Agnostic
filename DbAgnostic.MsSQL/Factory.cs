using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DbAgnostic.MsSql
{
    /// <summary>
    /// All instances of data access should be created using the factory
    /// </summary>
    public static class DbAccessFactory
    {
        public static IDbAccess Build(string connectionString) =>
            new DbAccess(() => new SqlConnection(connectionString));

        public static IDbAccess<T> Build<T>(string connectionString) =>
                new DbAccess<T>(() => new SqlConnection(connectionString));

        public static ICrudAccess<T> BuildCrud<T>(string connectionString) =>
                new CrudAccess<T>(() => new SqlConnection(connectionString), new MsSqlGenerator<T>());
    }
}
