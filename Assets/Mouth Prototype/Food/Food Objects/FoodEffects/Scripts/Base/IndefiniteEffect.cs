using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "IndefiniteEffect", menuName = "FoodEffect/IndefiniteEffect")]
public class IndefiniteEffect : Effect
{
    public override void Initialize(FoodObject foodObject, UnityEvent activeEvent)
    {
        base.Initialize(foodObject, activeEvent);

    }

    public override void Tick(float deltaTime)
    {
        if(this._coolDownTimer.IsRunning) 
            this._coolDownTimer.Tick(deltaTime);
    }
}