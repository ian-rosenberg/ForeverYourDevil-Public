using UnityEngine;

public class PartyManager : MonoBehaviour
{
    #region Main Variables

    public gameManager gameManager;

    [Header("Party")]
    public PartyMember currentPlayer;

    public PartyMember[] party;

    public static PartyManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PartyManager>();
            return instance;
        }
    }

    //Singleton creation
    private static PartyManager instance;

    [Header("Inventories")]
    private SharedInventory sharedInventory;

    private PersonalInventory[] personalInventories;

    public enum STATE { START, TRAVELING, COMBAT, PAUSED, TALKING }

    public STATE gameState; //Current State of the game
    private STATE prevState; //Previous State of the game (before pausing)

    #endregion Main Variables

    private void Awake()
    {
        gameManager = gameManager.Instance;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}