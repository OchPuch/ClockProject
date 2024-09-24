using System;
using System.Collections.Generic;
using Clock.ClockModules.Input;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Clock.ClockModules.CustomTime
{
    public class CustomTimeMediator : MonoBehaviour
    {
        [SerializeField] private CustomTimeController customTimeController;
        [Space(5)] 
        [SerializeField] private List<ClockInput> clockInputs;
        [SerializeField] private List<ClockInputView> clockInputViews;
        [Header("Events")]
        [SerializeField] private UnityEvent onStart;
        [SerializeField] private UnityEvent onAlarmModeEnded;
        [SerializeField] private UnityEvent onAlarmModeStarted;

        private ITimeProvider _timeProvider;

        [Inject]
        private void Construct(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }
        
        
        private void Start()
        {
            foreach (var clockInput in clockInputs)
            {
                clockInput.ValueChanged += OnValueChanged;
                clockInput.Switch(false);
            }

            foreach (var clockInputView in clockInputViews)
            {
                clockInputView.Hide();
            }
            
            customTimeController.AlarmModeStarted += OnCustomTimeModeStarted;
            customTimeController.AlarmModeEnded += OnCustomTimeModeEnded;
            onStart?.Invoke();
        }
        
        private void OnDestroy()
        {
            foreach (var clockInput in clockInputs)
            {
                clockInput.ValueChanged -= OnValueChanged;
            }
            customTimeController.AlarmModeStarted -= OnCustomTimeModeStarted;
            customTimeController.AlarmModeEnded -= OnCustomTimeModeEnded;
        }
        
        private void OnValueChanged(DateTime obj, ClockInput inputSource)
        {
            customTimeController.SetTime(obj.Hour, obj.Minute, obj.Second);
            foreach (var clockInput in clockInputs)
            {
                if (clockInput == inputSource) continue;
                clockInput.Sync(obj);
            }
        }
        
        private void OnCustomTimeModeEnded()
        {
            DisableInputs();
            onAlarmModeEnded?.Invoke();
        }

        private void OnCustomTimeModeStarted()
        {
            EnableInputs();
            onAlarmModeStarted?.Invoke();
        }

        private void DisableInputs()
        {
            foreach (var clockInput in clockInputs)
            {
                clockInput.Switch(false);
            }
            
            foreach (var clockInputView in clockInputViews)
            {
                clockInputView.Hide();
            }
        }
        
        private void EnableInputs()
        {
            foreach (var clockInput in clockInputs)
            {
                clockInput.Switch(true);
                clockInput.Sync(_timeProvider.GetTime());
            }
            
            foreach (var clockInputView in clockInputViews)
            {
                clockInputView.Show();
            }
        }
    }
}