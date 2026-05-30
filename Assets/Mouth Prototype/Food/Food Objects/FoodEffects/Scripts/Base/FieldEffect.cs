using System.Collections.Generic;
using Mouth_Prototype.Utility;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FieldEffect", menuName = "FoodEffect/FieldEffect")]
public class FieldEffect : TimeEffect
{
    
    [Header("Field")]
    [Range(0.1f, 3.5f)]
    public float fieldRadius;
    public float fieldStrength;

    [field: SerializeField] private bool _isPlayerAffected = false;
    
    private readonly List<FoodObject> _targetsInRange = new (10);

    public override void Activate()
    {
        base.Activate();
        if(!this._isActive) return;

        OnFieldStart();
    }

    public override void Deactivate()
    {
        foreach(FoodObject t in _targetsInRange)
            RemoveFromTarget(t);
        
        _targetsInRange.Clear();
        
        if(this._isPlayerAffected && this._player)
            RemoveFromPlayer(this._player);

        OnFieldEnd();
        base.Deactivate();
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        if(!this._isActive) return;

        GatherAndApply();
    }

    private void GatherAndApply()
    {
        var fresh = new List<FoodObject>();
        Collider[] results = new Collider[] { };
        int size = Physics.OverlapSphereNonAlloc(this._foodObject.transform.position, this.fieldRadius, results);

        for (int c = 0; c < size; c++)
        {
            results[c].TryGetComponent<FoodObject>( out FoodObject fo);
            if (fo == null || fo == this._foodObject) continue;
            if(this.TargetFoodID != -1 || fo.Stats.id != this.TargetFoodID) continue;
            fresh.Add(fo);
        }
        
        // Remove effect from targets that left the radius.
        foreach (FoodObject prev in _targetsInRange)
        {
            if (!fresh.Contains(prev))
                RemoveFromTarget(prev);
        }
 
        // Apply effect to targets that entered the radius.
        foreach (FoodObject next in fresh)
        {
            if (!_targetsInRange.Contains(next))
                ApplyToTarget(next);
        }
 
        _targetsInRange.Clear();
        _targetsInRange.AddRange(fresh);
 
        // Player sweep.
        if (this._isPlayerAffected && this._player != null)
        {
            float distToPlayer = Vector3.Distance(
                this._foodObject.transform.position,
                this._player.transform.position);
 
            if (distToPlayer <= fieldRadius)
                ApplyToPlayer(this._player);
            else
                RemoveFromPlayer(_player);
        }

    }


    protected virtual void OnFieldStart(){}
    
    
    protected virtual void OnFieldEnd() {}

    protected virtual void ApplyToTarget(FoodObject foodObject){}
    
    protected virtual void RemoveFromTarget(FoodObject foodObject){}

    protected virtual void ApplyToPlayer(HandMovement player){}
    
    protected void RemoveFromPlayer(HandMovement player){}

}
