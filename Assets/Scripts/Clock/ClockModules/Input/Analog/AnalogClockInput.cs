using System;
using UnityEngine;
using Utils;

namespace Clock.ClockModules.Input.Analog
{
    public class AnalogClockInput : ClockInput
    {
        [SerializeField] private ClockAnalogHandle hourAnalogHandle;
        [SerializeField] private ClockAnalogHandle minuteAnalogHandle;
        [SerializeField] private ClockAnalogHandle secondAnalogHandle;

        private const int HourLoopBonus = 12;
        private bool _hourLoopWasMade;
        
        private void Start()
        {
            Sync(TimeProvider.GetTime());
        }

        private void OnEnable()
        {
            hourAnalogHandle.ValueChanged += ChangeValue;
            minuteAnalogHandle.ValueChanged += ChangeValue;
            secondAnalogHandle.ValueChanged += ChangeValue;
        }

        private void OnDisable()
        {
            hourAnalogHandle.ValueChanged -= ChangeValue;
            minuteAnalogHandle.ValueChanged -= ChangeValue;
            secondAnalogHandle.ValueChanged -= ChangeValue;
        }

        private void ChangeValue(float _)
        {
            DateTime currentDateTime = TimeProvider.GetTime();

            int year = currentDateTime.Year;
            int month = currentDateTime.Month;
            int day = currentDateTime.Day;

            float hourAngle = hourAnalogHandle.GetCurrentValue();
            float minuteAngle = minuteAnalogHandle.GetCurrentValue();
            float secondAngle = secondAnalogHandle.GetCurrentValue();
            
            int second = TimeUtils.AngleToSeconds(secondAngle);
            int minute = TimeUtils.AngleToMinutes(minuteAngle);
            int hour = TimeUtils.AngleToHours(hourAngle);
            ApplyHourLoopBonus(ref hour);
            var newTimeInput = new DateTime(year, month, day, hour, minute, second);
            if (LastTimeInput.TimeOfDay == newTimeInput.TimeOfDay) return;
            LastTimeInput = newTimeInput;
            OnValueChanged(LastTimeInput);
        }

        private void ApplyHourLoopBonus(ref int hour)
        {
            var lastHour = LastTimeInput.Hour;
            
            if ((lastHour is >= 9 and < 12 or >= 21 && hour <= 3) //Checking if user scrolls the border from 0-11 to 12-23
                || (lastHour is >= 12 and < 18 or >= 0 and < 6 && hour is <= 11 and > 6)) //Checking if user scrolls same border but backwards
                _hourLoopWasMade = !_hourLoopWasMade;
            
            if (_hourLoopWasMade) hour += HourLoopBonus;
        }

        public override void Sync(DateTime time)
        {
            LastTimeInput = time;
            if (time.Hour >= 12)
            {
                _hourLoopWasMade = true;
            }
            hourAnalogHandle.SetRotation(TimeUtils.HoursToAngle(time.Hour));
            minuteAnalogHandle.SetRotation(TimeUtils.MinutesToAngle(time.Minute));
            secondAnalogHandle.SetRotation(TimeUtils.SecondsToAngle(time.Second));
        }

        protected override void OnSwitch(bool value)
        {
            hourAnalogHandle.enabled = value;
            minuteAnalogHandle.enabled = value;
            secondAnalogHandle.enabled = value;
        }
    }
}