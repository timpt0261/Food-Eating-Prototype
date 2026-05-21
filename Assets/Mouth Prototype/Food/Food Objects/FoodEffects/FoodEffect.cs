using UnityEngine;
using UnityEngine.Events;

public abstract class FoodEffect : ScriptableObject
{
    public enum FOOD_EFFECT_ACTIVE { GROUND, GRABBED, EATEN, DROPPED, AIR }
    public enum FOOD_EFFECT_DURATION { TIMER, EVENT, INDEFINITE }

    [field: SerializeField] public bool stackable;
    [field: SerializeField] public Material material;
    [field: SerializeField] public float duration;
    
    
    [feild: SerializeField] public FOOD_EFFECT_ACTIVE effectActive =  FOOD_EFFECT_ACTIVE.GROUND;
    [feild: SerializeField] public FOOD_EFFECT_DURATION durationEffect = FOOD_EFFECT_DURATION. INDEFINITE;
    
    protected FoodObject _foodObject;
    protected int EffectCount = 0;

    public virtual void Intialize(FoodObject foodObject, UnityEvent @event){
        this._foodObject = foodObject;
        @event.AddListener(() => { EffectCount++; Activate(); });
    }
    
    abstract public void Activate();
    abstract public void Deactivate();
    
    
}



public enum FoodEffectRepeat
{
    DO_ONCE,
    PING_PONG,
    REPEAT
}