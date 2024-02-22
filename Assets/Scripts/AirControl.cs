using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Car))]
public class AirControl : MonoBehaviour
{
    Car car;
    Rigidbody rigidbody;

    [Header("Jump")]
    [SerializeField] float jumpStrength;
    [SerializeField] float airMod;
    [SerializeField] int airJumps;
    [SerializeField] int maxAirJumps;
    [SerializeField] ParticleSystem jumpFX;

    [Header("Air Rotation")]
    float yawAxis, rollAxis, pitchAxis;
    [SerializeField] float yawRate;
    [SerializeField] float pitchRate;
    [SerializeField] float rollRate;
    [SerializeField] float rotationDamping;

    private void Awake()
    {
        car = GetComponent<Car>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (car.GetGrounded())
        {
            airJumps = maxAirJumps;
        }
        else
        {
            ApplyAirRotationForce();
        }
    }
    // allows player to jump 
    public void Jump() 
    {
        ApplyJumpForce();
    }

    public void Pitch(float pitchAxis)
    {
        this.pitchAxis = Mathf.Clamp(pitchAxis, -1, 1);
    }

    public void Yaw(float yawAxis)
    {
        this.yawAxis = Mathf.Clamp(yawAxis, -1, 1);
    }

    public void Roll(float rollAxis)
    {
        this.rollAxis = Mathf.Clamp(rollAxis, -1, 1);
    }
    // puts force in the jump
    private void ApplyJumpForce()
    {
        if (car.GetGrounded())
        {
            rigidbody.AddForce(transform.up * jumpStrength, ForceMode.Impulse);
            jumpFX.Play();
        }
        else
        {
            if (airJumps > 0)
            {
                rigidbody.AddForce(transform.up * jumpStrength * airMod, ForceMode.Impulse);
                jumpFX.Play();
                airJumps--;
            }
        }
    }
    // allows car to yaw, pitch and roll.
    private void ApplyAirRotationForce()
    {
        float pitchVelocity = Vector3.Dot(transform.right, rigidbody.angularVelocity);
        float yawVelocity = Vector3.Dot(transform.up, rigidbody.angularVelocity);
        float rollVelocity = Vector3.Dot(transform.forward, rigidbody.angularVelocity);

        float yawTorque = (Mathf.Abs(yawAxis) > 0f) ? yawAxis * yawRate : -yawVelocity * rotationDamping;
        float rollTorque = (Mathf.Abs(rollAxis) > 0f) ? rollAxis * rollRate : -rollVelocity * rotationDamping;
        float pitchTorque = (Mathf.Abs(pitchAxis) > 0f) ? pitchAxis * pitchRate : -pitchVelocity * rotationDamping;

        rigidbody.AddTorque(transform.up * yawTorque);
        rigidbody.AddTorque(transform.forward * rollTorque);
        rigidbody.AddTorque(transform.right * pitchTorque);
    }
}
