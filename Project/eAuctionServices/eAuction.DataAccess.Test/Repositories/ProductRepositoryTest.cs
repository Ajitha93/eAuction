using eAuction.DataAccess.DBContext;
using eAuction.DataAccess.Repositories.ProductRepositories;
using eAuction.Models.EntityModels;
using MongoDB.Bson;
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
    public class ProductRepositoryTest
    {
        private Mock<IMongoCollection<Products>> _mockCollection;
        private Mock<IMongoDBContext> _mockContext;
        private Products _product;
        private List<Products> _productLst;

        public ProductRepositoryTest()
        {
            _mockCollection = new Mock<IMongoCollection<Products>>();
            _mockContext = new Mock<IMongoDBContext>();
            _productLst = new List<Products>
            {

                    new Products { ProductId = "5dc1039a1521eaa36835e541", ProductName="Painting", ShortDescription="Canavas Painting",DetailedDescription="Canavas Painting Desc"
                        ,Category="Painting",StartingPrice=100,BidEndDate=DateTime.Now,FirstName="Ajitha",LastName="Gangineni",Address="Edward Street",City="Reston",State="VA",Pin="20945"
                        ,Phone="6573452341",Email="AG@gmail.com"}
                    , new Products {ProductId = "72c1039a1521eaa36835e541", ProductName="Ornament", ShortDescription="Gold Ornament",DetailedDescription="Gold Ornament Desc"
                        ,Category="Ornament",StartingPrice=300,BidEndDate=DateTime.Now.AddDays(45),FirstName="Ashish",LastName="Bhagam",Address="Hunts Ville",City="Chicago",State="IL",Pin="30080"
                        ,Phone="2341235346",Email="AB@gmail.com" } };
            _product = new Products
            {
                ProductId = "6ec1039a1521eaa36835e541",
                ProductName = "Sculpture",
                ShortDescription = "Stone Sculpture",
                DetailedDescription = "Stone Sculpture Desc",
                Category = "Sculpture",
                StartingPrice = 200,
                BidEndDate = DateTime.Now,
                FirstName = "Ajitha",
                LastName = "Gangineni",
                Address = "Edward Street",
                City = "Reston",
                State = "VA",
                Pin = "20945",
                Phone = "6573452341",
                Email = "AG@gmail.com"
            };
        }

        public void IntializeMongoCollection()
        {
            //Mock MoveNext

            Mock<IAsyncCursor<Products>> _productCursor = new Mock<IAsyncCursor<Products>>();
            _productCursor.Setup(_ => _.Current).Returns(_productLst);
            _productCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            //Mock FindSync

            _mockCollection.Setup(op => op.FindSync(It.IsAny<FilterDefinition<Products>>(),
            It.IsAny<FindOptions<Products, Products>>(),
            It.IsAny<CancellationToken>())).Returns(_productCursor.Object);

            //Mock GetCollection

            _mockContext.Setup(c => c.GetCollection<Products>(typeof(Products).Name)).Returns(_mockCollection.Object);
        }

        [Test]
        public void ProductRepository_GetProduct_Valid_Success()
        {
            //Arrange
            IntializeMongoCollection();


            var productRepo = new ProductRepository(_mockContext.Object);

            //Act
            var result = productRepo.GetProduct("72c1039a1521eaa36835e541");

            //Assert
            Assert.AreEqual(result.ProductName, "Ornament");

        }

        [Test]
        public void ProductRepository_CreateNewProduct_Valid_Success()
        {
            //Arrange
            _mockCollection.Setup(op => op.InsertOneAsync(_product, null,
            default(CancellationToken))).Returns(Task.CompletedTask);

            _mockContext.Setup(c => c.GetCollection<Products>
            (typeof(Products).Name)).Returns(_mockCollection.Object);
            var productRepo = new ProductRepository(_mockContext.Object);

            //Act
            productRepo.Create(_product);

            //Assert 

            //Verify if InsertOneAsync is called once 
            _mockCollection.Verify(c => c.InsertOne(_product, null, default(CancellationToken)), Times.Once);
        }

        
        [Test]
        public void ProductRepository_CreateNewProduct_Null_Product_Failure()
        {
            // Arrange
            _product = null;

            //Act 
            var productRepo = new ProductRepository(_mockContext.Object);

            // Assert
            Assert.Throws<ArgumentNullException>(() => productRepo.Create(_product));
        }

        [Test]
        public void ProductRepository_DeleteProduct_Valid_Success()
        {
            //IntializeMongoCollection();

            _mockCollection.Setup(op => op.InsertOneAsync(_product, null,
             default(CancellationToken))).Returns(Task.CompletedTask);

            _mockContext.Setup(c => c.GetCollection<Products>
            (typeof(Products).Name)).Returns(_mockCollection.Object);
            var productRepo = new ProductRepository(_mockContext.Object);

            //Act
            var res=productRepo.Delete("72c1039a1521eaa36835e541");

            //Assert 
            Assert.IsTrue(res);
        }
    }
}
