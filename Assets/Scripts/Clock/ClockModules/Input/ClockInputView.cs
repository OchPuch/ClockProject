using System;
using UnityEngine;

namespace Clock.ClockModules.Input
{
    public abstract class ClockInputView : MonoBehaviour
    {
        [field: SerializeField] protected ClockInput ClockInput { get; private set; }

        private void Awake()
        {
            ClockInput.ValueChanged += OnValueChanged;
            ClockInput.Switched += OnSwitched;
        }
        
        private void OnDestroy()
        {
            ClockInput.ValueChanged -= OnValueChanged;
            ClockInput.Switched -= OnSwitched;
        }

        public abstract void Hide();

        public abstract void Show();


        protected virtual void OnValueChanged(DateTime obj, ClockInput clockInput)
        {
            
        }
        
        protected virtual void OnSwitched(bool obj)
        {
            
        }
    }
}