using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DbAgnostic
{
    public abstract class SqlGenerator<T>
    {
        protected const char TAB = '\t';

        private string _TableName;
        private List<PropertyConfiguration> _PropertyConfiguration { get; set; }

        protected string TableName
        {
            get
            {
                return _TableName;
            }
        }

        #region CTOR

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
            //_PropertyNameList = new List<string>();
            _PropertyConfiguration = new List<PropertyConfiguration>();

            foreach (PropertyInfo pi in props)
            {
                CrudAccessKeyAttribute keyAtr = pi.GetCustomAttribute<CrudAccessKeyAttribute>();
                CrudAccessPropertySettingsAttribute settingAtr = pi.GetCustomAttribute<CrudAccessPropertySettingsAttribute>();

                PropertyConfiguration cfg = new PropertyConfiguration();
                cfg.Name = pi.Name;

                if (keyAtr != null)
                {
                    if (_PropertyConfiguration.Where(x => x.isKey).SingleOrDefault() != null)
                        throw new DbAccessException($"Multiple keys specified on crud model. ");//TODO: create support for multiple keys?

                    cfg.isKey = true;
                    cfg.Insert = keyAtr.Insert;
                    cfg.Update = keyAtr.Update;
                }
                if (settingAtr != null)
                {
                    //settings attribute overrides key attribute (wouldn't usually expect these on the same property!)
                    cfg.Insert = settingAtr.Insert;
                    cfg.Update = settingAtr.Update;
                    cfg.Select = settingAtr.Select;
                    cfg.DoBy = settingAtr.DoBy;
                }
                _PropertyConfiguration.Add(cfg);
            }
        }

        #endregion

        #region Statements

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
                    if (_PropertyConfiguration.Where(x => x.isKey).SingleOrDefault() == null)
                        throw new DbAccessException($"There is no CrudAccessKeyAttribute for '{typeof(T).Name}'.");

                    //if there is no key col name then we cannot use SelectByIdSatement
                    //if (string.IsNullOrWhiteSpace(_KeyColumnName))
                    //    throw new DbAccessException($"There is no CrudAccessInclude attribute with the property IsID set to true for '{typeof(T).Name}'");

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
                //if (!_PropertyNameList.Select(x => x.ToLower()).Contains(propertyName.ToLower()))
                //    throw new DbAccessException($"Property '{propertyName}' is not a member of the '{typeof(T).Name}' property list.");
                CanDoByTest(propertyName);

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
                //if (!_PropertyNameList.Select(x => x.ToLower()).Contains(propertyName.ToLower()))
                //    throw new DbAccessException($"Property '{propertyName}' is not a member of the '{typeof(T).Name}' property list.");
                CanDoByTest(propertyName);

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
                //if (!_PropertyNameList.Select(x => x.ToLower()).Contains(propertyName.ToLower()))
                //    throw new DbAccessException($"Property '{propertyName}' is not a member of the '{typeof(T).Name}' property list.");
                CanDoByTest(propertyName);

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
                //if (!_PropertyNameList.Select(x => x.ToLower()).Contains(propertyName.ToLower()))
                //    throw new DbAccessException($"Property '{propertyName}' is not a member of the '{typeof(T).Name}' property list.");
                CanDoByTest(propertyName);

                _DeleteBy[propertyName] = GenerateDeleteByStatement(propertyName);
            }
            return _DeleteBy[propertyName];
        }

        #endregion

        #region Key Name and Select/Insert/Update Property List

        //TODO: cache this lot
        protected string KeyColumnName
        {
            get
            {
                string output = _PropertyConfiguration
                    .Where(x => x.Select)
                    .Select(x => x.Name)
                    .SingleOrDefault();
                if (string.IsNullOrWhiteSpace(output))
                    throw new DbAccessException($"There is no CrudAccessKeyAttribute for '{typeof(T).Name}'.");
                else return output;
            }
        }
        protected string[] SelectList
        {
            get
            {
                IEnumerable<string> output = _PropertyConfiguration
                .Where(x => x.Select)
                .Select(x => x.Name);

                if (output == null || output.Count() <= 0)
                    throw new DbAccessException($"No properties found for select in '{typeof(T).Name}'.");
                else return output.ToArray();
            }
        }
        protected string[] InsertList
        {
            get
            {
                IEnumerable<string> output = _PropertyConfiguration
                    .Where(x => x.Insert)
                    .Select(x => x.Name);

                if (output == null || output.Count() <= 0)
                    throw new DbAccessException($"No properties found for insert in '{typeof(T).Name}'.");
                else return output.ToArray();
            }
        }
        protected string[] UpdateList
        {
            get
            {
                IEnumerable<string> output = _PropertyConfiguration
                    .Where(x => x.Update)
                    .Select(x => x.Name);

                if (output == null || output.Count() <= 0)
                    throw new DbAccessException($"No properties found for update in '{typeof(T).Name}'.");
                else return output.ToArray();
            }
        }

        #endregion

        private void CanDoByTest(string propertyName)
        {
            PropertyConfiguration cfg = _PropertyConfiguration.Where(x => x.Name == propertyName).SingleOrDefault();
            if (cfg != null)
            {
                if (!cfg.DoBy)
                    throw new DbAccessException($"Property '{propertyName}' cannot be used for 'select/update/delete by' due to CrudAccessPropertySettingsAttribute.");
            }
            throw new DbAccessException($"Property '{propertyName}' is not a member of the '{typeof(T).Name}' property list.");
        }

        //TODO: comment
        protected class PropertyConfiguration
        {
            public string Name { get; set; }
            public bool isKey { get; set; }
            public bool Insert { get; set; }
            public bool Update { get; set; }
            public bool Select { get; set; }
            public bool DoBy { get; set; }

            public PropertyConfiguration()
            {
                //defaults
                isKey = false;
                Insert = true;
                Update = true;
                Select = true;
                DoBy = true;
            }
        }

    }
}
