using UnityEngine;

public class NPCController : MonoBehaviour
{
    private Dialogue diagManager;
    private gameManager gameManager;
    public string conversationID; //Conversation number for communication
    public GameObject talkIndicator;

<<<<<<< HEAD
=======
    [Header("Player Input")]
    private PlayerControls pControls;
    private bool interact = false;

>>>>>>> Rebuilding inventory
    // Start is called before the first frame update
    private void Start()
    {
        diagManager = Dialogue.Instance;
        gameManager = gameManager.Instance;
    }

<<<<<<< HEAD
    // Update is called once per frame
    private void Update()
    {
=======
    private void OnEnable()
    {
        pControls = new PlayerControls();

        pControls.UI.Interact.performed += context => interact = !interact;

        pControls.Player.ManualTravel.Enable();
    }


    private void OnDisable()
    {
        pControls.UI.Interact.performed -= context => interact = !interact;

        pControls.Player.ManualTravel.Disable();
>>>>>>> Rebuilding inventory
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && gameManager.gameState == gameManager.STATE.TRAVELING)
        {
            talkIndicator.SetActive(true);

<<<<<<< HEAD
            if (Input.GetButtonDown("Interact"))
=======
            if (interact)
>>>>>>> Rebuilding inventory
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