using eAuction.DataAccess.DBContext;
using eAuction.DataAccess.Repositories.BuyerRepositories;
using eAuction.Models.EntityModels;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eAuction.DataAccess.Test.Repositories
{
    public class BuyersRepositoryTest
    {
        private Mock<IMongoCollection<Buyers>> _mockCollection;
        private Mock<IMongoDBContext> _mockContext;
        private List<Buyers> _buyerLst;
        private Buyers _buyer;

        public BuyersRepositoryTest()
        {
            _mockCollection = new Mock<IMongoCollection<Buyers>>();
            _mockContext = new Mock<IMongoDBContext>();
            _buyerLst = new List<Buyers>
            {

                    new Buyers { BuyerId="b1c1039a1521eaa36835e541", ProductId = "5dc1039a1521eaa36835e541",BidAmount=200 
                        ,FirstName="AshaLatha",LastName="Gangineni",Address="Elk Street",City="Newyork",State="NY",Pin="39231"
                        ,Phone="4532890432",Email="AN@gmail.com"}
                    , new Buyers {BuyerId="b1c1039a1521eaa36835e541", ProductId = "5dc1039a1521eaa36835e541",BidAmount=800
                        ,FirstName="GopiKiran",LastName="Nuthalapati",Address="Deer Ville",City="Dallas",State="TX",Pin="40080"
                        ,Phone="7861235346",Email="GN@gmail.com" } };
            _buyer = new Buyers
            {
                BuyerId = "b1c1039a1521eaa36835e541",
                ProductId = "72c1039a1521eaa36835e541",
                BidAmount = 800,
                FirstName = "AshaLatha",
                LastName = "Gangineni",
                Address = "Elk Street",
                City = "Newyork",
                State = "NY",
                Pin = "39231",
                Phone = "4532890432",
                Email = "AN@gmail.com"
            };
        }

        public void IntializeMongoCollection()
        {
            //Mock MoveNext

            Mock<IAsyncCursor<Buyers>> _buyerCursor = new Mock<IAsyncCursor<Buyers>>();
            _buyerCursor.Setup(_ => _.Current).Returns(_buyerLst);
            _buyerCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            //Mock FindSync

            _mockCollection.Setup(op => op.FindSync(It.IsAny<FilterDefinition<Buyers>>(),
            It.IsAny<FindOptions<Buyers, Buyers>>(),
            It.IsAny<CancellationToken>())).Returns(_buyerCursor.Object);

            //Mock GetCollection

            _mockContext.Setup(c => c.GetCollection<Buyers>(typeof(Buyers).Name)).Returns(_mockCollection.Object);

        }

        [Test]
        public void BuyerRepository_GetBuyer_Valid_Success()
        {
            //Arrange
            IntializeMongoCollection();

            var buyerRepo = new BuyerRepository(_mockContext.Object);

            //Act
            var result = buyerRepo.GetBuyer("5dc1039a1521eaa36835e541");

            //Assert
            Assert.AreEqual(result?.Count(), 2);
        }

        [Test]
        public void BuyerRepository_CreateNewBuyer_Valid_Success()
        {
            //Arrange
            _mockCollection.Setup(op => op.InsertOneAsync(_buyer, null,
            default(CancellationToken))).Returns(Task.CompletedTask);

            _mockContext.Setup(c => c.GetCollection<Buyers>
            (typeof(Buyers).Name)).Returns(_mockCollection.Object);
            var buyerRepo = new BuyerRepository(_mockContext.Object);

            //Act
            buyerRepo.Create(_buyer);

            //Assert 

            //Verify if InsertOneAsync is called once 
            _mockCollection.Verify(c => c.InsertOne(_buyer, null, default(CancellationToken)), Times.Once);
        }


        [Test]
        public void BuyerRepository_CreateNewBuyer_Null_Buyer_Failure()
        {
            // Arrange
            _buyer = null;

            //Act 
            var buyerRepo = new BuyerRepository(_mockContext.Object);

            // Assert
            Assert.Throws<ArgumentNullException>(() => buyerRepo.Create(_buyer));
        }

        [Test]
        public void BuyerRepository_UpdateBuyer_Valid_Success()
        {
            IntializeMongoCollection();
            ////Arrange
            //Task.Run(await async()=>_mockCollection.Setup(op => op.InsertOneAsync(_buyer, null,
            //default(CancellationToken))).Returns(Task.CompletedTask));

            //_mockContext.Setup(c => c.GetCollection<Buyers>
            //(typeof(Buyers).Name)).Returns(_mockCollection.Object);
            //var buyerRepo = new BuyerRepository(_mockContext.Object);

            var buyerRepo = new BuyerRepository(_mockContext.Object);

            //Act
            var res=buyerRepo.Update(_buyer.BuyerId,_buyer);

            //Assert 
            Assert.IsTrue(res);
        }
    }
}
