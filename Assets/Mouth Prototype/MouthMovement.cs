using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class MouthMovement : MonoBehaviour
{
    // --------------------
    // Inspector Fields
    // --------------------

    [Header("Input Settings")]
    [SerializeField] private KeyCode key = KeyCode.Q;

    [Header("Rigidbody References")]
    [SerializeField] private Rigidbody rb_roof;
    [SerializeField] private Rigidbody rb_jaw;

    [Header("Jaw Rotation Limits")]
    [SerializeField] private float minPitch = 0f;
    [SerializeField] private float maxPitch = 30f;

    [Header("Movement Speeds")]
    [SerializeField] private float openingSpeed = 20f;
    [SerializeField] private float closingSpeed = 20f;

    [Header("Interpolation Rates")]
    [SerializeField] private float openRate = 1f;
    [SerializeField] private float closeRate = 3f;

    // --------------------
    // Internal State (Hidden)
    // --------------------
    private enum MouthState { Idle, Opening, Closing }
    private MouthState mouthState = MouthState.Idle;
    private float currentPitch = 0f;

    private bool isOpeningMouth;


    void Awake()
    {
        rb_roof.MoveRotation(Quaternion.identity);
        rb_jaw.MoveRotation(Quaternion.identity);
    }



    // Update is called once per frame
    void Update()
    {
        isOpeningMouth = Input.GetKeyDown(key);
    }

    void FixedUpdate()
    {

        float targetPitch = isOpeningMouth ? currentPitch + openRate : currentPitch - closeRate;
        float speed = isOpeningMouth ? openingSpeed : closingSpeed;
        currentPitch = Mathf.Lerp(
                currentPitch,
                targetPitch,
                speed * Time.deltaTime
            );

        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        Debug.Log($"{currentPitch}");

        Quaternion roofRot = Quaternion.Euler(-currentPitch, 0, 0);
        Quaternion jawRot = Quaternion.Euler(currentPitch, 0, 0);

        rb_roof.MoveRotation(roofRot);
        rb_jaw.MoveRotation(jawRot);

    }





}
