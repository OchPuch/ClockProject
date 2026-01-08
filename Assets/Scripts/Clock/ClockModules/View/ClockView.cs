using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Clock.ClockModules.View
{
    public class ClockView : MonoBehaviour
    {
        [FormerlySerializedAs("hourTextMeshProUGUI")] [SerializeField] private TextMeshProUGUI _hourTextMeshProUGUI;
        [FormerlySerializedAs("minuteTextMeshProUGUI")] [SerializeField] private TextMeshProUGUI _minuteTextMeshProUGUI;
        [FormerlySerializedAs("secondTextMeshProUGUI")] [SerializeField] private TextMeshProUGUI _secondTextMeshProUGUI;
        
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
            _hourTextMeshProUGUI.text = $"{dateTime.Hour:00}";
            _minuteTextMeshProUGUI.text = $"{dateTime.Minute:00}";
            _secondTextMeshProUGUI.text = $"{dateTime.Second:00}";
        }
    }
}