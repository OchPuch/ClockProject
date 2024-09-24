using System;
using UnityEngine;
using Zenject;

namespace Clock.ClockModules.Input
{
    public abstract class ClockInput : MonoBehaviour
    {
        protected ITimeProvider TimeProvider;
        
        public DateTime LastTimeInput { get; protected set; }

        public event Action<DateTime, ClockInput> ValueChanged;
        public event Action<bool> Switched;
        
        [Inject]
        private void Construct(ITimeProvider timeProvider)
        {
            TimeProvider = timeProvider;
        }

        public abstract void Sync(DateTime time);
        public void Switch(bool value)
        {
            OnSwitch(value);
            Switched?.Invoke(value);
        }

        protected virtual void OnSwitch(bool value)
        {
            
        }
        
        protected virtual void OnValueChanged(DateTime obj)
        {
            ValueChanged?.Invoke(obj, this);
        }
    }
}