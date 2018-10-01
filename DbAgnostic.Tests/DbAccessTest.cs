using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DbAgnostic.Tests
{
    [TestFixture]
    public class DbAccessTest
    {
        [Test]
        public void Test()
        {
            //TODO: set up test database 


            IDbAccess db = DbAccessFactory.Build(() => new SqlConnection(""));
        }


        //[Test]
        //public void TestTest()
        //{
        //    Assert.True(false, "this is false...");
        //}
    }
}
