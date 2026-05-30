using UnityEngine;
using UnityEngine.Events;
using Timer = Mouth_Prototype.Utility.Timer;

public abstract class Effect : ScriptableObject
{
    public enum FoodEffectTarget
    {
        Self,
        Collision,
        Player,
        Field
    }

    public enum FoodEffectDuration
    {
        Timed,
        Event,
        Indefinite
    }

    public enum FoodEffectActive
    {
        Ground,
        Grabbed,
        Eaten,
        Dropped,
        Air
    }

    protected enum FoodEffectRepeat
    {
        DoOnce,
        PingPong,
        Repeat
    }

    [Header("EffectID")]
    [field: SerializeField] private protected int effectID; 
    public int EffectID => effectID;

    [field:SerializeField]private new string name = "";
    
    public string Name => name;

    [Header("Activation Trigger")] 
    public FoodEffectActive stateToActivateFoodEffect = FoodEffectActive.Ground;
    
    [Tooltip("Can it stack with other effects")] [field: SerializeField]
    protected bool isStackable = false;
    public bool IsStackable => isStackable;
    
    [Tooltip("Can Effect Repeat")] [field: SerializeField]
    protected FoodEffectRepeat repeat = FoodEffectRepeat.DoOnce;
    
    [Header("Duration")]
    [field:SerializeField] protected FoodEffectDuration duration = FoodEffectDuration.Timed;
    public FoodEffectDuration Duration => duration;
    
    
   
    [Range(0, 30.0f)] [field: SerializeField]
    protected float effectDuration = 2.0f;
    
    [Header("Cooldown")] [Range(0, 20.0f)] protected float effectCooldown = 0.5f;
    
    protected Timer _effectTimer = null;
    protected Timer _coolDownTimer = null;
    
    [Header("Target")]
    [field: SerializeField] public FoodEffectTarget target = FoodEffectTarget.Self;

    protected int TargetFoodID =-1;

    // Runtime State 
    protected bool _isActive = false;
    public bool IsActive => _isActive;
    protected int ActivationCount = 0;
    
    // External Game Objects
    protected HandMovement _player;
    protected FoodObject _foodObject;


    public virtual void Initialize(FoodObject foodObject, UnityEvent activeEvent)
    {
        if(foodObject == null) return;
        if(this._foodObject != null) return;
       
        this._foodObject = foodObject;
        
        if (this.duration == FoodEffectDuration.Timed)
        {
            this._effectTimer = new Timer(this.effectDuration);
            this._effectTimer.OnTimerEnd += Deactivate;
        }
        
        this._coolDownTimer = new Timer(this.effectCooldown);
        this._isActive = false;
        this.ActivationCount = 0;
        
        activeEvent.AddListener(StateEventFired);
    }
    
    public void SetPlayer(HandMovement player) => this._player = player;

    #region Core LifeCyle

    public virtual void Activate()
    {
        if(this._isActive) return;
        
        this._isActive = true;
        this.ActivationCount++;
        
        if(this.repeat == FoodEffectRepeat.DoOnce && this.ActivationCount > 1 )

        if (this.duration == FoodEffectDuration.Timed)
        {
            this._effectTimer.Reset();
            this._effectTimer.Start();
        }
    }

    public virtual void Deactivate()
    {
        if (this.duration == FoodEffectDuration.Timed)
        {
            this._effectTimer.Stop();// stop current effect timer
            
        }
        
        this._coolDownTimer.Reset(); // reset cooldown
        this._coolDownTimer.Start(); // start cooldown timer
        this._isActive = false;
    }

    public virtual void Tick(float deltaTime)
    {
        if (this._isActive)
        {
            this._effectTimer.Tick(deltaTime);
            Debug.Log("Progress: " + this._effectTimer.Progress );
        }
        
        if(this._coolDownTimer.IsRunning) 
            this._coolDownTimer.Tick(deltaTime);
    }

    public virtual void FixedTick(float deltaTime)
    {
    }

    #endregion

    public virtual void CleanUp()
    {
        if(this._effectTimer != null) this._effectTimer.Stop();
        if(this._coolDownTimer != null) this._coolDownTimer.Stop();
        
        this._isActive = false;
        this._foodObject = null;
        this._player     = null;

    }

    public virtual void StateEventFired()
    {
        this.ActivationCount++;
        this.Activate();
    }

    protected FoodObject ResolveCollisionTarget(FoodObject collisionTarget)
    {
        if (this.target != FoodEffectTarget.Collision) return null;
        if (collisionTarget == null) return null;
        if (this.TargetFoodID != -1 && collisionTarget.Stats.id != this.TargetFoodID) return null;
        return collisionTarget;
    }


}
