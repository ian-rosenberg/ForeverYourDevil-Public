using UnityEngine;

public class NPCController : MonoBehaviour
{
    private Dialogue diagManager;
    private gameManager gameManager;
    public string conversationID; //Conversation number for communication
    public GameObject talkIndicator;

    [Header("Player Input")]
    private PlayerControls pControls;
    private bool interact = false;

    // Start is called before the first frame update
    private void Start()
    {
        diagManager = Dialogue.Instance;
        gameManager = gameManager.Instance;
    }

    private void OnEnable()
    {
        pControls = new PlayerControls();

        pControls.UI.Interact.performed += context => interact = !interact;

        pControls.UI.Interact.Enable();

        pControls.Player.ManualTravel.Enable();
    }


    private void OnDisable()
    {
        pControls.UI.Interact.performed -= context => interact = !interact;

        pControls.UI.Interact.Disable();

        pControls.Player.ManualTravel.Disable();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && gameManager.gameState == gameManager.STATE.TRAVELING)
        {
            talkIndicator.SetActive(true);

            if (interact)
            {
                Debug.Log("Pressing interact...");
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

    private void Update()
    {
        Debug.Log("interact"+interact);
    }
}