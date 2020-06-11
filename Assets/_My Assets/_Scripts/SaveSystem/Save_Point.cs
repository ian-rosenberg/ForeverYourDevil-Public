using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save_Point : MonoBehaviour
{
    public GameObject SaveCanvas;
    gameManager gm;
    SaveManager sm;

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
                SaveCanvas.SetActive(true);
                gm.PauseGame();
                sm.UpdateAllSaveSlots();
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10) //10 = player
        {
            talkIndicator.SetActive(true);
            canActivate = true;
            gm.skyBoxDirectionalLerpValue=0.62f;
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
