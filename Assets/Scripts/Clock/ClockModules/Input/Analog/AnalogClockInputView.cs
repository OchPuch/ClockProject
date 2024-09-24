using Clock.ClockModules.View;
using DG.Tweening;
using UnityEngine;
using Utils;
using Zenject;

namespace Clock.ClockModules.Input.Analog
{
    public class AnalogClockInputView : ClockInputView
{
    [SerializeField] private ClockViewAnalog clockViewAnalog;
    [SerializeField] private ClockAnalogHandle hourHandle;
    [SerializeField] private ClockAnalogHandle minuteHandle;
    [SerializeField] private ClockAnalogHandle secondHandle;
    [Header("Animation settings")]
    [SerializeField] private float animationLength;

    private ITimeProvider _timeProvider;
    private Tween _hourRotationTween;
    private Tween _minuteRotationTween;
    private Tween _secondRotationTween;

    [Inject]
    private void Construct(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    
    public override void Show()
    {
    }
    
    public override void Hide()
    {
    }
    private void KillMultipleTween(params Tween[] tweenArray)
    {
        foreach (var tween in tweenArray )
        {
            if (tween != null && tween.IsActive())
            {
                tween.Kill();
            }
        }
    }

    protected override void OnSwitched(bool obj)
    {
        if (obj)
        {
            KillMultipleTween(_hourRotationTween, _minuteRotationTween, _secondRotationTween);
            clockViewAnalog.enabled = false;
        }
        else
        {
            InitTweens();
        }
    }

    private void InitTweens()
    {
        // Kill any existing tweens before starting new ones
        KillMultipleTween(_hourRotationTween, _minuteRotationTween, _secondRotationTween);

        // Disable clock view during animation
        clockViewAnalog.enabled = false;

        // Create a sequence
        Sequence sequence = DOTween.Sequence();

        // Add the hour tween
        _hourRotationTween = DOTween.To(() => hourHandle.GetCurrentValue(),
            x => hourHandle.SetRotation(x),
            TimeUtils.HoursToAngle(_timeProvider.GetTime().Hour, _timeProvider.GetTime().Minute),
            animationLength).SetEase(Ease.InOutSine);
        sequence.Join(_hourRotationTween);

        // Add the minute tween
        _minuteRotationTween = DOTween.To(() => minuteHandle.GetCurrentValue(),
            x => minuteHandle.SetRotation(x),
            TimeUtils.MinutesToAngle(_timeProvider.GetTime().Minute, _timeProvider.GetTime().Second),
            animationLength).SetEase(Ease.InOutSine);
        sequence.Join(_minuteRotationTween);

        // Add the second tween
        _secondRotationTween = DOTween.To(() => secondHandle.GetCurrentValue(),
            x => secondHandle.SetRotation(x),
            TimeUtils.SecondsToAngle(_timeProvider.GetTime().Second),
            animationLength).SetEase(Ease.InOutSine);
        sequence.Join(_secondRotationTween);

        // Add OnUpdate logic for each tween
        sequence.OnUpdate(() =>
        {
            _secondRotationTween.SetTarget(TimeUtils.SecondsToAngle(_timeProvider.GetTime().Second));
            _minuteRotationTween.SetTarget(TimeUtils.MinutesToAngle(_timeProvider.GetTime().Minute, _timeProvider.GetTime().Second));
            _hourRotationTween.SetTarget(TimeUtils.HoursToAngle(_timeProvider.GetTime().Hour, _timeProvider.GetTime().Minute));
        });

        // Add completion logic for the entire sequence
        sequence.OnComplete(() =>
        {
            clockViewAnalog.enabled = true;
            // Additional completion logic if needed
        });
    }
    }
}
