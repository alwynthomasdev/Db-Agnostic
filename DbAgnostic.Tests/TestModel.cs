using System;
using System.Collections.Generic;
using System.Text;

namespace DbAgnostic.Tests
{
    [CrudAccessTableName("User")]
    public class TestModel
    {
        [CrudAccessKey(Insert=false)]
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        [CrudAccessPropertyConfiguration]
        public DateTime DateCreated { get; set; }
        public string Bio { get; set; }
    }
}
