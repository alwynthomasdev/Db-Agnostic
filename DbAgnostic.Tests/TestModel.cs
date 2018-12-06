using System;
using System.Collections.Generic;
using System.Text;

namespace DbAgnostic.Tests
{
    [CrudAccessTableName("User")]
    public class TestModel
    {
        [CrudAccessKey(Insert=false, Update = false)]
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        [CrudAccessPropertySettings(Insert = false, Update = false)]
        public DateTime DateCreated { get; set; }
        public string Bio { get; set; }

        [CrudAccessPropertySettings(Insert = false, Update = false, Select = false, DoBy = false)]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

    }
}
