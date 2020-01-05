using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    Dialogue diagManager;
    gameManager gameManager;
    public string conversationID; //Conversation number for communication
    public GameObject talkIndicator;
    bool canTalk; //Can user press button to load conversation?

    // Start is called before the first frame update
    void Start()
    {
        diagManager = Dialogue.Instance;
        gameManager = gameManager.Instance;
        canTalk = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canTalk && gameManager.gameState == gameManager.STATE.TRAVELING && Input.GetKeyDown(KeyCode.E))
        {
            diagManager.TriggerDialogue(conversationID);
            talkIndicator.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && gameManager.gameState == gameManager.STATE.TRAVELING)
        {
            canTalk = true;
            talkIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            canTalk = false;
            talkIndicator.SetActive(false);
        }
    }

}
