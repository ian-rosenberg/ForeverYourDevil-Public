using UnityEngine;

public class NPCController : MonoBehaviour
{
    private Dialogue diagManager;
    private gameManager gameManager;
    public string conversationID; //Conversation number for communication
    public GameObject talkIndicator;

    private bool canTalk = false;

    // Start is called before the first frame update
    private void Start()
    {
        diagManager = Dialogue.Instance;
        gameManager = gameManager.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
        if (canTalk == true)
        {
            if (Input.GetButtonDown("Interact"))
            {
                diagManager.TriggerDialogue(conversationID);
                talkIndicator.SetActive(false);
                canTalk = false; //need this to not make the text turn to gibberish. 
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && gameManager.gameState == gameManager.STATE.TRAVELING)
        {
            talkIndicator.SetActive(true);
            canTalk = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            talkIndicator.SetActive(false);
            canTalk = false;
        }
    }
}
