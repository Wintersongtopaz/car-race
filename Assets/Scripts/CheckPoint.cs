using UnityEngine.Events;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] Animator animator;
    bool checkpointEnabled = true;
    public static UnityEvent<CheckPoint, GameObject> OnCheckpointPassed = new UnityEvent<CheckPoint, GameObject>();
    // when check point hit will be disabled
    private void OnTriggerEnter(Collider other)
    {
        if (!checkpointEnabled) return;
        if (other.attachedRigidbody.CompareTag("Car"))
        {
            OnCheckpointPassed.Invoke(this, other.attachedRigidbody.gameObject);
            checkpointEnabled = false;
        }
    }
    public void SetCheckpointEnabled(bool newValue) => animator.SetBool("Checkpoint Enabled", (checkpointEnabled = newValue));
 
}
