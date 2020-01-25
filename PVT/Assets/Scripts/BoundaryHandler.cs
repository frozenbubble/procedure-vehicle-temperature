using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryHandler : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        var bulletBehaviour = other.GetComponent<BulletBehaviour>();

        if (bulletBehaviour != null)
        {
            Destroy(other.gameObject);
        }
    }
}
