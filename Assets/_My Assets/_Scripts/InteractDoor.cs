using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDoor : Door
{
    public GameObject talkIndicator;
    SphereCollider col;
    bool canActivate;
    private void Update()
    {
        if (canActivate)
        {
            if (Input.GetButtonDown("Interact"))
            {
                Activate();
                talkIndicator.SetActive(false);
                canActivate = false;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10 && !locked) //10 = player
        {
            talkIndicator.SetActive(true);
            canActivate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            talkIndicator.SetActive(false);
            canActivate = false;
        }
    }

}

