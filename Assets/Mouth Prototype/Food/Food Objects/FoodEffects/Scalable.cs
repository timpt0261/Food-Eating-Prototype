using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "Scalable", menuName = "FoodEffect/Scalable")]
public class Scalable : Effect
{
    // do once, ping-pong, repeat
    // [field: SerializeField] public int repeatCounter = 1;
    // [field: SerializeField] public FoodEffectRepeat repeatEffect = FoodEffectRepeat.DO_ONCE;

    [Header("Scale Factors")]
    [Tooltip(" Scale Curve value determines actual scale")]
    [Range(0.1f, 3.5f)]
    [field: SerializeField]
    public float targetScale = 2f;

    [Header("Start Scale")] [Tooltip("The Speed of the Scale ")] [field: SerializeField]
    public float startSpeed = 1.5f;

    [field: SerializeField] public Ease startScaleEase = Ease.Linear;

    private Vector3 _startingScale;
    
    [Header("End Scale Scale")]
    [Tooltip("The Speed of the Scale ")]
    [field: SerializeField] public float endSpeed = 1.5f;
    
    [field: SerializeField] public Ease endScaleEase = Ease.Linear;
    
    [Header("Rigidbody")]
    
    private Rigidbody _rbFoodObject;
    [field:SerializeField] public bool isMassChanged = true;
    
    [field: SerializeField] public bool direction = true;

    public override void Intialize(FoodObject foodObject, UnityEvent activeEvent)
    {
        base.Intialize(foodObject, activeEvent);
        _rbFoodObject = foodObject.GetComponent<Rigidbody>();
    }

    public override void Activate()
    {
        if (!this._foodObject) return;
        if (this._isActive) return;
        if (this._effectCoolDownTimer.IsRunning) return;
        
        this._isActive = true;

        if (this.isEffectTimed)
        {
            this._effectTimer.Reset();
            this._effectTimer.Start();
        }

        this._startingScale = this._foodObject.transform.localScale;

        if (this.isMassChanged)
        {
            this._foodObject.Rigidbody.mass = direction
                ? this._foodObject.Rigidbody.mass * targetScale
                : this._foodObject.Rigidbody.mass / targetScale;
        }

        
        this._foodObject.transform.DOScale(targetScale, this.startSpeed).SetEase(this.startScaleEase);
        
    }

    public override void Deactivate()
    {
        if (!this._isActive) return;
        this._isActive = false;

        if (this.isEffectTimed)
            this._effectTimer.Stop();

        if (this.isMassChanged)
        {
            this._foodObject.Rigidbody.mass = direction
                ? this._foodObject.Rigidbody.mass / targetScale
                : this._foodObject.Rigidbody.mass * targetScale;
        }

        this._foodObject.transform.DOScale(_startingScale, this.endSpeed).SetEase(this.endScaleEase);

        this._effectCoolDownTimer.Reset();
        this._effectCoolDownTimer.Start();
        this._isActive = false;
    }

    


}

