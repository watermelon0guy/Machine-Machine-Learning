using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScript : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 10)
        {
            transform.parent.Translate(2, 0, 0, Space.World);
            Debug.Log("tr");
        }
    }
}
