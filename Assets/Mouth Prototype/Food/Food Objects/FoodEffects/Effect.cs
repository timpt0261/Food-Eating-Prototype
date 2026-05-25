using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Timer = Mouth_Prototype.Utility.Timer;

public abstract class Effect : ScriptableObject
{
    public enum FOOD_EFFECT_ACTIVE
    {
        GROUND,
        GRABBED,
        EATEN,
        DROPPED,
        AIR
    }
    protected bool _isActive = false;

    [Header("Material/Shaders")] [Tooltip("The material used to render the effect.")] [field: SerializeField]
    public Material materialEffects = null;
    
    
    [Header("Timer/Duration")] [feild: SerializeField]
    public bool isEffectTimed = true;

    [Range(0, 20.0f)] [field: SerializeField]
    public float effectDuration = 2.0f;

    protected Timer _effectTimer = null;

    [Header("Cooldown")] [Range(0, 20.0f)] public float effectCooldown = 0.5f;
    protected Timer _effectCoolDownTimer = null;

    [Header("State To Activate")] [feild: SerializeField]
    public FOOD_EFFECT_ACTIVE stateToActivateFoodEffect = FOOD_EFFECT_ACTIVE.GROUND;

    // External Game Objects
    protected HandMovement _player;
    protected FoodObject _foodObject;
    

    
    public virtual void Intialize(FoodObject foodObject, UnityEvent activeEvent)
    {
        if(foodObject == null) return;
        if(this._foodObject != null) return;
        this._foodObject = foodObject;
        
        AddMaterial(); // Update Food Object Render
        
        if (isEffectTimed)
        {
            this._effectTimer = new Timer(this.effectDuration);
            this._effectTimer.OnTimerEnd += Deactivate;
        }
        
        this._effectCoolDownTimer = new Timer(this.effectCooldown);
        this._isActive = false;
        
        activeEvent.AddListener(Activate);
        
    }

    public abstract void Activate();

    public abstract void Deactivate();
 

    public virtual void Tick(float deltaTime)
    {
        if (this._isActive && this.isEffectTimed)
        {
            this._effectTimer.Tick(deltaTime);
            Debug.Log("Progress: " + this._effectTimer.Progress );
        }

        
        
        if(this._effectCoolDownTimer.IsRunning) 
            this._effectCoolDownTimer.Tick(deltaTime);
    }

    protected virtual void AddMaterial()
    {
        if(this.materialEffects == null) return; // early edge case
        if(this._foodObject == null) return;
        
        Renderer renderer = this._foodObject.GetComponent<Renderer>();
        if(renderer == null) return;
        Material[] mat =renderer.materials;
        Material[] updatedMat = new Material[mat.Length + 1];
        for (int i = 0; i < updatedMat.Length; i++) updatedMat[i] = mat[i];
        mat[updatedMat.Length] = this.materialEffects;
        renderer.materials = mat;
        
    }

}



public enum FoodEffectRepeat
{
    DO_ONCE,
    PING_PONG,
    REPEAT
}