using Xunit;
using Moq;

public class UnitTest
{
  [Fact]
  public async Task TestItemsPost()
  {
    // Arrange
    var mockController = new Mock<ItemsController>();
    mockController
        .Setup(x => x.DataAll(It.IsAny<string>(), It.IsAny<int>()))
        .ReturnsAsync(new { myProp = 555 });

    var testObj = new SearchModel { zip = "22042", radius = 2 };

    // Act
    await mockController.Object.ItemsPost(testObj);

    // Assert
    mockController.Verify(x => x.DataAll(testObj.zip, testObj.radius), Times.Once);
  }
}
