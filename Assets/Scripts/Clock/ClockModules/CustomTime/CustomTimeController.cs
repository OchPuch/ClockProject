using System;
using UnityEngine;
using Zenject;

namespace Clock.ClockModules.CustomTime
{
    public class CustomTimeController : MonoBehaviour
    {
        private DateTime _customTime;
        private ClockService _clockService;
        private bool IsInAlarmMode { get; set; }
        public event Action AlarmModeStarted;
        public event Action AlarmModeEnded;

        [Inject]
        public void Construct(ClockService clockService)
        {
            _clockService = clockService;
        }
        
        public void StartAlarmMode()
        {
            if (IsInAlarmMode) return;
            IsInAlarmMode = true;
            
            AlarmModeStarted?.Invoke();
        }

        private void CancelAlarmMode()
        {
            if (!IsInAlarmMode) return;
            IsInAlarmMode = false;

            AlarmModeEnded?.Invoke();
        }

        public void SetTime(int hour, int minute, int second)
        {
            DateTime currentDateTime = _clockService.GetTime();

            int year = currentDateTime.Year;
            int month = currentDateTime.Month;
            int day = currentDateTime.Day;
            
            DateTime newDateTime = new DateTime(year, month, day, hour, minute, second);
            
            _customTime = newDateTime;
        }

        public void DiscardCustomTimeAndStop()
        {
            if (!IsInAlarmMode) return;
            _clockService.TimeOffset = new TimeSpan(0);
            CancelAlarmMode();
        }
        
        public void SetCustomTimeAndStop()
        {
            if (!IsInAlarmMode) return;
            _clockService.TimeOffset = _customTime - _clockService.GetTimeWithoutOffset();
            CancelAlarmMode();
        }
    }
}