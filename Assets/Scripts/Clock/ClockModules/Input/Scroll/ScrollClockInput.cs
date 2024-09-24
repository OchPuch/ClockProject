using System;
using System.Linq;
using Scroll_Flow.Scripts;
using UnityEngine;


namespace Clock.ClockModules.Input.Scroll
{
    public class ScrollClockInput : ClockInput
    {
        [SerializeField] private ScrollMechanic hourScroll;
        [SerializeField] private ScrollMechanic minuteScroll;
        [SerializeField] private ScrollMechanic secondScroll;
        private void Start()
        {
            InitScrolls();
            Sync(TimeProvider.GetTime());
        }

        private void InitScrolls()
        {
            var sixtyNumbers = Enumerable.Range(0, 60).Select(n => $"{n:00}").ToList();
            var hourNumbers = Enumerable.Range(0, 24).Select(n => $"{n:00}").ToList();
            hourScroll.Initialize(hourNumbers, true);
            minuteScroll.Initialize(sixtyNumbers, true);
            secondScroll.Initialize(sixtyNumbers, true);
        }

        private void OnEnable()
        {
            hourScroll.ValueChanged += ChangeValue;
            minuteScroll.ValueChanged += ChangeValue;
            secondScroll.ValueChanged += ChangeValue;
        }
        
        private void OnDisable()
        {
            hourScroll.ValueChanged -= ChangeValue;
            minuteScroll.ValueChanged -= ChangeValue;
            secondScroll.ValueChanged -= ChangeValue;
        }

        private void ChangeValue(int _)
        {
            DateTime currentDateTime = TimeProvider.GetTime();

            int year = currentDateTime.Year;
            int month = currentDateTime.Month;
            int day = currentDateTime.Day;

            int second = secondScroll.GetCurrentValue();
            int minute = minuteScroll.GetCurrentValue();
            int hour = hourScroll.GetCurrentValue();

            var newTimeInput = new DateTime(year, month, day, hour, minute, second);
            if (LastTimeInput.TimeOfDay == newTimeInput.TimeOfDay) return;
            LastTimeInput = newTimeInput;
            OnValueChanged(LastTimeInput);
        }

        public override void Sync(DateTime time)
        {
            LastTimeInput = time;   
            hourScroll.SetCurrentValue(time.Hour);
            minuteScroll.SetCurrentValue(time.Minute);
            secondScroll.SetCurrentValue(time.Second);
        }

        protected override void OnSwitch(bool value)
        {
            hourScroll.enabled = value;
            minuteScroll.enabled = value;
            secondScroll.enabled = value;
        }
        
    }
}