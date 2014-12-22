using UnityEngine;
using System.Collections;

public class SpawnObjectInteraction : MonoBehaviour, IInteraction
{
    public GameObject ObjectToSpawn;
    public Vector3 RelativeLocationToSpawn;

    public void Interact()
    {
        Instantiate(ObjectToSpawn, transform.position + RelativeLocationToSpawn, Random.rotation);
    }
}
