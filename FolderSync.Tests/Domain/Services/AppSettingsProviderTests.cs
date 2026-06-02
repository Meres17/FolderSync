using NSubstitute;

namespace FolderSync.Tests.Domain.Services
{
    public class AppSettingsProviderTests
    {
        [Fact]
        public void Current_建構時載入設定()
        {
            // Arrange
            var repo = Substitute.For<IAppSettingsRepository>();
            repo.Load().Returns(new AppSettings { Source = "a" });

            // Act
            var sut = new AppSettingsProvider(repo);

            // Assert
            Assert.Equal("a", sut.Current.Source);
        }

        [Fact]
        public void Reload_重新從Repository載入最新設定()
        {
            // Arrange
            var repo = Substitute.For<IAppSettingsRepository>();
            repo.Load().Returns(new AppSettings { Source = "a" }, new AppSettings { Source = "b" });
            var sut = new AppSettingsProvider(repo);

            // Act
            sut.Reload();

            // Assert
            Assert.Equal("b", sut.Current.Source);
        }
    }
}
