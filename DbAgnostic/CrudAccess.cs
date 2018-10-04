using System;
using System.Collections.Generic;
using System.Data;

namespace DbAgnostic
{

    #region Interfece

    /// <summary>
    /// Provides data access with all of the basic crud methods automatically provided
    /// </summary>
    public interface ICrudAccess<T> : IDbAccess<T>
    {
        /// <summary>
        /// Selects all records in database table.
        /// </summary>
        IEnumerable<T> Get();

        /// <summary>
        /// Selects all records in database table.  Used for running statements within a transaction. 
        /// </summary>
        IEnumerable<T> Get(IDbConnection con, IDbTransaction tran);

        /// <summary>
        /// Selects a single record by its id, returns null if nothing is found. 
        /// </summary>
        T Get(object id);

        /// <summary>
        /// Selects a single record by its id, returns null if nothing is found.  Used for running statements within a transaction. 
        /// </summary>
        T Get(IDbConnection con, IDbTransaction tran, object id);

        /// <summary>
        /// Inserts a new item into the database.
        /// </summary>
        T Create(T obj);

        /// <summary>
        /// Inserts a new item into the database. Used for running statements within a transaction.
        /// </summary>
        T Create(IDbConnection con, IDbTransaction tran, T obj);

        /// <summary>
        /// Updates an existing item in the database using value in the id field.
        /// </summary>
        T Update(T obj);

        /// <summary>
        /// Updates an existing item in the database using value in the id field. Used for running statements within a transaction. 
        /// </summary>
        T Update(IDbConnection con, IDbTransaction tran, T obj);

        /// <summary>
        /// Updates an existing item in the database where the given property has a certain value.
        /// </summary>
        T UpdateBy(string propertyName, T obj);

        /// <summary>
        /// Updates an existing item in the database where the given property has a certain value.
        /// </summary>
        T UpdateBy(IDbConnection con, IDbTransaction tran, string propertyName, T obj);

        /// <summary>
        /// Deletes an item from the database using a given id.
        /// </summary>
        void Delete(object id);

        /// <summary>
        /// Deletes an item from the database using a given id. Used for running statements within a transaction. 
        /// </summary>
        void Delete(IDbConnection con, IDbTransaction tran, object id);

        /// <summary>
        /// Deletes an item from the database here the given property has a certain value.
        /// </summary>
        void DeleteBy(string propertyName, object value);

        /// <summary>
        /// Deletes an item from the database here the given property has a certain value.
        /// </summary>
        void DeleteBy(IDbConnection con, IDbTransaction tran, string propertyName, object value);

        /// <summary>
        /// Selects items form the database where the given property has a certain value
        /// </summary>
        IEnumerable<T> GetBy(string propertyName, object value);

        /// <summary>
        /// Selects items form the database where the given property has a certain value. Used for running statements within a transaction. 
        /// </summary>
        IEnumerable<T> GetBy(IDbConnection con, IDbTransaction tran, string propertyName, object value);

        /// <summary>
        /// Gets a single item where the given property has a value, if nothing is found null is returned
        /// </summary>
        T GetSingleBy(string propertyName, object value);

        /// <summary>
        /// Gets a single item where the given property has a value, if nothing is found null is returned. Used for running statements within a transaction. 
        /// </summary>
        T GetSingleBy(IDbConnection con, IDbTransaction tran, string propertyName, object value);
    }

    #endregion

    public class CrudAccess<T> : DbAccess<T>, ICrudAccess<T>
    {
        private readonly SqlGenerator<T> _SqlGenerator;

        public CrudAccess(Func<IDbConnection> fnGetConnection, SqlGenerator<T> sqlGenerrator)
            : base(fnGetConnection)
        {
            _SqlGenerator = sqlGenerrator;
        }

        public IEnumerable<T> Get() =>
            Query<T>(_SqlGenerator.SelectSatement);

        public IEnumerable<T> Get(IDbConnection con, IDbTransaction tran) =>
            Query<T>(con, tran, _SqlGenerator.SelectSatement);

        public T Get(object id) =>
           QueryFirstOrDefault<T>(_SqlGenerator.SelectByIdSatement, new { ID = id });

        public T Get(IDbConnection con, IDbTransaction tran, object id) =>
            QueryFirstOrDefault<T>(con, tran, _SqlGenerator.SelectByIdSatement, new { ID = id });

        public T Create(T obj) =>
            QueryFirstOrDefault<T>(_SqlGenerator.InsertSatement, obj);

        public T Create(IDbConnection con, IDbTransaction tran, T obj) =>
            QueryFirstOrDefault<T>(con, tran, _SqlGenerator.InsertSatement, obj);

        public T Update(T obj) =>
            QueryFirstOrDefault<T>(_SqlGenerator.UpdateSatement, obj);

        public T Update(IDbConnection con, IDbTransaction tran, T obj) =>
            QueryFirstOrDefault<T>(con, tran, _SqlGenerator.UpdateSatement, obj);

        public T UpdateBy(string propertyName, T obj) =>
            QueryFirstOrDefault<T>(_SqlGenerator.UpdateByStatement(propertyName), obj);

        public T UpdateBy(IDbConnection con, IDbTransaction tran, string propertyName, T obj) =>
            QueryFirstOrDefault<T>(con, tran, _SqlGenerator.UpdateByStatement(propertyName), obj);

        public void Delete(object id) =>
            Execute(_SqlGenerator.DeleteSatement, new { ID = id });

        public void Delete(IDbConnection con, IDbTransaction tran, object id) =>
            Execute(con, tran, _SqlGenerator.DeleteSatement, new { ID = id });

        public void DeleteBy(string propertyName, object value) =>
            Execute(_SqlGenerator.DeleteByStatement(propertyName), new { ID = value });

        public void DeleteBy(IDbConnection con, IDbTransaction tran, string propertyName, object value) =>
            Execute(con, tran, _SqlGenerator.DeleteByStatement(propertyName), new { ID = value });

        public IEnumerable<T> GetBy(string propertyName, object value) =>
            Query<T>(_SqlGenerator.SelectByStatement(propertyName), new { Value = value });

        public IEnumerable<T> GetBy(IDbConnection con, IDbTransaction tran, string propertyName, object value) =>
            Query<T>(con, tran, _SqlGenerator.SelectByStatement(propertyName), new { Value = value });

        public T GetSingleBy(string propertyName, object value) =>
            QueryFirstOrDefault<T>(_SqlGenerator.SelectSingleByStatement(propertyName), new { Value = value });

        public T GetSingleBy(IDbConnection con, IDbTransaction tran, string propertyName, object value) =>
            QueryFirstOrDefault<T>(con, tran, _SqlGenerator.SelectSingleByStatement(propertyName), new { Value = value });
    }
}
