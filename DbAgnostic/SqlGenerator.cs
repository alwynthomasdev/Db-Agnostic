using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DbAgnostic
{
    public abstract class SqlGenerator<T>
    {
        protected const char TAB = '\t';
        protected string _TableName;
        protected List<string> _PropertyNameList;
        protected string _KeyColumnName;
        protected bool _IdentityInsert;

        //CTOR
        public SqlGenerator()
        {
            Type t = typeof(T);

            CrudAccessTableNameAttribute atrTableName = t.GetCustomAttribute<CrudAccessTableNameAttribute>(false);      //get the table name attribute for the DTO
            if (atrTableName == null || string.IsNullOrWhiteSpace(atrTableName.Name))                                   //if there is no table name for DTO
                throw new DbAccessException($"There is no CrudAccessTableName attribute set for '{typeof(T).Name}'");   //throw an exception

            //else get the table name
            else
                _TableName = atrTableName.Name;

            //get the property names for this DTO
            PropertyInfo[] props = t.GetProperties();
            _PropertyNameList = new List<string>();

            foreach (PropertyInfo pi in props)
            {
                CrudAccessIncludeAttribute atr = pi.GetCustomAttribute<CrudAccessIncludeAttribute>();
                if (atr != null)
                {
                    _PropertyNameList.Add(pi.Name);
                    if (atr.IsID)
                    {
                        _IdentityInsert = atr.InsertID;
                        _KeyColumnName = pi.Name;
                    }
                }
            }
        }

        protected abstract string GenerateSelectStatement();
        private string _Select;
        public string SelectSatement
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Select))
                {
                    _Select = GenerateSelectStatement();
                }
                return _Select;
            }
        }

        protected abstract string GenerateSelectByIdSatement();
        private string _SelectByID;
        public string SelectByIdSatement
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_SelectByID))
                {
                    //if there is no key col name then we cannot use SelectByIdSatement
                    if (string.IsNullOrWhiteSpace(_KeyColumnName))
                        throw new DbAccessException($"There is no CrudAccessInclude attribute with the property IsID set to true for '{typeof(T).Name}'");

                    _SelectByID = GenerateSelectByIdSatement();
                }
                return _SelectByID;
            }
        }

        protected abstract string GenerateInsertSatement();
        private string _Insert;
        public string InsertSatement
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Insert))
                {
                    _Insert = GenerateInsertSatement();
                }
                return _Insert;
            }
        }

        protected abstract string GenerateUpdateSatement();
        private string _Update;
        public string UpdateSatement
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Update))
                {
                    _Update = GenerateUpdateSatement();
                }
                return _Update;
            }
        }

        protected abstract string GenerateDeleteSatement();
        private string _Delete;
        public string DeleteSatement
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Delete))
                {
                    _Delete = GenerateDeleteSatement();
                }
                return _Delete;
            }
        }

        protected abstract string GenerateSelectByStatement(string propertyName);
        private Dictionary<string, string> _SelectBy;
        public string SelectByStatement(string propertyName)
        {
            if (_SelectBy == null) _SelectBy = new Dictionary<string, string>();
            if (!_SelectBy.ContainsKey(propertyName))
            {
                //make sure this property is a valid property to search by
                if (!_PropertyNameList.Select(x => x.ToLower()).Contains(propertyName.ToLower()))
                    throw new DbAccessException($"Property '{propertyName}' is not a member of the '{typeof(T).Name}' property list.");

                _SelectBy[propertyName] = GenerateSelectByStatement(propertyName);
            }
            return _SelectBy[propertyName];
        }

        protected abstract string GenerateSelectSingleByStatement(string propertyName);
        private Dictionary<string, string> _SelectSingleBy;
        public string SelectSingleByStatement(string propertyName)
        {
            if (_SelectSingleBy == null) _SelectSingleBy = new Dictionary<string, string>();
            if (!_SelectSingleBy.ContainsKey(propertyName))
            {
                //make sure this property is a valid property to search by
                if (!_PropertyNameList.Select(x => x.ToLower()).Contains(propertyName.ToLower()))
                    throw new DbAccessException($"Property '{propertyName}' is not a member of the '{typeof(T).Name}' property list.");

                _SelectSingleBy[propertyName] = GenerateSelectSingleByStatement(propertyName);
            }
            return _SelectSingleBy[propertyName];
        }

        protected abstract string GenerateUpdateByStatement(string propertyName);
        private Dictionary<string, string> _UpdateBy;
        public string UpdateByStatement(string propertyName)
        {
            if (_UpdateBy == null) _UpdateBy = new Dictionary<string, string>();
            if (!_UpdateBy.ContainsKey(propertyName))
            {
                //make sure this property is a valid property to search by
                if (!_PropertyNameList.Select(x => x.ToLower()).Contains(propertyName.ToLower()))
                    throw new DbAccessException($"Property '{propertyName}' is not a member of the '{typeof(T).Name}' property list.");

                _UpdateBy[propertyName] = GenerateUpdateByStatement(propertyName);
            }
            return _UpdateBy[propertyName];
        }

        protected abstract string GenerateDeleteByStatement(string propertyName);
        private Dictionary<string, string> _DeleteBy;
        public string DeleteByStatement(string propertyName)
        {
            if (_DeleteBy == null) _UpdateBy = new Dictionary<string, string>();
            if (!_DeleteBy.ContainsKey(propertyName))
            {
                //make sure this property is a valid property to search by
                if (!_PropertyNameList.Select(x => x.ToLower()).Contains(propertyName.ToLower()))
                    throw new DbAccessException($"Property '{propertyName}' is not a member of the '{typeof(T).Name}' property list.");

                _DeleteBy[propertyName] = GenerateDeleteByStatement(propertyName);
            }
            return _DeleteBy[propertyName];
        }

    }
}
