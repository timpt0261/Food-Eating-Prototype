using System.Collections.Generic;
using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
//  EffectProcessor
//
//  Sits on the same GameObject as FoodObject (enforced via RequireComponent).
//  Owns the full lifecycle of every Effect defined in FoodStats:
//
//    Start        — initializes all effects, resolves player reference
//    Update       — ticks every active effect's timers each frame
//    OnCollision  — passes collision targets to COLLISION effects
//    OnDestroy    — cleans up all effects when the food object dies
//
//  Non-stackable replacement policy:
//    If an effect with IsStackable = false tries to activate while an
//    instance of the same effectId is already active, the existing one
//    is deactivated first, then the incoming one activates fresh.
//
//  Pending queue:
//    Effects that cannot activate immediately (e.g. same effectId is active
//    and non-stackable, but repeatBehavior is REPEAT) are held in a pending
//    queue and promoted once the active effect deactivates.
// ─────────────────────────────────────────────────────────────────────────────
[RequireComponent(typeof(FoodObject))]
public class EffectProcessor : MonoBehaviour
{
    // ── References ────────────────────────────────────────────────────────────

    private FoodObject    _foodObject;
    private HandMovement  _player;

    // ── Effect lists ──────────────────────────────────────────────────────────

    // All effects registered from FoodStats — the full definition set.
    private readonly List<Effect> _allEffects = new List<Effect>();

    // Effects currently running (IsActive == true).
    private readonly List<Effect> _activeEffects = new List<Effect>();

    // Effects waiting to activate once a blocking active effect deactivates.
    private readonly Queue<Effect> _pendingEffects = new Queue<Effect>();

    // Last FoodObject collision target, used by COLLISION effects.
    private FoodObject _lastCollisionTarget = null;


    // ── Unity lifecycle ───────────────────────────────────────────────────────

    private void Start()
    {
        _foodObject = GetComponent<FoodObject>();
        _player     = FindObjectOfType<HandMovement>();

        RegisterEffectsFromStats();
    }

    private void Update()
    {
        TickActiveEffects();
        FlushExpiredActives();
        PromotePending();
    }

    private void OnDestroy()
    {
        foreach (Effect effect in _allEffects)
            effect.CleanUp();

        _allEffects.Clear();
        _activeEffects.Clear();
        _pendingEffects.Clear();
    }


    // ── Registration ──────────────────────────────────────────────────────────

    private void RegisterEffectsFromStats()
    {
        if (_foodObject.Stats == null) return;
        if (_foodObject.Stats.effects == null) return;

        foreach (Effect effect in _foodObject.Stats.effects)
        {
            if (effect == null) continue;

            // ScriptableObjects are shared assets — clone each one so multiple
            // FoodObjects using the same FoodStats don't share runtime state.
            Effect instance = Instantiate(effect);

            UnityEngine.Events.UnityEvent triggerEvent = ResolveEventForState(instance.stateToActivateFoodEffect);
            if (triggerEvent == null) continue;

            instance.Initialize(_foodObject, triggerEvent);
            instance.SetPlayer(_player);

            _allEffects.Add(instance);
        }

        // Wire a processor-level listener on each trigger event so
        // RequestActivation intercepts every state-triggered activation attempt.
        foreach (Effect instance in _allEffects)
        {
            Effect captured = instance; // capture for lambda closure
            UnityEngine.Events.UnityEvent triggerEvent = ResolveEventForState(captured.stateToActivateFoodEffect);
            triggerEvent?.AddListener(() => RequestActivation(captured));
        }
    }

    private UnityEngine.Events.UnityEvent ResolveEventForState(Effect.FoodEffectActive state)
    {
        switch (state)
        {
            case Effect.FoodEffectActive.Ground:   return _foodObject.OnGrounded;
            case Effect.FoodEffectActive.Grabbed:  return _foodObject.OnGrab;
            case Effect.FoodEffectActive.Eaten:    return _foodObject.OnEaten;
            case Effect.FoodEffectActive.Dropped:  return _foodObject.OnDropped;
            case Effect.FoodEffectActive.Air:      return _foodObject.OnAir;
            default:                                 return _foodObject.OnGrounded;
        }
    }


    // ── Activation control ────────────────────────────────────────────────────

