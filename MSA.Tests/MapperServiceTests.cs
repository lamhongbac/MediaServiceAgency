using Shared.Library.Utilities;
using MSA.BLL.DTOs;
using MSA.BLL;
using Xunit;

namespace MSA.Tests
{
    public class MapperServiceTests
    {
        [Fact]
        public void MapTo_ShouldConvertUploadRequestDtoToUploadMediaRequest()
        {
            // Arrange
            var source = new UploadRequestDto
            {
                AppCode = "POS",
                MediaType = "Images",
                Entity = "Product",
                UniqueCode = "P001"
            };

            // Act
            var destination = source.MapTo<UploadRequestDto, UploadMediaRequest>();

            // Assert
            Assert.NotNull(destination);
            Assert.Equal(source.AppCode, destination.AppCode);
            Assert.Equal(source.MediaType, destination.MediaType);
            Assert.Equal(source.Entity, destination.Entity);
            Assert.Equal(source.UniqueCode, destination.UniqueCode);
        }

        [Fact]
        public void MapTo_ShouldReturnDefault_WhenSourceIsNull()
        {
            // Arrange
            UploadRequestDto source = null;

            // Act
            var destination = source.MapTo<UploadRequestDto, UploadMediaRequest>();

            // Assert
            Assert.Null(destination);
        }
    }
}
