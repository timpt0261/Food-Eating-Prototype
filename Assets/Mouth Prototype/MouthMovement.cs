using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class MouthMovement : MonoBehaviour
{
    // --------------------
    // Input Settings
    // --------------------
    [SerializeField] private KeyCode key = KeyCode.Q;

    // --------------------
    // Rigidbody References
    // --------------------
    [SerializeField] private Rigidbody rb_roof;
    [SerializeField] private Rigidbody rb_jaw;

    // --------------------
    // Jaw Rotation Limits
    // --------------------
    [SerializeField] private float minPitch = 0f;
    [SerializeField] private float maxPitch = 30f;

    // --------------------
    // Jaw State Machine
    // --------------------
    private enum MouthState { Idle, Opening, Closing }
    private MouthState mouthState = MouthState.Idle;

    // --------------------
    // Movement Speeds
    // --------------------
    [SerializeField] private float openingSpeed = 20f;
    [SerializeField] private float closingSpeed = 20f;

    // --------------------
    // Interpolation Rates
    // --------------------
    [SerializeField] private float openRate = 1f;
    [SerializeField] private float closeRate = 3f;

    // --------------------
    // Internal State
    // --------------------
    private float currentPitch = 0f;


    void Awake()
    {
        rb_roof.MoveRotation(Quaternion.identity);
        rb_jaw.MoveRotation(Quaternion.identity);
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key) && mouthState == MouthState.Idle)
        {
            mouthState = MouthState.Opening;
        }


    }

    void FixedUpdate()
    {

        switch (mouthState)
        {

            case MouthState.Opening:
                currentPitch = Mathf.MoveTowards(
                    currentPitch,
                    maxPitch,
                    openingSpeed * Time.fixedDeltaTime
                );
                if (Mathf.Approximately(currentPitch, maxPitch))
                    mouthState = MouthState.Closing;
                break;

            case MouthState.Closing:
                currentPitch = Mathf.MoveTowards(
                    currentPitch,
                    minPitch,
                    closingSpeed * Time.fixedDeltaTime
                );
                if (Mathf.Approximately(currentPitch, 0f))
                    mouthState = MouthState.Idle;
                break;
        }

        Quaternion roofRot = Quaternion.Euler(-currentPitch, 0, 0);
        Quaternion jawRot = Quaternion.Euler(currentPitch, 0, 0);

        rb_roof.MoveRotation(roofRot);
        rb_jaw.MoveRotation(jawRot);

    }





}