    // Called by the trigger event listener for every effect.
    // Decides whether to activate immediately, replace, or queue.
    private void RequestActivation(Effect incoming)
    {
        // Resolve and assign the collision target before any activation attempt.
        if (incoming.target == Effect.FoodEffectTarget.Collision)
            SetCollisionTargetOnEffect(incoming, _lastCollisionTarget);

        if (incoming.IsStackable)
        {
            // Stackable — always activate directly.
            ActivateEffect(incoming);
            return;
        }

        // Non-stackable — check if an instance with the same effectId is active.
        Effect blocking = FindActiveById(incoming.EffectID);

        if (blocking == null)
        {
            // Nothing blocking — activate immediately.
            ActivateEffect(incoming);
            return;
        }

        // Replacement policy: deactivate the existing one, then activate the new one.
        DeactivateEffect(blocking);
        ActivateEffect(incoming);
    }

    private void ActivateEffect(Effect effect)
    {
        effect.Activate();

        if (effect.IsActive && !_activeEffects.Contains(effect))
            _activeEffects.Add(effect);
    }

    private void DeactivateEffect(Effect effect)
    {
        effect.Deactivate();
        _activeEffects.Remove(effect);

        // After deactivation, give the pending queue a chance to promote.
        PromotePending();
    }


    // ── Per-frame work ────────────────────────────────────────────────────────

    private void TickActiveEffects()
    {
        for (int i = 0; i < _activeEffects.Count; i++)
            _activeEffects[i].Tick(Time.deltaTime);
    }

    // Remove effects that deactivated themselves (e.g. timer expired).
    private void FlushExpiredActives()
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            if (!_activeEffects[i].IsActive)
                _activeEffects.RemoveAt(i);
        }
    }

    // Promote the front of the pending queue if nothing is blocking it.
    private void PromotePending()
    {
        while (_pendingEffects.Count > 0)
        {
            Effect next = _pendingEffects.Peek();

            if (!next.IsStackable && FindActiveById(next.EffectID) != null)
                break; // still blocked — leave in queue

            _pendingEffects.Dequeue();
            ActivateEffect(next);
        }
    }


    // ── Collision handling ────────────────────────────────────────────────────

    private void OnCollisionEnter(Collision collision)
    {
        FoodObject other = collision.gameObject.GetComponent<FoodObject>();
        if (other == null) return;

        _lastCollisionTarget = other;

        // Notify all COLLISION effects that are already active so they can
        // redirect to the new target if needed.
        foreach (Effect effect in _activeEffects)
        {
            if (effect.target == Effect.FoodEffectTarget.Collision)
                SetCollisionTargetOnEffect(effect, other);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        FoodObject other = collision.gameObject.GetComponent<FoodObject>();
        if (other == null) return;

        if (_lastCollisionTarget == other)
            _lastCollisionTarget = null;
    }

    // Routes the collision target to the correct intermediate class.
    // FieldEffect uses Physics.OverlapSphere so it does not need this.
    private void SetCollisionTargetOnEffect(Effect effect, FoodObject target)
    {
        // if (effect is ScaleEffect scaleEffect)
        //     scaleEffect.SetCollisionTarget(target);
        // else if (effect is ObjectMutationEffect mutationEffect)
        //     mutationEffect.SetCollisionTarget(target);
    }


    // ── Helpers ───────────────────────────────────────────────────────────────

    private Effect FindActiveById(int id)
    {
        for (int i = 0; i < _activeEffects.Count; i++)
        {
            if (_activeEffects[i].EffectID == id)
                return _activeEffects[i];
        }
        return null;
    }


    // ── Public API ────────────────────────────────────────────────────────────

    // Force-deactivate all active effects. Useful when the food is destroyed
    // mid-effect or a game rule clears all status conditions.
    public void ClearAllEffects()
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
            _activeEffects[i].Deactivate();

        _activeEffects.Clear();
        _pendingEffects.Clear();
    }

    // Force-deactivate a specific effect by effectID.
    public void ClearEffectById(int effectId)
    {
        Effect target = FindActiveById(effectId);
        if (target != null)
            DeactivateEffect(target);
    }

    // Manually trigger an effect by effectID regardless of FoodObject state.
    // Used by AllergyEffect to chain sub-effects directly.
    public void TriggerEffectById(int effectId)
    {
        Effect effect = _allEffects.Find(e => e.EffectID== effectId);
        if (effect != null)
            RequestActivation(effect);
    }

    // Returns true if an effect with the given effectID is currently active.
    // Used by AllergyEffect to avoid double-triggering sub-effects.
    public bool IsEffectActiveById(int effectId) => FindActiveById(effectId) != null;
}