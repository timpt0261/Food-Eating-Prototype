using DG.Tweening;
using DG.Tweening.Core.Easing;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "Emit", menuName = "FoodEffect/Emit")]
public class Emit : FoodEffect
{
    // do once, ping-pong, repeat
    [field: SerializeField] public int repeatCounter = 1;
    [field: SerializeField] public FoodEffectRepeat repeatEffect = FoodEffectRepeat.DO_ONCE;
    [feild:SerializeField] public GameObject emitPrefab;

    [field: SerializeField] public float startSize;
    [field: SerializeField] public Color startColor;

}