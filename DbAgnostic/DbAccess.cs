using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DbAgnostic
{

    #region Interfaces

    /// <summary>
    /// Most simple data access version, makes no assumptions about the data being returned
    /// </summary>
    public interface IDbAccess
    {
        /// <summary>
        /// Simply returns a connection object to the database.
        /// </summary>
        IDbConnection GetConnection();

        /// <summary>
        /// Queries the database using provided SQL statement and parameter object 
        /// returning a typed list of results.
        /// </summary>
        IEnumerable<T> Query<T>(string sql, object @params = null);

        /// <summary>
        /// Queries the database using provided SQL statement and parameter object 
        /// returning a typed list of results. Used for running statements within a transaction.
        /// </summary>
        IEnumerable<T> Query<T>(IDbConnection con, IDbTransaction tran, string sql, object @params = null);

        /// <summary>
        /// Queries the database using provided SQL statement and parameter object 
        /// returning the first item or null if nothing is found.
        /// </summary>
        T QueryFirstOrDefault<T>(string sql, object @params = null);

        /// <summary>
        /// Queries the database using provided SQL statement and parameter object 
        /// returning the first item or null if nothing is found. Used for running statements within a transaction.
        /// </summary>
        T QueryFirstOrDefault<T>(IDbConnection con, IDbTransaction tran, string sql, object @params = null);

        /// <summary>
        /// Executes an SQL statement returning the number of rows affected.
        /// </summary>
        int Execute(string sql, object @params = null);

        /// <summary>
        /// Executes an SQL statement returning the number of rows affected. Used for running statements within a transaction.
        /// </summary>
        int Execute(IDbConnection con, IDbTransaction tran, string sql, object @params = null);

        /// <summary>
        /// Runs an SQL query that returns multiple sets of data.
        /// </summary>
        SqlMapper.GridReader QueryMultiple(IDbConnection con, string sql, object @params = null);

        /// <summary>
        /// Runs an SQL query that returns multiple sets of data. Used for running statements within a transaction.
        /// </summary>
        SqlMapper.GridReader QueryMultiple(IDbConnection con, IDbTransaction tran, string sql, object @params = null);
    }

    /// <summary>
    /// A basic data access for working with specified type.
    /// </summary>
    public interface IDbAccess<T>
    {
        /// <summary>
        /// Simply returns a connection object to the database.
        /// </summary>
        IDbConnection GetConnection();

        /// <summary>
        /// Queries the database using provided SQL statement and parameter object 
        /// returning a typed list of results.
        /// </summary>
        IEnumerable<T> Query(string sql, object @params = null);

        /// <summary>
        /// Queries the database using provided SQL statement and parameter object 
        /// returning a typed list of results. Used for running statements within a transaction.
        /// </summary>
        IEnumerable<T> Query(IDbConnection con, IDbTransaction tran, string sql, object @params = null);

        /// <summary>
        /// Queries the database using provided SQL statement and parameter object 
        /// returning the first item or null if nothing is found
        /// </summary>
        T QueryFirstOrDefault(string sql, object @params = null);

        /// <summary>
        /// Queries the database using provided SQL statement and parameter object 
        /// returning the first item or null if nothing is found. Used for running statements within a transaction.
        /// </summary>
        T QueryFirstOrDefault(IDbConnection con, IDbTransaction tran, string sql, object @params = null);

        /// <summary>
        /// Executes an SQL statement returning the number of rows affected
        /// </summary>
        int Execute(string sql, object @params = null);

        /// <summary>
        /// Executes an SQL statement returning the number of rows affected. Used for running statements within a transaction.
        /// </summary>
        int Execute(IDbConnection con, IDbTransaction tran, string sql, object @params = null);

        /// <summary>
        /// Runs an SQL query that returns multiple sets of data.
        /// </summary>
        SqlMapper.GridReader QueryMultiple(IDbConnection con, string sql, object @params = null);

        /// <summary>
        /// Runs an SQL query that returns multiple sets of data. Used for running statements within a transaction.
        /// </summary>
        SqlMapper.GridReader QueryMultiple(IDbConnection con, IDbTransaction tran, string sql, object @params = null);
    }

    #endregion

    #region Implementations

    public class DbAccess : IDbAccess
    {
        private readonly Func<IDbConnection> _FnGetConnection;
        public DbAccess(Func<IDbConnection> fnGetConnection)
        {
            _FnGetConnection = fnGetConnection;
        }

        public IDbConnection GetConnection() => _FnGetConnection();

        public IEnumerable<T> Query<T>(string sql, object @params = null)
        {
            try
            {
                using (var con = GetConnection())
                {
                    return con.Query<T>(sql, @params);
                }
            }
            catch (Exception ex)
            {
                throw new DbAccessException("Exception found when trying to execute SQL.", ex, sql, @params);
            }
        }

        public T QueryFirstOrDefault<T>(string sql, object @params = null)
        {
            try
            {
                using (var con = GetConnection())
                {
                    return con.QueryFirstOrDefault<T>(sql, @params);
                }
            }
            catch (Exception ex)
            {
                throw new DbAccessException("Exception found when trying to execute SQL.", ex, sql, @params);
            }
        }

        public int Execute(string sql, object @params = null)
        {
            try
            {
                using (var con = GetConnection())
                {
                    return con.Execute(sql, @params);
                }
            }
            catch (Exception ex)
            {
                throw new DbAccessException("Exception found when trying to execute SQL.", ex, sql, @params);
            }
        }

        public SqlMapper.GridReader QueryMultiple(IDbConnection con, string sql, object @params = null)
        {
            try
            {
                return con.QueryMultiple(sql, @params);
            }
            catch (Exception ex)
            {
                throw new DbAccessException("Exception found when trying to execute SQL.", ex, sql, @params);
            }
        }

        public IEnumerable<T> Query<T>(IDbConnection con, IDbTransaction tran, string sql, object @params = null)
        {
            try
            {
                return con.Query<T>(sql, @params, transaction: tran);
            }
            catch (Exception ex)
            {
                throw new DbAccessException("Exception found when trying to execute SQL.", ex, sql, @params);
            }
        }

        public T QueryFirstOrDefault<T>(IDbConnection con, IDbTransaction tran, string sql, object @params = null)
        {
            try
            {
                return con.QueryFirstOrDefault<T>(sql, @params, transaction: tran);
            }
            catch (Exception ex)
            {
                throw new DbAccessException("Exception found when trying to execute SQL.", ex, sql, @params);
            }
        }

        public int Execute(IDbConnection con, IDbTransaction tran, string sql, object @params = null)
        {
            try
            {
                return con.Execute(sql, @params, transaction: tran);
            }
            catch (Exception ex)
            {
                throw new DbAccessException("Exception found when trying to execute SQL.", ex, sql, @params);
            }
        }

        public SqlMapper.GridReader QueryMultiple(IDbConnection con, IDbTransaction tran, string sql, object @params = null)
        {
            try
            {
                return con.QueryMultiple(sql, @params, transaction: tran);
            }
            catch (Exception ex)
            {
                throw new DbAccessException("Exception found when trying to execute SQL.", ex, sql, @params);
            }
        }
    }

    public class DbAccess<T> : DbAccess, IDbAccess<T>
    {
        public DbAccess(Func<IDbConnection> fnGetConnection) : base(fnGetConnection) { }

        IEnumerable<T> IDbAccess<T>.Query(string sql, object @params) =>
            base.Query<T>(sql, @params);

        IEnumerable<T> IDbAccess<T>.Query(IDbConnection con, IDbTransaction tran, string sql, object @params) =>
            base.Query<T>(con, tran, sql, @params);

        T IDbAccess<T>.QueryFirstOrDefault(string sql, object @params) =>
            base.QueryFirstOrDefault<T>(sql, @params);

        T IDbAccess<T>.QueryFirstOrDefault(IDbConnection con, IDbTransaction tran, string sql, object @params) =>
            base.QueryFirstOrDefault<T>(con, tran, sql, @params);
    }

    #endregion

}
