using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    Dialogue diagManager;
    gameManager gameManager;
    public string conversationID; //Conversation number for communication
    public GameObject talkIndicator;

    // Start is called before the first frame update
    void Start()
    {
        diagManager = Dialogue.Instance;
        gameManager = gameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && gameManager.gameState == gameManager.STATE.TRAVELING)
        {
            talkIndicator.SetActive(true);

            if (Input.GetButtonDown("Interact"))
            {
                diagManager.TriggerDialogue(conversationID);
                talkIndicator.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            talkIndicator.SetActive(false);
        }
    }

}
