using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Clock
{
    public class ClockService : ITimeProvider, ITickable
    {
        public TimeSpan TimeOffset { get; set; } = new(0);
        private DateTime _currentTime = DateTime.Now;
        private CancellationTokenSource _timeUpdaterCancellationTokenSource;
        public event Action<DateTime> TimeUpdated;

        public void ResetTimeOffset()
        {
            TimeOffset = new TimeSpan(0);
        }
        
        public DateTime GetTime()
        {
            return _currentTime + TimeOffset;
        }

        public DateTime GetTimeWithoutOffset()
        {
            return _currentTime;
        }

        public void CancelTimeUpdater()
        {
            if (_timeUpdaterCancellationTokenSource != null)
            {
                _timeUpdaterCancellationTokenSource.Cancel();
                _timeUpdaterCancellationTokenSource.Dispose();
                _timeUpdaterCancellationTokenSource = null;
            }
        }

        public async void SetInternetRetrieveTimeUpdater(int minutes, GameObject context)
        {
            if (minutes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes), minutes, "dont set time updater to more then once a minute");
            }
            
            CancelTimeUpdater();
            
            _timeUpdaterCancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _timeUpdaterCancellationTokenSource.Token;
            
            try
            {
                while (!cancellationToken.IsCancellationRequested && context)
                {
                    RetrieveInternetTime();
                    await Task.Delay(TimeSpan.FromMinutes(minutes), cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                Debug.Log("Time retrieving is canceled");
            }
        }

        public void Tick()
        {
            _currentTime = _currentTime.AddSeconds(Time.unscaledDeltaTime);
            UpdateTime();
        }

        private async void RetrieveInternetTime()
        {
            _currentTime = await WebTimeProvider.GetTimeAsync();
            Debug.Log($"Retrieved time{_currentTime} and system time is {DateTime.Now}");
            UpdateTime();
        }

        private void UpdateTime()
        {
            TimeUpdated?.Invoke(_currentTime + TimeOffset);
        }
    }
}