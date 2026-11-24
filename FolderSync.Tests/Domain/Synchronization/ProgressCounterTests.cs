using NSubstitute;

namespace FolderSync.Tests.Domain.Synchronization
{
    public class ProgressCounterTests
    {
        [Fact]
        public void ProgressCounter_建立時_回報零進度()
        {
            // arrange
            var mock = Substitute.For<IProgress<SyncProgress>>();

            // act
            _ = new ProgressCounter("更新", 3, mock);

            // assert
            mock.Received(1).Report(new SyncProgress("更新", 0, 3));
        }

        [Fact]
        public void Increment_全部完成_回報總數()
        {
            // arrange
            var mock = Substitute.For<IProgress<SyncProgress>>();
            var sut = new ProgressCounter("備份", 3, mock);

            // act
            sut.Increment();
            sut.Increment();
            sut.Increment();

            // assert
            mock.Received(1).Report(new SyncProgress("備份", 3, 3));
        }

        [Fact]
        public void Increment_未滿間隔且未完成_不回報()
        {
            // arrange
            var mock = Substitute.For<IProgress<SyncProgress>>();
            var sut = new ProgressCounter("更新", 1000, mock);

            // act
            sut.Increment();

            // assert
            mock.DidNotReceive().Report(new SyncProgress("更新", 1, 1000));
        }

        [Fact]
        public void Increment_並行呼叫_最後回報總數()
        {
            // arrange
            var mock = Substitute.For<IProgress<SyncProgress>>();
            var sut = new ProgressCounter("更新", 500, mock);

            // act
            Parallel.For(0, 500, _ => sut.Increment());

            // assert
            mock.Received(1).Report(new SyncProgress("更新", 500, 500));
        }

        [Theory]
        [InlineData("更新", 20, 500, "更新 20/500")]
        [InlineData("站台離線，等待 5 秒", 0, 0, "站台離線，等待 5 秒")]
        public void SyncProgress_ToString_有總數顯示計數否則只顯示階段(string stage, int done, int total, string expected)
        {
            // arrange
            var sut = new SyncProgress(stage, done, total);

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal(expected, result);
        }
    }
}
