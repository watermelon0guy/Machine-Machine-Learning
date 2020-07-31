using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelScript : MonoBehaviour
{
    public Car car;
    public Rigidbody rb;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9 && car.alive)
        {
            car.alive = false;
            car.inf.aliveCreature -= 0;
            rb.isKinematic = true;
        }
    }
}
