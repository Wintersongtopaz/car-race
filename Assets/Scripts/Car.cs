using UnityEngine;
using System.Collections.Generic;

public class Car : MonoBehaviour
{
    private Rigidbody rigidbody;  // reference to rigidbody
    private float driveAxis, brakeAxis, turnAxis;    // allows values to work in private methods
    private bool grounded = false;

    [Header("Suspension")]

    [SerializeField] List<Transform> wheels;

    [Tooltip("Radius used for wheel raycasts.")]
    [Range(0.1f, 1f)]
    [SerializeField] float wheelRadius = 0.4f;

    [Tooltip("Spring force constant k. Applies upwards spring force proportional to wheel vertical offset.")]
    [Range(50f, 250f)]
    [SerializeField] float springStrength = 100f;

    [Tooltip("Spring damping value. Damps spring force proportional to point velocity.")]
    [Range(1f, 5f)]
    [SerializeField] float springDamping = 3f;

    public float maxSpeed;

    [Header("Friction")]

    [Tooltip("Longitudinal friction coefficient. Used to apply oppositional longitudinal force proportional to velocity")]
    [Range(1f, 5f)]
    [SerializeField] float longitudinalFriction = 2f;

    [Tooltip("Lateral friction coefficient. Used to apply oppositional lateral force proportional to velocity.")]
    [Range(1f, 5f)]
    [SerializeField] float lateralFriction = 2f;

    [Header("Steering")]

    [Tooltip("Turn angle for wheels.")]
    [Range(10, 45)]
    [SerializeField] float steeringAngle = 30f;

    [Tooltip("Damping coefficient for Y-Axis rotational velocity")]
    [Range(1f, 10f)]
    [SerializeField] float turnDamping = 5f;

    #region Public InterFace
    // Accepts and validates external inputs. Clamps axis between values
    public void Drive(float driveAxis) 
    {
        this.driveAxis = Mathf.Clamp(driveAxis, -1f, 1f);
    }

    public void Brake(float brakeAxis)
    {
        this.brakeAxis = Mathf.Clamp(brakeAxis, 0f, 1f);
    }

    public void Turn(float turnAxis)
    {
        this.turnAxis = Mathf.Clamp(turnAxis, -1f, 1f);
    }

    public bool GetGrounded() => grounded;
    #endregion

    #region MonoBehavior Life Cycle
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()   // to add forces to rigidbody
    {
        ApplySuspensionForce();

        if (!grounded) return;

        ApplyLongitudinalForce();
        ApplyLateralForce();
        ApplyTurningForce();
    }
    #endregion

    #region Forces
    private void ApplySuspensionForce()
    {
        bool tempGrounded = false;
        foreach(Transform wheel in wheels)  //for each wheel is going to do the steps
        {
            Vector3 origin = wheel.position;
            Vector3 direction = -wheel.up;
            RaycastHit hit;
            float offset = 0f;

            if (Physics.Raycast(origin,direction,out hit, wheelRadius)) //raycast is running
            {
                tempGrounded = true;

                Vector3 end = origin + (direction * wheelRadius); // calculates the offset
                offset = (end - hit.point).magnitude;
                // calculates spring force when raycast is hiting the ground
                float pointVelocity = Vector3.Dot(wheel.up, rigidbody.GetPointVelocity(wheel.position));
                // calculates suspension force
                float suspensionForce = (springStrength * offset) + (-pointVelocity * springDamping);
                //apply to rigidboy at the position of wheel
                rigidbody.AddForceAtPosition(wheel.up * suspensionForce, wheel.position); 
            }
        }
        grounded = tempGrounded;
    }
    private void ApplyLongitudinalForce() // makes the car move forward and backward
    {
        Vector3 force = Vector3.zero;
        float forwardVelocity = Vector3.Dot(transform.forward, rigidbody.velocity);
        float maxSpeedRatio = (1 - (Mathf.Abs(forwardVelocity) / maxSpeed));

        if (Mathf.Abs(driveAxis) > 0)
        {
            force = transform.forward * driveAxis * maxSpeed * maxSpeedRatio;
        }
        else
        {
            force = transform.forward * -forwardVelocity * longitudinalFriction;
        }

        rigidbody.AddForce(force);
    }
    private void ApplyLateralForce()  // makes care move left to right
    {
        float rightVelocity = Vector3.Dot(transform.right, rigidbody.velocity);
        rigidbody.AddForce(transform.right * -rightVelocity * lateralFriction);
    }
    private void ApplyTurningForce() // makes the car turn
    {
        float forwardVelocity = Vector3.Dot(transform.forward, rigidbody.velocity);
        float rotationalVelocity = Vector3.Dot(transform.up, rigidbody.angularVelocity);

        float torque = forwardVelocity * turnAxis * (Mathf.Deg2Rad * steeringAngle);
        torque += -rotationalVelocity * turnDamping;

        rigidbody.AddTorque(transform.up * torque);
    }
    #endregion
}
