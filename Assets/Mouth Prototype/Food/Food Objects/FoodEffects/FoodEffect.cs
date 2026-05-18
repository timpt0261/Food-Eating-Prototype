using UnityEngine;
public abstract class FoodEffect : ScriptableObject
{
    [field: SerializeField] public bool stackable;
    [field: SerializeField] public Material material;
    
    protected FoodObject _foodObject;
    
    public void Intialize(FoodObject foodObject){
        this._foodObject = foodObject;
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