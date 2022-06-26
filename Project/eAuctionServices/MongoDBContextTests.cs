using System;

public class MongoDBContextTests
{
    //public Class1()
    //{
    [Test]
    public void MongoDBContext_Constructor_Success()
    {
        var settings = new Mongosettings()
        {
            Connection = "mongodb://tes123 ",
            DatabaseName = "TestDB"
        };

        _mockOptions.Setup(s => s.Value).Returns(settings);
        _mockClient.Setup(c => c
        .GetDatabase(_mockOptions.Object.Value.DatabaseName, null))
            .Returns(_mockDB.Object);

        //Act 
        var context = new MongoBookDBContext(_mockOptions.Object);

        //Assert 
        Assert.NotNull(context);
    }
}
