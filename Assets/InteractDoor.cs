using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;

public class InteractDoor : Door
{
    public GameObject talkIndicator;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10 && gm.gameState == gameManager.STATE.TRAVELING) //10 = player
        {
            talkIndicator.SetActive(true);

            if (Input.GetButtonDown("Interact"))
            {
                Activate();
                talkIndicator.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            talkIndicator.SetActive(false);
        }
    }

}

