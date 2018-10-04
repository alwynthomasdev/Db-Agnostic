using MySql.Data.MySqlClient;

namespace DbAgnostic
{
    /// <summary>
    /// All instances of data access should be created using the factory
    /// </summary>
    public static partial class DbAccessFactory
    {
        public static IDbAccess Build(string connectionString) =>
            new DbAccess(() => new MySqlConnection(connectionString));

        public static IDbAccess<T> Build<T>(string connectionString) =>
                new DbAccess<T>(() => new MySqlConnection(connectionString));

        public static ICrudAccess<T> BuildCrud<T>(string connectionString) =>
                new CrudAccess<T>(() => new MySqlConnection(connectionString), new MySqlGenerator<T>());
    }
}
