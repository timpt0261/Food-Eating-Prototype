using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Emitter", menuName = "FoodEffect/Emitter")]
public class Emitter : Effect
{
    [Header("Collider")]

    
    [field:SerializeField] private Collider[] colliders;
    [Range(0.5f, 3.0f)]
    [Tooltip("The radius of the emitter")]
    [field:SerializeField] private float colliderRadius = 0.5f;
    [field:SerializeField] private LayerMask colliderLayerMask;

    [field: SerializeField] private UnityEvent castEffect;
    public override void Activate()
    {
        DetectCollisonsWithinProximity();
    }

    private void DetectCollisonsWithinProximity()
    {
        colliders = Physics.OverlapSphere(this._foodObject.transform.position, this.colliderRadius,
            this.colliderLayerMask, QueryTriggerInteraction.Collide);
       
        if (colliders.Length <= 0) return;

        foreach (Collider collider in colliders)
        {
            GameObject go = collider.gameObject;
            // if player  apply effect to appropriate target (Hand, Camera)
            if(go.CompareTag("Player")) return;
            // if player  apply effect to appropriate target (Hand, Camera)
            if(go.CompareTag(this._foodObject.tag)) return;
        }
        
        
        // if other food object 
    }

    public override void Deactivate()
    {
        
    }
    
    
}
