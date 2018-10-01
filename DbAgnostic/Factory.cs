using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DbAgnostic
{
    /// <summary>
    /// All instances of data access should be created using the factory
    /// </summary>
    public static class DbAccessFactory
    {
        public static IDbAccess Build(Func<IDbConnection> fn) =>
            new DbAccess(fn);

        public static IDbAccess<T> Build<T>(Func<IDbConnection> fn) =>
                new DbAccess<T>(fn);
    }
}
