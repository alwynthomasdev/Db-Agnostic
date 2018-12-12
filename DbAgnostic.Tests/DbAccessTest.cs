using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DbAgnostic.Tests
{
    [TestFixture]
    public class DbAccessTest
    {
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

            Assert.IsNotNull(con, "The connection string created was NULL.");
            Assert.AreEqual(_connectionString, con.ConnectionString, "Unexpected connection string.");
        }

        [Test]
        public void TestInsertAndSelect()
        {
            string uniqueValue = Guid.NewGuid().ToString();
            string EmailAddress = uniqueValue + "@dbagnostic.test";

            //insert a unique value into the databasle
            _dbAccess.Execute("INSERT INTO [User] ([FirstName], [LastName], [EmailAddress]) VALUES (@FirstName, @LastName, @EmailAddress)",
                new { FirstName = "Test", LastName = uniqueValue, EmailAddress = EmailAddress });

            //get the data just inserted
            IEnumerable<dynamic> data = _dbAccess.Query<dynamic>("SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = EmailAddress });

            Assert.IsNotNull(data, "No data found (NULL) with EmailAddress '{0}'", EmailAddress);
            Assert.IsTrue(data.Count() > 0, "0 records found with the EmailAddress '{0}'.", EmailAddress);
            Assert.IsTrue(data.First().LastName == uniqueValue, "The LastName field for the data retrived does not match the expected value of '{0}'.", uniqueValue);

            //clean up
            _dbAccess.Execute("DELETE FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = EmailAddress });
        }

        [Test]
        public void TestUpdate()
        {
            string uniqueValue = Guid.NewGuid().ToString();
            string EmailAddress = uniqueValue + "@dbagnostic.test";

            //insert the data first
            _dbAccess.Execute("INSERT INTO [User] ([FirstName], [LastName], [EmailAddress]) VALUES (@FirstName, @LastName, @EmailAddress)",
                new { FirstName = "Test", LastName = uniqueValue, EmailAddress = EmailAddress });

            //run the update
            _dbAccess.Execute("UPDATE [User] SET [FirstName] = @FirstName WHERE [EmailAddress] = @EmailAddress", new { FirstName = "Updated", EmailAddress = EmailAddress });

            //get the updated data
            IEnumerable<dynamic> data = _dbAccess.Query<dynamic>("SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = EmailAddress });

            Assert.IsNotNull(data, "No data found (NULL) with EmailAddress '{0}'", EmailAddress);
            Assert.IsTrue(data.Count() > 0, "0 records found with the EmailAddress '{0}'.", EmailAddress);
            Assert.IsTrue(data.First().FirstName == "Updated", "The FirstName field for the data retrived does not match the expected value of 'Updated'.");

            //clean up
            _dbAccess.Execute("DELETE FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = EmailAddress });
        }

        [Test]
        public void TestDelete()
        {
            string uniqueValue = Guid.NewGuid().ToString();
            string EmailAddress = uniqueValue + "@dbagnostic.test";

            //insert the data first
            _dbAccess.Execute("INSERT INTO [User] ([FirstName], [LastName], [EmailAddress]) VALUES (@FirstName, @LastName, @EmailAddress)",
                new { FirstName = "Test", LastName = uniqueValue, EmailAddress = EmailAddress });

            //delete the data
            _dbAccess.Execute("DELETE FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = EmailAddress });

            //check to see if the data is found
            IEnumerable<dynamic> data = _dbAccess.Query<dynamic>("SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = EmailAddress });

            //expect empty
            Assert.IsEmpty(data, "Data found with EmailAddress '{0}', record has not been deleted.", EmailAddress);
        }

        [Test]
        public void TestQueryFirstOrDefault()
        {
            string uniqueValue = Guid.NewGuid().ToString();
            string EmailAddress = uniqueValue + "@dbagnostic.test";

            //insert the data first
            _dbAccess.Execute("INSERT INTO [User] ([FirstName], [LastName], [EmailAddress]) VALUES (@FirstName, @LastName, @EmailAddress)",
                new { FirstName = "Test", LastName = uniqueValue, EmailAddress = EmailAddress });

            //select data
            dynamic data = _dbAccess.QueryFirstOrDefault<dynamic>("SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = EmailAddress });

            Assert.IsNotNull(data, "No data found (NULL) with EmailAddress '{0}'", EmailAddress);
            Assert.IsTrue(data.LastName == uniqueValue, "The LastName field for the data retrived does not match the expected value of '{0}'.", uniqueValue);

            //clean up
            _dbAccess.Execute("DELETE FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = EmailAddress });
        }

        [Test]
        public void TestQueryMultiple()
        {
            string uniqueValue = Guid.NewGuid().ToString();
            string EmailAddress = uniqueValue + "@dbagnostic.test";

            //insert the data first
            _dbAccess.Execute("INSERT INTO [User] ([FirstName], [LastName], [EmailAddress]) VALUES (@FirstName, @LastName, @EmailAddress)",
                new { FirstName = "Test", LastName = uniqueValue, EmailAddress = EmailAddress });

            IDbConnection con = _dbAccess.GetConnection();
            QueryMultipleResultReader result = _dbAccess.QueryMultiple(con, @"
SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress;
SELECT COUNT(*) AS [cnt] FROM [User] WHERE [EmailAddress] = @EmailAddress;
", 
                new { EmailAddress = EmailAddress });

            Assert.IsNotNull(result, "No data found with QueryMultiple.");
            IEnumerable<dynamic> first = result.Read();
            IEnumerable<dynamic> second = result.Read();
            //
            Assert.IsNotNull(first, "Result from first query is null");
            Assert.IsTrue(first.Count() > 0, "The count of results returned by the first query is not greater than 0.");
            Assert.IsTrue(first.First().LastName == uniqueValue, "The LastName field for the data retrived does not match the expected value of '{0}'.", uniqueValue);
            //
            Assert.IsNotNull(second, "Result from second query is null");
            Assert.IsTrue(second.Count() > 0, "The count of results returned by the second query is not greater than 0.");
            Assert.IsTrue(second.First().cnt == 1, "The count of users in the database with the EmailAddress '{0}' is not equal to 1.", EmailAddress);

            //clean up
            _dbAccess.Execute("DELETE FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = EmailAddress });
        }

        [Test]
        public void TestTransactionFail()
        {
            string uniqueValue = Guid.NewGuid().ToString();
            string EmailAddress = uniqueValue + "@dbagnostic.test";

            using (IDbConnection con = _dbAccess.GetConnection())
            {
                con.Open();
                using (IDbTransaction trn = con.BeginTransaction())
                {
                    try
                    {
                        //insert a unique value into the database
                        _dbAccess.Execute(con, trn, "INSERT INTO [User] ([FirstName], [LastName], [EmailAddress]) VALUES (@FirstName, @LastName, @EmailAddress)",
                            new { FirstName = "Test", LastName = uniqueValue, EmailAddress = EmailAddress });
                        throw new Exception("Test exception");
                        //trn.Commit();//this will never commit
                    }
                    catch(Exception ex)
                    {
                        trn.Rollback();
                    }
                }
            }
            //try to get the data inserted in transaction 
            IEnumerable<dynamic> data = _dbAccess.Query<dynamic>("SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = EmailAddress });

            //check no data found
            Assert.IsEmpty(data, "Data found with EmailAddress '{0}', data should not be committed in a rollback transaction", EmailAddress);
        }

        [Test]
        public void TestTransactionSuccess()
        {
            string uniqueValue = Guid.NewGuid().ToString();
            string EmailAddress = uniqueValue + "@dbagnostic.test";

            using (IDbConnection con = _dbAccess.GetConnection())
            {
                con.Open();
                using (IDbTransaction trn = con.BeginTransaction())
                {
                    //insert a unique value into the databasle
                    _dbAccess.Execute(con, trn, "INSERT INTO [User] ([FirstName], [LastName], [EmailAddress]) VALUES (@FirstName, @LastName, @EmailAddress)",
                        new { FirstName = "Test", LastName = uniqueValue, EmailAddress = EmailAddress });

                    trn.Commit();
                }
            }

            //get the data just inserted
            IEnumerable<dynamic> data = _dbAccess.Query<dynamic>("SELECT * FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = EmailAddress });

            Assert.IsNotNull(data, "No data found (NULL) with EmailAddress '{0}'", EmailAddress);
            Assert.IsTrue(data.Count() > 0, "0 records found with the EmailAddress '{0}'.", EmailAddress);
            Assert.IsTrue(data.First().LastName == uniqueValue, "The LastName field for the data retrived does not match the expected value of '{0}'.", uniqueValue);

            //clean up
            _dbAccess.Execute("DELETE FROM [User] WHERE [EmailAddress] = @EmailAddress", new { EmailAddress = EmailAddress });
        }
    }
}
