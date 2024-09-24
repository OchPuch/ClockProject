using System;
using TMPro;
using UnityEngine;

namespace Clock.ClockModules.Input.Keyboard
{
    public class KeyboardClockInput : ClockInput
    {
        [SerializeField] private TMP_InputField inputField;

        private readonly int[] _dateTimeIntegers = new int[6];
        private int _currentInputIndex;
        private void IncrementCurrentInputIndex () =>  _currentInputIndex = (_currentInputIndex + 1) % _dateTimeIntegers.Length;
        
        private void Start()
        {
            inputField.onValidateInput += OnValidateInput;
        }

        protected override void OnSwitch(bool value)
        {
            inputField.enabled = value;
        }

        private char OnValidateInput(string text, int charIndex, char addedChar)
        {
            if (char.IsDigit(addedChar))
            {
                if (int.TryParse(addedChar.ToString(), out int number))
                {
                    _dateTimeIntegers[_currentInputIndex] = number; 
                    IncrementCurrentInputIndex();
                    ChangeValue();
                }
            }
            
            return '\0'; 
        }

        private void ChangeValue()
        {
            _dateTimeIntegers[0] = Mathf.Clamp(_dateTimeIntegers[0], 0, 2);
            _dateTimeIntegers[1] = Mathf.Clamp(_dateTimeIntegers[1], 0, 4);
            _dateTimeIntegers[2] = Mathf.Clamp(_dateTimeIntegers[2], 0, 5);
            _dateTimeIntegers[3] = Mathf.Clamp(_dateTimeIntegers[3], 0, 9);
            _dateTimeIntegers[4] = Mathf.Clamp(_dateTimeIntegers[4], 0, 5);
            _dateTimeIntegers[5] = Mathf.Clamp(_dateTimeIntegers[5], 0, 9);
            
            DateTime currentDateTime = TimeProvider.GetTime();

            int year = currentDateTime.Year;
            int month = currentDateTime.Month;
            int day = currentDateTime.Day;

            int second = _dateTimeIntegers[4] * 10 + _dateTimeIntegers[5];
            int minute = _dateTimeIntegers[2] * 10 + _dateTimeIntegers[3];
            int hour = _dateTimeIntegers[0] * 10 + _dateTimeIntegers[1];

            var newTimeInput = new DateTime(year, month, day, hour, minute, second);
            if (LastTimeInput.TimeOfDay == newTimeInput.TimeOfDay) return;
            LastTimeInput = newTimeInput;
            OnValueChanged(LastTimeInput);
        }

        public override void Sync(DateTime time)
        {
            _currentInputIndex = 0;
            _dateTimeIntegers[0] = time.Hour / 10;
            _dateTimeIntegers[1] = time.Hour % 10;
            _dateTimeIntegers[2] = time.Minute / 10;
            _dateTimeIntegers[3] = time.Minute % 10;
            _dateTimeIntegers[4] = time.Second / 10;
            _dateTimeIntegers[5] = time.Second % 10;
        }
        
        
    }
}