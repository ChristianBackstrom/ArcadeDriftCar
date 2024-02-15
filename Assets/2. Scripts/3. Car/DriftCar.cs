using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class DriftCar : MonoBehaviour
{
    [Header("Physics")]
    [Space(15)]
    [SerializeField] private float power;

    [SerializeField] private float steeringPower;

    [SerializeField] private float maxSteering = 25;

    [SerializeField] private float steeringSpeed = 4;

    [SerializeField] private float isGroundedDistance = 1f;

    [SerializeField] private LayerMask groundLayer;

    [Header("Visual")] 
    [Space(15)]
    
    [SerializeField] private Transform[] FrontWheels = new Transform[2];
    [SerializeField] private float maxSteeringAngle = 25;
    [Space(5)]
    [SerializeField] private ParticleSystem[] RearWheelParticle = new ParticleSystem[2];
    [SerializeField] private int smokeDensity = 5;
    private PlayerControlls playerControlls;

    private new Rigidbody rigidBody;

    public Rigidbody Rigidbody => rigidBody;
    

    private Vector2 moveInput;

    private float Throttle => moveInput.y;
    private float Steering => moveInput.x;
    
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        playerControlls = new PlayerControlls();

        playerControlls.Car.Throttle.Enable();
        playerControlls.Car.Throttle.performed += ThrottleInput;
        playerControlls.Car.Throttle.canceled += ResetThrottleInput;
        playerControlls.Car.Steering.Enable();
        playerControlls.Car.Steering.performed += SteeringInput;
        playerControlls.Car.Steering.canceled += ResetSteeringInput;
    }

    private void OnDisable()
    {
        playerControlls.Car.Throttle.Disable();
        playerControlls.Car.Throttle.performed -= ThrottleInput;
        playerControlls.Car.Throttle.canceled -= ResetThrottleInput;
        playerControlls.Car.Steering.Disable();
        playerControlls.Car.Steering.performed -= SteeringInput;
        playerControlls.Car.Steering.canceled -= ResetSteeringInput;
    }

    private void ThrottleInput(InputAction.CallbackContext ctx)
    {
        moveInput.y = ctx.ReadValue<float>();
    }

    private void ResetThrottleInput(InputAction.CallbackContext ctx)
    {
        moveInput.y = 0;
    }
    
    private void SteeringInput(InputAction.CallbackContext ctx)
    {
        moveInput.x = ctx.ReadValue<float>();
    }

    private void ResetSteeringInput(InputAction.CallbackContext ctx)
    {
        moveInput.x = 0;
    }

    private void FixedUpdate()
    {
        if (!IsGrounded()) return;
        
        // float direction = Mathf.Sign(Vector3.Dot(rigidBody.velocity, transform.right));
        float direction = 1;
        Quaternion rotation = rigidBody.rotation;

        rotation *= Quaternion.AngleAxis(Mathf.Clamp(Steering * direction * rigidBody.velocity.magnitude * 
            steeringPower, -maxSteering, maxSteering), 
            Vector3.up);

        rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, rotation, Time.deltaTime * steeringSpeed);
        
        rigidBody.AddRelativeForce(Throttle * power,0, 0, ForceMode.Acceleration);
        
        rigidBody.AddRelativeForce(-Vector3.right * (rigidBody.velocity.magnitude * Steering) / 2, ForceMode.Force);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, isGroundedDistance, groundLayer);
    }

    private void Update()
    {
        foreach (Transform wheel in FrontWheels)
        {
            wheel.localRotation = Quaternion.AngleAxis(Steering * maxSteeringAngle, Vector3.up);
        }

        Vector3 xzVelocity = new (rigidBody.velocity.x, 0, rigidBody.velocity.z);

        if (Vector3.Dot(transform.right, xzVelocity.normalized) < .9f && xzVelocity.magnitude > 3f)
        {
            foreach (ParticleSystem particles in RearWheelParticle)
            {
                particles.Emit(smokeDensity);
            }
        }
       
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * isGroundedDistance);
        
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + rigidBody.velocity.normalized * 5);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 5);

    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(DriftCar))]
public class DriftCarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DriftCar car = (DriftCar)target;

        if (!car.Rigidbody) return;
        EditorGUILayout.LabelField("velocity: ", car.Rigidbody.velocity.magnitude.ToString());
        
        Vector3 xzVelocity = new(car.Rigidbody.velocity.x, 0, car.Rigidbody.velocity.z);

        EditorGUILayout.LabelField("Dot: ", Vector3.Dot(car.transform.right, xzVelocity.normalized).ToString());
    }
}
#endif