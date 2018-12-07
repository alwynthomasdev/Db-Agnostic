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
            TestModel original = _dbAccess.Create(new TestModel
            {
                FirstName = "Test",
                LastName = uniqueValue,
                EmailAddress = uniqueValue + "@dbagnostic.test",
                Bio = "Test bio"
            });

            TestModel updated = original.Clone();//create a copy, not ref
            updated.FirstName = "Updated";

            updated = _dbAccess.Update(updated);

            Assert.AreEqual(updated.UserID, original.UserID);
            Assert.AreEqual(updated.FirstName, "Updated");
            Assert.AreNotEqual(updated.FirstName, original.FirstName);

            //clean up
            _dbAccess.Delete(updated.UserID);
        }

    }
}
