using System;

namespace DbAgnostic
{
    //TODO: document these

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CrudAccessPropertySettingsAttribute : Attribute
    {
        public CrudAccessPropertySettingsAttribute()
        {
            //default exclusion settings, never update or insert, but always select and be able to select/update by
            Insert = false;
            Update = false;
            Select = true;
            DoBy = true;
        }

        public bool Insert { get; set; }
        public bool Update { get; set; }
        public bool Select { get; set; }
        public bool DoBy { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CrudAccessKeyAttribute : Attribute
    {
        public CrudAccessKeyAttribute()
        {
            Insert = true;
            Update = false;
        }
        public bool Insert { get; set; }
        public bool Update { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CrudAccessTableNameAttribute : Attribute
    {
        public CrudAccessTableNameAttribute(string name)
        {
            _Name = name;
        }

        private string _Name;
        public string Name { get { return _Name; } }
    }
}
