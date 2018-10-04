using System;

namespace DbAgnostic
{
    //TODO: document these

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CrudAccessIncludeAttribute : Attribute
    {
        public CrudAccessIncludeAttribute()
        {
            IsID = false;
            InsertID = false;
        }

        public bool IsID { get; set; }
        public bool InsertID { get; set; }

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
