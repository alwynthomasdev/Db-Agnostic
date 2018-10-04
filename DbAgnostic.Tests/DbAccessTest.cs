using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DbAgnostic.Tests
{
    [TestFixture]
    public class DbAccessTest
    {
        //TODO: test all transactional stuff

        #region Setup

        string _connectionString;
        IDbAccess _dbAccess;

        public DbAccessTest()
        {
            _connectionString = ConfigurationHelper.GetConnectionString();
            _dbAccess = DbAccessFactory.Build(() => new SqlConnection(_connectionString));
        }

        #endregion

        [Test]
        public void GetConnectionTest()
        {
            IDbConnection con = _dbAccess.GetConnection();

            Assert.IsNotNull(con);
            Assert.AreEqual(_connectionString, con.ConnectionString);
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

        [Test]
        public void TestQueryFirstOrDefault()
        {
            dynamic data = _dbAccess.QueryFirstOrDefault<dynamic>("SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = "testuser@dbagnostic.test" });

            Assert.IsNotNull(data);
            Assert.IsTrue(data.FirstName == "Test");
        }

        [Test]
        public void TestQueryMultiple()
        {
            IDbConnection con = _dbAccess.GetConnection();
            QueryMultipleResultReader result = _dbAccess.QueryMultiple(con, @"
SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress;
SELECT COUNT(*) AS [cnt] FROM [User];
", 
                new { EmailAddress = "testuser@dbagnostic.test" });

            Assert.IsNotNull(result);

            IEnumerable<dynamic> first = result.Read();

            Assert.IsNotNull(first);
            Assert.IsTrue(first.Count() > 0);
            Assert.IsTrue(first.First().FirstName == "Test");

            IEnumerable<dynamic> second = result.Read();

            Assert.IsNotNull(second);
            Assert.IsTrue(second.Count() > 0);
            Assert.IsTrue(second.First().cnt == 1);
        }

        [Test]
        public void TestUpdate()
        {
            _dbAccess.Execute("UPDATE [User] SET [FirstName] = @FirstName WHERE [EmailAddress] = @EmailAddress", new { FirstName = "Updated", EmailAddress = "testuser@dbagnostic.test" });

            IEnumerable<dynamic> data = _dbAccess.Query<dynamic>("SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = "testuser@dbagnostic.test" });

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count() > 0);
            Assert.IsTrue(data.First().FirstName == "Updated");
        }

        [Test]
        public void TestDelete()
        {
            _dbAccess.Execute("DELETE FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = "testuser@dbagnostic.test" });
            IEnumerable<dynamic> data = _dbAccess.Query<dynamic>("SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = "testuser@dbagnostic.test" });

            Assert.IsEmpty(data);
        }

        

    }
}
