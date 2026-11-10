
using NSubstitute;

namespace FolderSync.Tests.Application.Services
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
            var sut = new AppSettingsService(repo, new AppSettingsProvider(repo), Substitute.For<IDirectoryProbe>());

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
            var sut = new AppSettingsService(repo, new AppSettingsProvider(repo), Substitute.For<IDirectoryProbe>());

            // Act & Assert
            Assert.Throws<DomainException>(() => sut.Save("", "", "bad:name", "", "0"));
            repo.DidNotReceive().Save(Arg.Any<AppSettings>());
        }

        [Fact]
        public void Save_來源資料夾不存在_拋出DomainException()
        {
            // Arrange
            var repo = Substitute.For<IAppSettingsRepository>();
            var sut = new AppSettingsService(repo, new AppSettingsProvider(repo), Substitute.For<IDirectoryProbe>());

            // Act & Assert
            Assert.Throws<DomainException>(
                () => sut.Save("X:\\not\\exist\\path", "", "", "", "0"));
        }

        [Fact]
        public void Save_延遲非數字_視為零()
        {
            // Arrange
            var repo = Substitute.For<IAppSettingsRepository>();
            repo.Load().Returns(new AppSettings());
            var sut = new AppSettingsService(repo, new AppSettingsProvider(repo), Substitute.For<IDirectoryProbe>());

            // Act
            sut.Save("", "", "", "", "abc");

            // Assert
            repo.Received(1).Save(Arg.Is<AppSettings>(s => s.SiteDelay == 0));
        }

        [Theory]
        [InlineData("appsettings*.json")]
        [InlineData("*.pdb")]
        [InlineData("a?.txt")]
        public void Save_忽略檔名含萬用字元_儲存設定(string ignoreFiles)
        {
            // arrange
            var mock = CreateRepository();
            var sut = CreateService(mock);

            // act
            sut.Save("", "", "", ignoreFiles, "0");

            // assert
            mock.Received(1).Save(Arg.Is<AppSettings>(s => s.IgnoreFiles.Single() == ignoreFiles));
        }

        [Theory]
        [InlineData("a\\b.txt")]
        [InlineData("a/b.txt")]
        [InlineData("a:b.txt")]
        [InlineData("a\"b.txt")]
        [InlineData("a<b.txt")]
        [InlineData("a>b.txt")]
        [InlineData("a|b.txt")]
        public void Save_忽略檔名含其他無效字元_拋出例外且不儲存(string ignoreFiles)
        {
            // arrange
            var mock = CreateRepository();
            var sut = CreateService(mock);

            // act
            var act = () => sut.Save("", "", "", ignoreFiles, "0");

            // assert
            Assert.Throws<DomainException>(act);
            mock.DidNotReceive().Save(Arg.Any<AppSettings>());
        }

        [Theory]
        [InlineData("log*")]
        [InlineData("log?")]
        public void Save_忽略資料夾含萬用字元_拋出例外且不儲存(string ignoreFolders)
        {
            // arrange
            var mock = CreateRepository();
            var sut = CreateService(mock);

            // act
            var act = () => sut.Save("", "", ignoreFolders, "", "0");

            // assert
            Assert.Throws<DomainException>(act);
            mock.DidNotReceive().Save(Arg.Any<AppSettings>());
        }

        private static IAppSettingsRepository CreateRepository()
        {
            var repository = Substitute.For<IAppSettingsRepository>();
            repository.Load().Returns(new AppSettings());
            return repository;
        }

        private static AppSettingsService CreateService(IAppSettingsRepository repository)
        {
            return new AppSettingsService(repository, new AppSettingsProvider(repository), Substitute.For<IDirectoryProbe>());
        }
    }
}
