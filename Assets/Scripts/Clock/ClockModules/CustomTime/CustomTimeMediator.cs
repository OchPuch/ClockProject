using System;
using System.Collections.Generic;
using Clock.ClockModules.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Zenject;

namespace Clock.ClockModules.CustomTime
{
    public class CustomTimeMediator : MonoBehaviour
    {
        [FormerlySerializedAs("customTimeController")] [SerializeField]
        private CustomTimeController _customTimeController;

        [FormerlySerializedAs("clockInputs")] [Space(5)] [SerializeField]
        private List<ClockInput> _clockInputs;

        [FormerlySerializedAs("clockInputViews")] [SerializeField]
        private List<ClockInputView> _clockInputViews;

        [FormerlySerializedAs("onStart")] [Header("Events")] [SerializeField]
        private UnityEvent _onStart;

        [FormerlySerializedAs("onAlarmModeEnded")] [SerializeField]
        private UnityEvent _onAlarmModeEnded;

        [FormerlySerializedAs("onAlarmModeStarted")] [SerializeField]
        private UnityEvent _onAlarmModeStarted;

        private ITimeProvider _timeProvider;

        [Inject]
        private void Construct(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }


        private void Start()
        {
            foreach (var clockInput in _clockInputs)
            {
                clockInput.ValueChanged += OnValueChanged;
                clockInput.Switch(false);
            }

            foreach (var clockInputView in _clockInputViews)
            {
                clockInputView.Hide();
            }

            _customTimeController.AlarmModeStarted += OnCustomTimeModeStarted;
            _customTimeController.AlarmModeEnded += OnCustomTimeModeEnded;
            _onStart?.Invoke();
        }

        private void OnDestroy()
        {
            foreach (var clockInput in _clockInputs)
            {
                clockInput.ValueChanged -= OnValueChanged;
            }

            _customTimeController.AlarmModeStarted -= OnCustomTimeModeStarted;
            _customTimeController.AlarmModeEnded -= OnCustomTimeModeEnded;
        }

        private void OnValueChanged(DateTime obj, ClockInput inputSource)
        {
            _customTimeController.SetTime(obj.Hour, obj.Minute, obj.Second);
            foreach (var clockInput in _clockInputs)
            {
                if (clockInput == inputSource) continue;
                clockInput.Sync(obj);
            }
        }

        private void OnCustomTimeModeEnded()
        {
            DisableInputs();
            _onAlarmModeEnded?.Invoke();
        }

        private void OnCustomTimeModeStarted()
        {
            EnableInputs();
            _onAlarmModeStarted?.Invoke();
        }

        private void DisableInputs()
        {
            foreach (var clockInput in _clockInputs)
            {
                clockInput.Switch(false);
            }

            foreach (var clockInputView in _clockInputViews)
            {
                clockInputView.Hide();
            }
        }

        private void EnableInputs()
        {
            foreach (var clockInput in _clockInputs)
            {
                clockInput.Switch(true);
                clockInput.Sync(_timeProvider.GetTime());
            }

            foreach (var clockInputView in _clockInputViews)
            {
                clockInputView.Show();
            }
        }
    }
}