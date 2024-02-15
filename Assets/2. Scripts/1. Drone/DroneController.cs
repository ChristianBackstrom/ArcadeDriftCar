using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [SerializeField] private float angle;
    [SerializeField] private float elevationSpeed, speed;
    [SerializeField] private float yRotationSpeed, xRotationSpeed;

    private Animator animator;
    
    private new Rigidbody rigidbody;

    private PlayerControlls playerControlls;

    private float pitchAngle, rollAngle;
    private float pitchAxis, rollAxis, yAxis;

    private float yRotation;
    private float xRotation;


    private Vector2 moveInput;
    private Vector2 elevateInput;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.SetBool("IsGrounded", false);
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
        playerControlls.Player.Elevate.canceled += StopElevateInput;
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
        playerControlls.Player.Elevate.canceled -= StopElevateInput;
        playerControlls.Player.Look.performed -= RotateInput;
    }


    private void MoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void RotateInput(InputAction.CallbackContext ctx)
    {
    }

    private void ResetMoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    private void ElevateInput(InputAction.CallbackContext ctx)
    {
        yAxis = ctx.ReadValue<float>() * elevationSpeed;
    }

    private void StopElevateInput(InputAction.CallbackContext ctx)
    {
        yAxis = 0;
    }
    
    
    private void HandleControlls()
    {
        float pitchInput = moveInput.y;
        float rollInput = moveInput.x;

        if (pitchInput != 0)
        {
            pitchAngle = Mathf.Lerp(pitchAngle, angle * pitchInput, Time.deltaTime);
            pitchAxis = speed * pitchInput;
        }
        else
        {
            pitchAngle = Mathf.Lerp(pitchAngle, 0, Time.deltaTime);
            pitchAxis = 0;
        }

        if (rollInput != 0)
        {
            rollAngle = Mathf.Lerp(rollAngle, angle * rollInput, Time.deltaTime);
            rollAxis = speed * rollInput;
        }
        else
        {
            rollAngle = Mathf.Lerp(rollAngle, 0, Time.deltaTime);
            rollAxis = 0;
        }

        if (pitchInput != 0 && rollInput != 0)
        {
            pitchAxis *= .5f;
            rollAxis *= .5f;
        } 
    }
    
    private void Update()
    {
        HandleControlls();
        Vector2 input = playerControlls.Player.Look.ReadValue<Vector2>();
        yRotation += input.x * yRotationSpeed;
        xRotation += input.y * yRotationSpeed;
        
        transform.localEulerAngles = Vector3.back * rollAngle + Vector3.right * pitchAngle + Vector3.up * yRotation +
            Vector3.right * xRotation;
    }

    private void FixedUpdate()
    {
        rigidbody.AddRelativeForce(rollAxis, yAxis, pitchAxis);
    }
}
