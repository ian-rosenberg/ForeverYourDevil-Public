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

    //private void OnEnable()
    //{
    //    pControls = new PlayerControls();

    //    pControls.Player.Use.performed += context => interact = true;

    //    pControls.Player.ManualTravel.Disable();
    //}


    //private void OnDisable()
    //{
    //    pControls.Player.Use.performed -= context => interact = true;

    //    pControls.Player.Use.Disable();
    //    pControls.Player.ManualTravel.Enable();
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && gameManager.gameState == gameManager.STATE.TRAVELING)
        {
            talkIndicator.SetActive(true);

            //if (interact)
            //{
            //    pControls.Player.Use.performed -= context => interact = true;

            //    diagManager.TriggerDialogue(conversationID);
            //    talkIndicator.SetActive(false);
            //}

            if (Input.GetKeyDown(KeyCode.E)){
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


    //private void Update()
    //{
    //    Debug.Log("interact" + interact);
    //}
}
