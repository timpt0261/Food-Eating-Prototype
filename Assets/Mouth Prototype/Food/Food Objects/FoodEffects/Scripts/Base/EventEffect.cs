using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EventEffect", menuName = "FoodEffect/EventEffect")]
public class EventEffect : Effect
{
    [field:SerializeField] protected UnityEvent _conditionEvent;
    
    protected virtual void InitializeWithCondition(FoodObject foodObject, UnityEvent activeEvent, UnityEvent conditionEvent)
    {
        base.Initialize(foodObject, activeEvent);
        
        this._conditionEvent = conditionEvent;
        this._conditionEvent.AddListener(this.Deactivate);
    }

    
}
