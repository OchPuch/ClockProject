using System;
using UnityEngine;
using Utils;
using Zenject;

namespace Clock.ClockModules.View
{
    public class ClockViewAnalog : MonoBehaviour
    {
        [SerializeField] private Transform hourClockHand;
        [SerializeField] private Transform minuteClockHand;
        [SerializeField] private Transform secondClockHand;
        
        private ITimeProvider _timeProvider;
        
        [Inject]
        public void Construct(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }
        
        private void OnEnable()
        {
            _timeProvider.TimeUpdated += SetClockHands;
        }

        private void OnDisable()
        {
            _timeProvider.TimeUpdated -= SetClockHands;
        }

        private void SetClockHands(DateTime dateTime)
        {
            int seconds = dateTime.Second;
            int minutes = dateTime.Minute;
            int hours = dateTime.Hour;
            
            SetHours(hours, minutes);
            SetMinutes(minutes, seconds);
            SetSeconds(seconds);
        }

        public void SetHours(int hours, int minutes = 0)
        {
            float hoursAngle = TimeUtils.HoursToAngle(hours, minutes);
            hourClockHand.localRotation = Quaternion.Euler(0f, 0f, hoursAngle);
        }

        public void SetMinutes(int minutes, int seconds = 0)
        {
            float minutesAngle = TimeUtils.MinutesToAngle(minutes, seconds);
            minuteClockHand.localRotation = Quaternion.Euler(0f, 0f, minutesAngle);

        }

        public void SetSeconds(int seconds)
        {
            float secondsAngle = TimeUtils.SecondsToAngle(seconds);
            secondClockHand.localRotation = Quaternion.Euler(0f, 0f, secondsAngle);
        }
    }
}
