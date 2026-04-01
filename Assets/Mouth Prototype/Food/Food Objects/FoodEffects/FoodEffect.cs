using UnityEngine;
public abstract class FoodEffect : ScriptableObject
{
    [field: SerializeField] public FoodEffectActivation activation = FoodEffectActivation.ONDROPPED;
    [field: SerializeField] public bool stackable;
 
}

public enum FoodEffectActivation
{
    ONGRAB,
    ONEATEN,
    ONDROPPED
}

public enum FoodEffectRepeat
{
    DO_ONCE,
    PING_PONG,
    REPEAT
}