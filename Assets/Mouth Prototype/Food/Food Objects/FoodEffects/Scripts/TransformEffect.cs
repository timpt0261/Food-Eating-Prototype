using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "TransformEffect", menuName = "FoodEffect/TimedEffect/TransformEffect")]
public class TransformEffect : TimeEffect
{

    [Header("Scale Factors")]
    [Tooltip(" Scale Curve value etermines actual scale")]
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
    public override void Activate()
    {
        if (!this._foodObject) return;
        if (this._isActive) return;
        if (this._coolDownTimer.IsRunning) return;
        
        base.Activate();
        
        this._startingScale = this._foodObject.transform.localScale;

        // if (this.isMassChanged)
        // {
        //     this._foodObject.Rigidbody.mass = isMassChangeInverse 
        //         ? this._foodObject.Rigidbody.mass * targetScale
        //         : this._foodObject.Rigidbody.mass / targetScale;
        // }

        
        this._foodObject.transform.DOScale(targetScale, this.startSpeed).SetEase(this.startScaleEase);
        
    }

    public override void Deactivate()
    {
        if (!this._isActive) return;
        base.Deactivate();
          

        // if (this.isMassChanged)
        // {
        //     this._foodObject.Rigidbody.mass = isMassChangeInverse 
        //         ? this._foodObject.Rigidbody.mass / targetScale
        //         : this._foodObject.Rigidbody.mass * targetScale;
        // }

        this._foodObject.transform.DOScale(_startingScale, this.endSpeed).SetEase(this.endScaleEase);

       
    }

    


}

