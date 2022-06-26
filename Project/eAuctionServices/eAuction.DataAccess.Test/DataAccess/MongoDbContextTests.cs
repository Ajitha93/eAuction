using eAuction.DataAccess.DBContext;
using eAuction.Models.EntityModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.DataAccess.Test.DataAccess
{
    public class MongoDbContextTests
    {
        private Mock<IOptions<Mongosettings>> _mockOptions;
        private Mock<IMongoDatabase> _mockDB;
        private Mock<IMongoClient> _mockClient;

        public MongoDbContextTests()
        {
            _mockOptions = new Mock<IOptions<Mongosettings>>();
            _mockDB = new Mock<IMongoDatabase>();
            _mockClient = new Mock<IMongoClient>();
        }

        [Test]
        public void MongoDBContext_Constructor_Success()
        {
            var settings = new Mongosettings()
            {
                ConnectionString = "mongodb://tes123 ",
                DatabaseName = "TestDB"
            };

            _mockOptions.Setup(s => s.Value).Returns(settings);
            _mockClient.Setup(c => c
            .GetDatabase(_mockOptions.Object.Value.DatabaseName, null))
                .Returns(_mockDB.Object);

            //Act 
            var context = new MongoDBContext(_mockOptions.Object);

            //Assert 
            Assert.NotNull(context);
        }

        [Test]
        public void MongoDBContext_GetCollection_NameEmpty_Failure()
        {

            //Arrange
            var settings = new Mongosettings()
            {
                ConnectionString = "mongodb://tes123",
                DatabaseName = "TestDB"
            };

            _mockOptions.Setup(s => s.Value).Returns(settings);
            _mockClient.Setup(c => c
            .GetDatabase(_mockOptions.Object.Value.DatabaseName, null))
                .Returns(_mockDB.Object);

            //Act 
            var context = new MongoDBContext(_mockOptions.Object);
            var myCollection = context.GetCollection<Products>("");

            //Assert 
            Assert.Null(myCollection);
        }

        [Test]
        public void MongoBookDBContext_GetCollection_ValidName_Success()
        {
            //Arrange
            var settings = new Mongosettings()
            {
                ConnectionString = "mongodb://tes123 ",
                DatabaseName = "TestDB"
            };

            _mockOptions.Setup(s => s.Value).Returns(settings);

            _mockClient.Setup(c => c.GetDatabase(_mockOptions.Object.Value.DatabaseName, null)).Returns(_mockDB.Object);

            //Act 
            var context = new MongoDBContext(_mockOptions.Object);
            var myCollection = context.GetCollection<Products>("Products");

            //Assert 
            Assert.NotNull(myCollection);
        }
    }
}
