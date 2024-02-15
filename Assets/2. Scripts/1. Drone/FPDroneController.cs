using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FPDroneController : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float gravityMultiplier = 4;
    [SerializeField] private float pitchRotationSpeed, rollRotationSpeed, yawRotationSpeed;
    [SerializeField] private float rotationSpeed = 5;
    
    private PlayerControlls playerControlls;
    private new Rigidbody rigidbody;

    private float throttle;

    private float pitchRotation;
    private float yawRotation;
    private float rollRotation;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerControlls = new PlayerControlls();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        playerControlls.Player.Move.performed += MoveInput;
        playerControlls.Player.Move.canceled += ResetMoveInput;
        playerControlls.Player.Move.Enable();
        playerControlls.Player.Elevate.performed += ElevateInput;
        // playerControlls.Player.Elevate.canceled += ResetElevateInput;
        playerControlls.Player.Elevate.Enable();
        playerControlls.Player.Look.performed += RotateInput;
        playerControlls.Player.Look.Enable();
        
        playerControlls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControlls.Player.Move.performed -= MoveInput;
        playerControlls.Player.Move.canceled -= ResetMoveInput;
        playerControlls.Player.Elevate.performed -= ElevateInput;
        // playerControlls.Player.Elevate.canceled -= ResetElevateInput;
        playerControlls.Player.Look.performed -= RotateInput;
    }


    private void MoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void ResetMoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }
    
    private void RotateInput(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }


    private void ElevateInput(InputAction.CallbackContext ctx)
    {
        throttle = ctx.ReadValue<float>();
    }
    
    // private void ResetElevateInput(InputAction.CallbackContext ctx)
    // {
    //     throttle = 0;
    // }

    private void Update()
    {
        if (!playerControlls.Player.Elevate.inProgress)
        {
            throttle -= Time.deltaTime;
            throttle = Mathf.Clamp01(throttle);
        }

        Vector2 input = playerControlls.Player.Look.ReadValue<Vector2>();
        rollRotation += input.x * rollRotationSpeed;
        pitchRotation += input.y * pitchRotationSpeed;
        
        
        yawRotation += moveInput.x * yawRotationSpeed;
        
        Quaternion targetRotation = Quaternion.identity;
        targetRotation.eulerAngles = Vector3.up * yawRotation + Vector3.back * rollRotation + Vector3.right * pitchRotation;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void FixedUpdate()
    {
        rigidbody.AddForce(Vector3.down * (9.81f * gravityMultiplier));
        rigidbody.AddRelativeForce(new Vector3(0, throttle * speed, 0), ForceMode.Acceleration);
    }
}
