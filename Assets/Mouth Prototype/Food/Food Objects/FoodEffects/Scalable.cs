using DG.Tweening;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "Scalable", menuName = "FoodEffect/Scalable")]
public class Scalable : FoodEffect
{
    // do once, ping-pong, repeat
    [field: SerializeField] public int repeatCounter = 1;
    [field: SerializeField] public FoodEffectRepeat repeatEffect = FoodEffectRepeat.DO_ONCE;
    [field: SerializeField] public Ease scaleEase = Ease.Linear;

    [Tooltip(" Scale Curve value determines actual scale")]
    [field: SerializeField] public AnimationCurve scaleCurve = AnimationCurve.Linear(0, 0.1f, 1, 1.0f);
   
    [Tooltip("duration of scale animation in seconds")]
    [field: SerializeField] public float scaleDuration = 1;
    
    private bool _isActive = false;
    public bool IsActive { get => _isActive; }


    public override void Intialize(FoodObject foodObject, UnityEvent @event)
    {
        base.Intialize(foodObject, @event);
        
    }

    public override void Activate()
    {
        if(this._foodObject == null) return;
        Debug.Log("Activating Scalable");
        _isActive = true;
        this.EffectCount++;
        float endValue = scaleCurve.Evaluate(10); // modifity to stats scale * multiplier/scalar
        this._foodObject.transform.DOScale( endValue, this.scaleDuration).SetEase(this.scaleEase);
        this._foodObject.Rigidbody.mass += endValue;
       
    }

    public override void Deactivate()
    {
        float startValue = scaleCurve.Evaluate(0);
        this._foodObject.transform.DOScale(startValue, this.scaleDuration).SetEase(this.scaleEase);
        _isActive = false;
        
    }
}
// keep track of current scale
