using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDoor : Door
{
    //Collide/Trigger this door in order to warp player

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colliding with door");
        if (collision.collider.gameObject.layer == 10 && !locked)
        { //10 = Player
            Activate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggering door");
        if (other.gameObject.layer == 10 && !locked)
        { //10 = Player
            Activate();
        }
    }


}
