using UnityEngine;
using UnityEngine.Events;

public class BoundsVolume : MonoBehaviour
{
    public static UnityEvent<Rigidbody> OnBoundsVolumeEnter = new UnityEvent<Rigidbody>();
    // makes sure play dosent fall through ground when car rolls
    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.gameObject.CompareTag("Car"))
        {
            OnBoundsVolumeEnter.Invoke(other.attachedRigidbody);
        }
    }
}
