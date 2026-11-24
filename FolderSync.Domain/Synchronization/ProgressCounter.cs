namespace FolderSync.Domain.Synchronization
{
    /// <summary>
    /// 可並行呼叫的檔案計數器；每 100ms 或全部完成時才回報，避免大量檔案時灌爆 UI。
    /// </summary>
    public class ProgressCounter
    {
        private const long IntervalMs = 100;

        private readonly string _stage;
        private readonly int _total;
        private readonly IProgress<SyncProgress>? _progress;
        private int _done;
        private long _lastReport;

        public ProgressCounter(string stage, int total, IProgress<SyncProgress>? progress)
        {
            _stage = stage;
            _total = total;
            _progress = progress;
            _lastReport = Environment.TickCount64;
            _progress?.Report(new SyncProgress(stage, 0, total));
        }

        public void Increment()
        {
            var done = Interlocked.Increment(ref _done);
            if (done == _total || TryClaimReport())
            {
                _progress?.Report(new SyncProgress(_stage, done, _total));
            }
        }

        // 多執行緒同時到期時只讓一個回報。
        private bool TryClaimReport()
        {
            var now = Environment.TickCount64;
            var last = Interlocked.Read(ref _lastReport);
            return now - last >= IntervalMs
                && Interlocked.CompareExchange(ref _lastReport, now, last) == last;
        }
    }
}
