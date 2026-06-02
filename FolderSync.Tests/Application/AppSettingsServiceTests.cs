using FolderSync.Application;
using FolderSync.Domain;

using NSubstitute;

namespace FolderSync.Tests.Application
{
    public class AppSettingsServiceTests
    {
        // 註：忽略名稱含無效字元時改拋 DomainException

        [Fact]
        public void Save_有效輸入_儲存對應設定()
        {
            // Arrange
            var repo = Substitute.For<IAppSettingsRepository>();
            repo.Load().Returns(new AppSettings());
            var sut = new AppSettingsService(repo);

            // Act
            sut.Save("", "", $"bin{Environment.NewLine}obj", "web.config", "3000");

            // Assert
            repo.Received(1).Save(Arg.Is<AppSettings>(s =>
                s.IgnoreFolders.Length == 2 &&
                s.IgnoreFiles.Length == 1 &&
                s.SiteDelay == 3000));
        }

        [Fact]
        public void Save_忽略名稱含無效字元_拋出例外且不儲存()
        {
            // Arrange
            var repo = Substitute.For<IAppSettingsRepository>();
            repo.Load().Returns(new AppSettings());
            var sut = new AppSettingsService(repo);

            // Act & Assert
            Assert.Throws<DomainException>(() => sut.Save("", "", "bad:name", "", "0"));
            repo.DidNotReceive().Save(Arg.Any<AppSettings>());
        }

        [Fact]
        public void Save_來源資料夾不存在_拋出DirectoryNotFoundException()
        {
            // Arrange
            var repo = Substitute.For<IAppSettingsRepository>();
            var sut = new AppSettingsService(repo);

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(
                () => sut.Save("X:\\not\\exist\\path", "", "", "", "0"));
        }

        [Fact]
        public void Save_延遲非數字_視為零()
        {
            // Arrange
            var repo = Substitute.For<IAppSettingsRepository>();
            repo.Load().Returns(new AppSettings());
            var sut = new AppSettingsService(repo);

            // Act
            sut.Save("", "", "", "", "abc");

            // Assert
            repo.Received(1).Save(Arg.Is<AppSettings>(s => s.SiteDelay == 0));
        }
    }
}
