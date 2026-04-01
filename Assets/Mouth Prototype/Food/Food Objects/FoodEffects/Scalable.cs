using DG.Tweening;
using DG.Tweening.Core.Easing;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "Scalable", menuName = "FoodEffect/Scalable")]
public class Scalable : FoodEffect
{
    // do once, ping-pong, repeat
    [field: SerializeField] public int repeatCounter = 1;
    [field: SerializeField] public FoodEffectRepeat repeatEffect = FoodEffectRepeat.DO_ONCE;
    [field: SerializeField] public Ease scaleEase = Ease.Linear;
    [field: SerializeField] public AnimationCurve scaleCurve = AnimationCurve.Linear(0, 0.1f, 1, 1);

}

