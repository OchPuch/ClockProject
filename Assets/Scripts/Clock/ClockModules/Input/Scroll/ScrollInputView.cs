using DG.Tweening;
using Scroll_Flow.Scripts;
using UnityEngine;

namespace Clock.ClockModules.Input.Scroll
{
    public class ScrollInputView : ClockInputView
    {
        [SerializeField] private ScrollMechanic hourScroll;
        [SerializeField] private ScrollMechanic minuteScroll;
        [SerializeField] private ScrollMechanic secondScroll;
        [Header("Animation Settings")]
        [SerializeField] private float animationLength;
        [SerializeField] private Color hideColor;
        [SerializeField] private Color showColor;
        private Tween _colorAnimationTween;
        
        public override void Hide()
        {
            hourScroll.gameObject.SetActive(false);
            minuteScroll.gameObject.SetActive(false);
            secondScroll.gameObject.SetActive(false);
        }

        public override void Show()
        {
            hourScroll.gameObject.SetActive(true);
            minuteScroll.gameObject.SetActive(true);
            secondScroll.gameObject.SetActive(true);
        }

        protected override void OnSwitched(bool obj)
        {
            if (_colorAnimationTween != null && _colorAnimationTween.IsActive())
            {
                _colorAnimationTween.Kill();
            }

            _colorAnimationTween = obj ? ChangeColor(hideColor, showColor) : ChangeColor(showColor, hideColor);
        }
        
        private Tween ChangeColor(Color startColor, Color endColor)
        {
            hourScroll.Color = startColor;
            minuteScroll.Color = startColor;
            secondScroll.Color = startColor;

            return DOTween.To(() => hourScroll.Color, SetColor, endColor, animationLength);
        }
        
        private void SetColor(Color color)
        {
            hourScroll.Color = color;
            minuteScroll.Color = color;
            secondScroll.Color = color;
        }

        
    }
}