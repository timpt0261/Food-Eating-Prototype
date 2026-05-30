using Mouth_Prototype.Utility;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TimedEffect", menuName = "FoodEffect/TimedEffect")]
public class TimeEffect : Effect
{
   
    public override void Initialize(FoodObject foodObject, UnityEvent activeEvent)
    {
        base.Initialize(foodObject, activeEvent);
        if (this._effectTimer != null)
            _effectTimer.OnTimerTick += OnEffectTick;
    }

    protected virtual void OnEffectTick(float progress)
    {
    }
}
