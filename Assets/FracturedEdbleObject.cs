using System.Linq;
using UnityEngine;

public class FracturedEdbleObject : MonoBehaviour
{

    public Collider mainCollider;

    public Rigidbody mainRigidBody;

    [Header("Fragements")]
    public GameObject[] fragments;

    public Rigidbody[] fragmentRigidBody;

    public Collider[] fragmentCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCollider = GetComponent<Collider>();
        mainRigidBody = GetComponent<Rigidbody>();

        fragments = GetComponentsInChildren<Transform>(includeInactive: true)
                        .Where(t => t != transform)          // skip the parent itself
                        .Select(t => t.gameObject)
                        .ToArray();

        fragmentRigidBody = GetComponentsInChildren<Rigidbody>(includeInactive: true)
                        .Where(rb => rb != mainRigidBody)          // skip the parent itself
                        .ToArray();


        fragmentCollider = GetComponentsInChildren<Collider>(includeInactive: true)
                        .Where(col => col != mainCollider)          // skip the parent itself
                        .ToArray();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
