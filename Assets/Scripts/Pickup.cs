using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] string validTag;
    [SerializeField] string messageName;
    [SerializeField] float respawnTime = 2f;
    [SerializeField] GameObject particleSystemPrefab;
    bool active = true;

    private void OnTriggerEnter(Collider other)
    {
        if (active && other.attachedRigidbody.gameObject.CompareTag(validTag))
        {
            Debug.Log("Collided");
            other.attachedRigidbody.gameObject.SendMessage(messageName, SendMessageOptions.DontRequireReceiver);
            ToggleActive(false);
            StartCoroutine(Respawn());
            Instantiate(particleSystemPrefab, transform.position, transform.rotation); //Instantiate particle system prefab
        }
    }
    void ToggleActive(bool newValue)
    {
        active = newValue;
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }
    IEnumerator Respawn() // respawn pickup
    {
        yield return new WaitForSeconds(respawnTime);
        ToggleActive(true);
    }
}
