using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DbAgnostic.Tests
{
    [TestFixture]
    public class DbAccessTest
    {

        IDbAccess _dbAccess;

        public DbAccessTest()
        {
            string con = ConfigurationHelper.GetConnectionString();
            _dbAccess = DbAccessFactory.Build(() => new SqlConnection(con));
        }


        [Test]
        public void TestInsert()
        {
            _dbAccess.Execute("INSERT INTO [User] ([FirstName], [LastName], [EmailAddress]) VALUES (@FirstName, @LastName, @EmailAddress)",
                new { FirstName = "Test", LastName = "User", EmailAddress = "testuser@dbagnostic.test" });
        }

        [Test]
        public void TestSelect()
        {
            IEnumerable<dynamic> data = _dbAccess.Query<dynamic>("SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = "testuser@dbagnostic.test" });

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count() > 0);
            Assert.IsTrue(data.First().FirstName == "Test");
        }
    }
}
