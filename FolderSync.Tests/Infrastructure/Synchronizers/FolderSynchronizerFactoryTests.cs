using NSubstitute;

namespace FolderSync.Tests.Infrastructure.Synchronizers
{
    public class FolderSynchronizerFactoryTests
    {
        [Fact]
        public void Modes_回傳所有同步模式()
        {
            // Act & Assert
            Assert.Equal(new[] { SyncMode.資料夾, SyncMode.站台 }, NewFactory().Modes);
        }

        [Theory]
        [InlineData(SyncMode.資料夾)]
        [InlineData(SyncMode.站台)]
        public void Create_依模式回傳同步器(SyncMode mode)
        {
            // Act & Assert
            Assert.NotNull(NewFactory().Create(mode));
        }

        [Fact]
        public void Create_不支援的模式_拋出NotSupportedException()
        {
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => NewFactory().Create((SyncMode)999));
        }

        private static FolderSynchronizerFactory NewFactory()
        {
            var backups = Substitute.For<IBackupRepository>();
            var repo = Substitute.For<IAppSettingsRepository>();
            repo.Load().Returns(new AppSettings());
            return new FolderSynchronizerFactory(backups, new AppSettingsProvider(repo));
        }
    }
}
