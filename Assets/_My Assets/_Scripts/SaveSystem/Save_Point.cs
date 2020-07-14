using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save_Point : MonoBehaviour
{
    gameManager gm;
    SaveManager sm;

    [Tooltip("The distance that the player must be in to interact with the save point")]
    public float distanceRange;
    public GameObject talkIndicator;
    bool canActivate;

    private void Awake()
    {
        gm = gameManager.Instance;
        sm = SaveManager.Instance;
    }

    private void Update()
    {
        if (canActivate)
        {
            //WILL CHANGE WITH NEW INPUT SYSTEM
            if (Input.GetButtonDown("Interact"))
            {
                sm.ShowSaveMenu();
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10 && Vector3.Distance(other.transform.position, transform.position) < distanceRange) //10 = player
        {
            talkIndicator.SetActive(true);
            canActivate = true;
            gm.skyBoxDirectionalLerpValue = 0.62f;
        }
        else
        {
            talkIndicator.SetActive(false);
            canActivate = false;
            gm.skyBoxDirectionalLerpValue = 1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            talkIndicator.SetActive(false);
            canActivate = false;
            gm.skyBoxDirectionalLerpValue = 1f;

        }
    }

}
