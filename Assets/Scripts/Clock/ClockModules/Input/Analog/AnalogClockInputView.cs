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
    private Sequence _tweenSequence;

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
    

    protected override void OnSwitched(bool obj)
    {
        if (obj)
        { 
            if (_tweenSequence is not null) _tweenSequence.Kill();
            clockViewAnalog.enabled = false;
        }
        else
        {
            InitTweens();
        }
    }

    private void InitTweens()
    {
        if (_tweenSequence is not null) _tweenSequence.Kill();
        _tweenSequence = DOTween.Sequence();

        var hourRotationTween = DOTween.To(() => hourHandle.GetCurrentValue(),
            x => hourHandle.SetRotation(x),
            TimeUtils.HoursToAngle(_timeProvider.GetTime().Hour, _timeProvider.GetTime().Minute),
            animationLength).SetEase(Ease.InOutSine);
        _tweenSequence.Join(hourRotationTween);

        var minuteRotationTween = DOTween.To(() => minuteHandle.GetCurrentValue(),
            x => minuteHandle.SetRotation(x),
            TimeUtils.MinutesToAngle(_timeProvider.GetTime().Minute, _timeProvider.GetTime().Second),
            animationLength).SetEase(Ease.InOutSine);
        _tweenSequence.Join(minuteRotationTween);

        var secondRotationTween = DOTween.To(() => secondHandle.GetCurrentValue(),
            x => secondHandle.SetRotation(x),
            TimeUtils.SecondsToAngle(_timeProvider.GetTime().Second),
            animationLength).SetEase(Ease.InOutSine);
        _tweenSequence.Join(secondRotationTween);

        _tweenSequence.OnUpdate(() =>
        {
            clockViewAnalog.enabled = false;
            secondRotationTween.SetTarget(TimeUtils.SecondsToAngle(_timeProvider.GetTime().Second));
            minuteRotationTween.SetTarget(TimeUtils.MinutesToAngle(_timeProvider.GetTime().Minute, _timeProvider.GetTime().Second));
            hourRotationTween.SetTarget(TimeUtils.HoursToAngle(_timeProvider.GetTime().Hour, _timeProvider.GetTime().Minute));
        });

        _tweenSequence.OnComplete(() =>
        {
            clockViewAnalog.enabled = true;
        });
    }
    }
}
