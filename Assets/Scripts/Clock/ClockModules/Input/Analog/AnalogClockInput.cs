using System;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Clock.ClockModules.Input.Analog
{
    public class AnalogClockInput : ClockInput
    {
        [FormerlySerializedAs("hourAnalogHandle")] [SerializeField] private ClockAnalogHandle _hourAnalogHandle;
        [FormerlySerializedAs("minuteAnalogHandle")] [SerializeField] private ClockAnalogHandle _minuteAnalogHandle;
        [FormerlySerializedAs("secondAnalogHandle")] [SerializeField] private ClockAnalogHandle _secondAnalogHandle;

        private const int HourLoopBonus = 12;
        private bool _hourLoopWasMade;
        
        private void Start()
        {
            Sync(TimeProvider.GetTime());
        }

        private void OnEnable()
        {
            _hourAnalogHandle.ValueChanged += ChangeValue;
            _minuteAnalogHandle.ValueChanged += ChangeValue;
            _secondAnalogHandle.ValueChanged += ChangeValue;
        }

        private void OnDisable()
        {
            _hourAnalogHandle.ValueChanged -= ChangeValue;
            _minuteAnalogHandle.ValueChanged -= ChangeValue;
            _secondAnalogHandle.ValueChanged -= ChangeValue;
        }

        private void ChangeValue(float _)
        {
            DateTime currentDateTime = TimeProvider.GetTime();

            int year = currentDateTime.Year;
            int month = currentDateTime.Month;
            int day = currentDateTime.Day;

            float hourAngle = _hourAnalogHandle.GetCurrentValue();
            float minuteAngle = _minuteAnalogHandle.GetCurrentValue();
            float secondAngle = _secondAnalogHandle.GetCurrentValue();
            
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
            
            UpdateRotation(time);
        }

        private void UpdateRotation(DateTime time)
        {
            _hourAnalogHandle.SetRotation(TimeUtils.HoursToAngle(time.Hour));
            _minuteAnalogHandle.SetRotation(TimeUtils.MinutesToAngle(time.Minute));
            _secondAnalogHandle.SetRotation(TimeUtils.SecondsToAngle(time.Second));
        }

        protected override void OnSwitch(bool value)
        {
            _hourAnalogHandle.enabled = value;
            _minuteAnalogHandle.enabled = value;
            _secondAnalogHandle.enabled = value;
        }
    }
}