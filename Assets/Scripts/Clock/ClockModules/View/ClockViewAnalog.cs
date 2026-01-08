using System;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Zenject;

namespace Clock.ClockModules.View
{
    public class ClockViewAnalog : MonoBehaviour
    {
        [FormerlySerializedAs("hourClockHand")] [SerializeField] private Transform _hourClockHand;
        [FormerlySerializedAs("minuteClockHand")] [SerializeField] private Transform _minuteClockHand;
        [FormerlySerializedAs("secondClockHand")] [SerializeField] private Transform _secondClockHand;
        
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

        private void SetHours(int hours, int minutes = 0)
        {
            float hoursAngle = TimeUtils.HoursToAngle(hours, minutes);
            _hourClockHand.localRotation = Quaternion.Euler(0f, 0f, hoursAngle);
        }

        private void SetMinutes(int minutes, int seconds = 0)
        {
            float minutesAngle = TimeUtils.MinutesToAngle(minutes, seconds);
            _minuteClockHand.localRotation = Quaternion.Euler(0f, 0f, minutesAngle);
        }

        private void SetSeconds(int seconds)
        {
            float secondsAngle = TimeUtils.SecondsToAngle(seconds);
            _secondClockHand.localRotation = Quaternion.Euler(0f, 0f, secondsAngle);
        }
    }
}
