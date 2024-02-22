using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBoost : MonoBehaviour
{
    private bool active = false;
    [SerializeField] private float boost;
    private Rigidbody rigidbody;
    [SerializeField] private float maxBoost;
    [SerializeField] private float forceStrength;
    [SerializeField] ParticleSystem particleSystem;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        boost = maxBoost;
    }
    // animation plays when shift is hit
    public void ToggleBoost(bool newValue)
    {
        if (active == newValue) return;
        active = newValue;

        if (active && boost > 0f) particleSystem.Play();
    }


    private void FixedUpdate()
    {
        // makes boost meter work
        PlayerUI.SetImageFill("Boost Meter Fill", boost / maxBoost);
        if (!active || boost <= 0f)
        {
            particleSystem.Stop();
            return;
        }
        if (!active || boost <= 0f) return; 

        float forwardVelocity = Vector3.Dot(transform.forward, rigidbody.velocity);
        float speedRatio = (1 - (forwardVelocity / forceStrength));
        rigidbody.AddForce(transform.forward * forceStrength * speedRatio);
        boost -= Time.fixedDeltaTime;
    }

    #region Interface
    // public void ToggleBoost(bool newValue) => active = newValue;
    public void MaxBoost() => boost = maxBoost;
    #endregion

}
