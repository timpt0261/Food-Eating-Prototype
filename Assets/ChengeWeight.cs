using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ChengeWeight : MonoBehaviour
{

    Rigidbody rigidbody;

    public float A = 4.5f;
    public float B = 5f;
    public float C = 0;
    public float D = 5.5f;

    float x;
    void Start()
    {
        x = 0;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {

        float period = 2 * Mathf.PI / B;
        rigidbody.mass = A * Mathf.Cos(period * (x + C)) + D;
        x += 1;
    }

}
