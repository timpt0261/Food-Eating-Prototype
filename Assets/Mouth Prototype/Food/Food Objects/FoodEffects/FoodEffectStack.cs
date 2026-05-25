using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FoodEffectStack : MonoBehaviour
{ 
    
    private FoodObject _foodObject;
    private FoodStats  _foodStats;
    private List<Effect> _foodEffectsActive = new List<Effect>();

    private void RegisterFoodEffectsFromStats()
    {
        foreach (Effect effect in _foodStats.effects)
        {
            switch (effect.stateToActivateFoodEffect)
            {
                case Effect.FOOD_EFFECT_ACTIVE.GROUND:
                    effect.Intialize(this._foodObject, this._foodObject.onGrounded );
                    _foodEffectsActive.Add(effect);
                    break;
                case Effect.FOOD_EFFECT_ACTIVE.GRABBED:
                    effect.Intialize(this._foodObject, this._foodObject.onGrounded );
                    _foodEffectsActive.Add(effect);
                    break;
                case Effect.FOOD_EFFECT_ACTIVE.DROPPED:
                    effect.Intialize(this._foodObject, this._foodObject.onDropped );
                    _foodEffectsActive.Add(effect);
                    break;
                case Effect.FOOD_EFFECT_ACTIVE.AIR:
                    effect.Intialize(this._foodObject, this._foodObject.onAir );
                    _foodEffectsActive.Add(effect);
                    break;
            }
        }
    }

    private void HandleStateChange()
    {
    }

   
}
