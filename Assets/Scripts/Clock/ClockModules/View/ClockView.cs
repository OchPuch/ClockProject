using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Clock.ClockModules.View
{
    public class ClockView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI hourTextMeshProUGUI;
        [SerializeField] private TextMeshProUGUI minuteTextMeshProUGUI;
        [SerializeField] private TextMeshProUGUI secondTextMeshProUGUI;
        
        private ITimeProvider _timeProvider;
        
        [Inject]
        public void Construct(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }
        
        private void OnEnable()
        {
            _timeProvider.TimeUpdated += SetTimeText;
        }

        private void OnDisable()
        {
            _timeProvider.TimeUpdated -= SetTimeText;
        }

        public void SetTimeText(DateTime dateTime)
        {
            hourTextMeshProUGUI.text = $"{dateTime.Hour:00}";
            minuteTextMeshProUGUI.text = $"{dateTime.Minute:00}";
            secondTextMeshProUGUI.text = $"{dateTime.Second:00}";
        }
    }
}