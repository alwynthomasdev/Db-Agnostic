using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbAgnostic.Tests
{

    //TODO: proper test messages

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
        public void GetTest()
        {
            //prepare 
            TestModel m1 = new TestModel { FirstName = "test", LastName = "user1", EmailAddress = "testuser1@dbagnostic.test" };
            TestModel m2 = new TestModel { FirstName = "test", LastName = "user2", EmailAddress = "testuser2@dbagnostic.test" };
            TestModel m3 = new TestModel { FirstName = "test", LastName = "user3", EmailAddress = "testuser3@dbagnostic.test" };
            m1 = _dbAccess.Create(m1);
            m2 = _dbAccess.Create(m2);
            m3 = _dbAccess.Create(m3);

            IEnumerable<TestModel> result = _dbAccess.Get();

            Assert.IsNotEmpty(result);
            Assert.AreEqual(3, result.Count());
            Assert.IsNotNull(result.Where(x => x.EmailAddress == "testuser2@dbagnostic.test").SingleOrDefault());

            //clean up
            _dbAccess.Delete(m1.UserID);
            _dbAccess.Delete(m2.UserID);
            _dbAccess.Delete(m3.UserID);
        }

        [Test]
        public void GetByIdTest()
        {
            //prepare 
            TestModel m1 = new TestModel { FirstName = "test", LastName = "user1", EmailAddress = "testuser1@dbagnostic.test" };
            TestModel m2 = new TestModel { FirstName = "test", LastName = "user2", EmailAddress = "testuser2@dbagnostic.test" };
            TestModel m3 = new TestModel { FirstName = "test", LastName = "user3", EmailAddress = "testuser3@dbagnostic.test" };
            m1 = _dbAccess.Create(m1);
            m2 = _dbAccess.Create(m2);
            m3 = _dbAccess.Create(m3);

            //test using the m2 userid
            TestModel result = _dbAccess.Get(m2.UserID);
            Assert.IsNotNull(result);
            Assert.AreEqual("testuser2@dbagnostic.test", result.EmailAddress);

            //clean up
            _dbAccess.Delete(m1.UserID);
            _dbAccess.Delete(m2.UserID);
            _dbAccess.Delete(m3.UserID);
        }

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

        [Test]
        public void UpdateByTest()
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

            updated = _dbAccess.UpdateBy("EmailAddress", updated);

            Assert.AreEqual(updated.UserID, original.UserID);
            Assert.AreEqual(updated.FirstName, "Updated");
            Assert.AreNotEqual(updated.FirstName, original.FirstName);

            //clean up
            _dbAccess.Delete(updated.UserID);
        }

        [Test]
        public void DeleteTest()
        {
            //create the user to delete
            string uniqueValue = Guid.NewGuid().ToString();
            TestModel m = _dbAccess.Create(new TestModel
            {
                FirstName = "Test",
                LastName = uniqueValue,
                EmailAddress = uniqueValue + "@dbagnostic.test",
                Bio = "Test bio"
            });
            //run delete
            _dbAccess.Delete(m.UserID);

            //check to see if it exists
            TestModel result = _dbAccess.Get(m.UserID);

            Assert.IsNull(result);
        }

        [Test]
        public void DeleteByTest()
        {
            //create the user to delete
            string uniqueValue = Guid.NewGuid().ToString();
            TestModel m = _dbAccess.Create(new TestModel
            {
                FirstName = "Test",
                LastName = uniqueValue,
                EmailAddress = uniqueValue + "@dbagnostic.test",
                Bio = "Test bio"
            });
            //run delete
            _dbAccess.DeleteBy("EmailAddress", m.EmailAddress);

            //check to see if it exists
            TestModel result = _dbAccess.Get(m.UserID);

            Assert.IsNull(result);
        }

        [Test]
        public void GetByTest()
        {
            //prepare 
            TestModel m1 = new TestModel { FirstName = "test", LastName = "user1", EmailAddress = "testuser1@dbagnostic.test" };
            TestModel m2 = new TestModel { FirstName = "test_a", LastName = "user2", EmailAddress = "testuser2@dbagnostic.test" };
            TestModel m3 = new TestModel { FirstName = "test_a", LastName = "user3", EmailAddress = "testuser3@dbagnostic.test" };
            m1 = _dbAccess.Create(m1);
            m2 = _dbAccess.Create(m2);
            m3 = _dbAccess.Create(m3);

            IEnumerable<TestModel> result = _dbAccess.GetBy("FirstName", "test_a");

            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsNotNull(result.Where(x => x.EmailAddress == "testuser2@dbagnostic.test").SingleOrDefault());

            //clean up
            _dbAccess.Delete(m1.UserID);
            _dbAccess.Delete(m2.UserID);
            _dbAccess.Delete(m3.UserID);
        }

        [Test]
        public void GetSingleByTest()
        {
            //prepare 
            TestModel m1 = new TestModel { FirstName = "test", LastName = "user1", EmailAddress = "testuser1@dbagnostic.test" };
            TestModel m2 = new TestModel { FirstName = "test", LastName = "user2", EmailAddress = "testuser2@dbagnostic.test" };
            TestModel m3 = new TestModel { FirstName = "test", LastName = "user3", EmailAddress = "testuser3@dbagnostic.test" };
            m1 = _dbAccess.Create(m1);
            m2 = _dbAccess.Create(m2);
            m3 = _dbAccess.Create(m3);

            //test using the m2 userid
            TestModel result = _dbAccess.GetSingleBy("EmailAddress", m2.EmailAddress);
            Assert.IsNotNull(result);
            Assert.AreEqual("testuser2@dbagnostic.test", result.EmailAddress);

            //clean up
            _dbAccess.Delete(m1.UserID);
            _dbAccess.Delete(m2.UserID);
            _dbAccess.Delete(m3.UserID);
        }

    }
}
