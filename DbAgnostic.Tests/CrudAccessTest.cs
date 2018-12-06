using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbAgnostic.Tests
{
    [TestFixture]
    class CrudAccessTest
    {
        #region Setup
        string _connectionString;
        ICrudAccess<TestModel> _dbAccess;

        public CrudAccessTest()
        {
            _connectionString = ConfigurationHelper.GetConnectionString();
            _dbAccess = DbAgnostic.MsSQL.DbAccessFactory.BuildCrud<TestModel>(_connectionString);
        }
        #endregion

        [Test]
        public void CreateTest()
        {
            string uniqueValue = Guid.NewGuid().ToString();
            TestModel m = _dbAccess.Create(new TestModel
            {
                FirstName = "Test",
                LastName = uniqueValue,
                EmailAddress = uniqueValue + "@dbagnostic.test",
                Bio = "Test bio"
            });

            Assert.Greater(m.UserID, 0);

            //clean up
            _dbAccess.Delete(m.UserID);
        }

        [Test]
        public void UpdateTest()
        {
            string uniqueValue = Guid.NewGuid().ToString();
            TestModel m1 = _dbAccess.Create(new TestModel
            {
                FirstName = "Test",
                LastName = uniqueValue,
                EmailAddress = uniqueValue + "@dbagnostic.test",
                Bio = "Test bio"
            });

            m1.FirstName = "Updated";
            TestModel m2 = _dbAccess.Update(m1);

            Assert.Equals(m2.UserID, m1.UserID);
            Assert.Equals(m2.FirstName, "Updated");
            Assert.AreNotEqual(m2.FirstName, m1.FirstName);
        }

    }
}
