using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.EventSystems;

public class gameManager : MonoBehaviour
{
    #region Main Variables

    //Party Infotmation
    public string Leader = "Penny_Test_Head";

    public string[] partyMembers = new string[] { "", "", "" };
    public string[] extraMembers = new string[] { "", "", "", "" };

    private bool canPause = true; //Allow pausing?

    public Light skyBoxDirectionalLight;
    public float skyBoxDirectionalLerpValue = 1f;
    public GameObject fade;

    #region SOME VARIABLES IN THIS REGION MAY BE REMOVED ONCE COMBAT TRANSITION SYSTEM IS IMPROVED

    [Header("Common")]
    public CameraController mainCamera; //Main parent object for camera

    public PlayerController player; //Reference to player script

    [Header("Combat")] //Will most likely move to combat manager
    public Transform[] playerSpawn, enemySpawn;

    public Transform cameraSpawn;
    private GameObject enemyCombatTriggerer; //The enemy that triggered combat last (temporary value)

    [Header("Level-Specific")]
    public GameObject normalWorld; //Represents overworld

    public string areaId = "Level1";
    public string sceneName;
    public string chapterName = "Chapter _: The Antithesis of Graphic Design";
    public GameObject battleWorld; //Represents battlefield

    #endregion SOME VARIABLES IN THIS REGION MAY BE REMOVED ONCE COMBAT TRANSITION SYSTEM IS IMPROVED

    [Header("Menus")]

    public Animator pauseMenu;

    public Animator CanvasAnimator;

    [Header("Click Indicator")]
    public GameObject clickIndicator; //Has 2 particles, one for normal and one for turning off.

    public Animator clickIndicAnim;

    [Header("Inventory Management")]
    private InventoryManagement invMan;

    //Singleton creation
    private static gameManager instance;

    public static gameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<gameManager>();
            return instance;
        }
    }

    /** STATE KEY
     * NULL - NULL STATE, should not be used unless in a default case or if you know what you're doing.
     * START - Start of the game, initialize all systems
     * TRAVELING - Player is in the overworld, not in combat
     * COMBAT - Player is in a turn based combat mode, not in overworld
     * GAME_PAUSED - Game has paused without player input. Player has no control over this
     * PLAYER_PAUSED - Player has paused the game with input. Player can unpause at any time in this state.
     * TALKING - Player is speaking with NPC in VN Dialogue System
     */
    public enum STATE { NULL, START, TRAVELING, COMBAT, GAME_PAUSED, PLAYER_PAUSED, TALKING }

    public STATE gameState; //Current State of the game
    private STATE prevState; //Previous State of the game (before pausing)

    #endregion Main Variables

    [Header("Player Controls For Game")]
    public PlayerControls pControls;

    public GameObject resetText;

    #region Player Actions
    private void OnEnable()
    {
        player = PlayerController.Instance;
        mainCamera = CameraController.Instance;

        pControls = new PlayerControls();

        pControls.Player.TogglePause.performed += TogglePause;

        pControls.Player.TogglePause.Enable();
    }

    private void OnDisable()
    {
        pControls.Player.TogglePause.performed -= TogglePause;

        pControls.Player.TogglePause.Disable();
    }

    #endregion Player Actions

    private void Awake()
    {
        if (FindObjectsOfType<gameManager>().Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }
    private void Start()
    {
        skyBoxDirectionalLerpValue = 1f;

        Leader = "Penny_Test_Head";
        partyMembers = new string[] { "Player", "", "" };
        extraMembers = new string[] { "", "", "", "" };

        ChangeState(STATE.TRAVELING);
        prevState = STATE.START; //Start out of combat\
        clickIndicator.SetActive(false);
        sceneName = SceneManager.GetActiveScene().name;

        InventoryManagement.Instance.DisableInventoryInput();
    }

    public IEnumerator ClickOff()
    {
        clickIndicAnim.SetTrigger("Off");
        yield return new WaitForSeconds(0.25f);
        //clickIndicator.SetActive(false);
    }

    private void FixedUpdate()
    {
        skyBoxDirectionalLightLerp();
    }

    public void TogglePause(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                if (canPause)
                {
                    if (gameState == STATE.PLAYER_PAUSED)
                    {
                        StartCoroutine(ExitPauseMenu());

                    }
                    else //Player is able to pause and has not paused already
                    {
                        pauseMenu.gameObject.SetActive(true);
                        fade.SetActive(true);
                        Player_PauseGame();
                    }
                }
                break;

            default:
                break;
        }
    }


    /**
     * @brief Change the state of the game and update all dependant classes's game states
     */

    public void ChangeState(STATE state)
    {
        if (state != gameState && state != STATE.START && state != STATE.NULL) //Make sure state is not a duplicate or null
        {
            //If state is valid, change it.
            prevState = gameState;
            gameState = state;

            //Send message to dependant components within GameManager
            BroadcastMessage("ChangedStateTo", state);
            Debug.Log("Message Sent?");
        }
    }

    #region Pausing

    /**
     * @brief Pauses the game with player input. This cannot be done if canPause is false or if already paused.
     */
    public void Player_PauseGame()
    {
        if (gameState != STATE.GAME_PAUSED && gameState != STATE.PLAYER_PAUSED)
        {
            ChangeState(STATE.PLAYER_PAUSED);
            mainCamera.ChangeCameraState(CameraController.MODE.PAUSED);
            Time.timeScale = 0;
        }
    }

    /**
     * @brief Pauses the game without player input. This cannot be done if already paused. (Must change CameraState Separately)
     */
    public void Game_PauseGame()
    {
        if (gameState != STATE.GAME_PAUSED)
        {
            ChangeState(STATE.GAME_PAUSED);
            Time.timeScale = 0;

            //Player cannot pause when game pauses.
            AllowPlayerToPause(false);
        }
    }

    /**
     * @brief Unpauses the game with player input. Cannot do this if not paused or if gameState = GAME_PAUSED.
     */
    public void Player_UnPauseGame()
    {
        if (gameState == STATE.PLAYER_PAUSED)
        {
            ChangeState(prevState);
            mainCamera.ChangeCameraState(mainCamera.prevMode);
            Time.timeScale = 1;
            player.agent.ResetPath();

            InventoryManagement.Instance.DisableInventoryInput();

        }
    }
    /**
     * @brief Unpauses the game without. Cannot do this if not paused.
     * @param newState the state to go to when unPause is finished (will be previous state if none specified)
     */
    public void Game_UnPauseGame(STATE newState = STATE.NULL)
    {
        if (gameState == STATE.GAME_PAUSED || gameState == STATE.PLAYER_PAUSED)
        {
            if (newState == STATE.NULL) //If no state provided, unpause to previous state
                ChangeState(prevState);
            else
                ChangeState(newState);
            Time.timeScale = 1;
            player.agent.ResetPath();

            InventoryManagement.Instance.DisableInventoryInput();

            //Player must be allowed to pause when game unpauses.
            AllowPlayerToPause(true);
        }
    }

    public void OpenInventory()
    {
        SharedInventory sI = InventoryManagement.Instance.sharedInventory.GetComponentInChildren<SharedInventory>();

        pauseMenu.gameObject.SetActive(false);

        AllowPlayerToPause(false);

        if (!sI.gameObject.activeInHierarchy)
        {
            InventoryManagement.Instance.SetSharedInventoryActive(true);

            InventoryManagement.Instance.EnableInventoryInput();
        }

        sI.SelectItemByIndex(0);
    }


    public void AllowPlayerToPause(bool pause)
    {
        canPause = pause;
    }


    #endregion Pausing

    #region Entering Combat

    public void TriggerCombat(GameObject enemy)
    {
        // StartCoroutine(ScreenCap());
        Debug.Log("Triggering Combat");
        enemyCombatTriggerer = enemy;
        Game_PauseGame();
        AllowPlayerToPause(false);
        CanvasAnimator.SetTrigger("Battle");
        player.anim.SetTrigger("CombatTrigger");
        StartCoroutine(LoadCombatDelay());
    }

    private IEnumerator LoadCombatDelay()
    {
        Debug.Log("Loading Combatant Delay");
        yield return new WaitForSecondsRealtime(3f);
        LoadCombatants();
    }

    public void LoadCombatants()
    {
        Debug.Log("Loading Combatants");
        Game_UnPauseGame();

        //Turn on Battlefield
        normalWorld.SetActive(false);
        battleWorld.SetActive(true);

        //Teleport Player to the BattleField
        player.agent.ResetPath();
        player.agent.enabled = false;
        player.transform.position = player.grid.NearestGridNode(playerSpawn[0].position).worldPosition;
        player.combatPosition = player.grid.NearestGridNode(player.transform.position);
        player.agent.enabled = true;

        //Teleport Enemy to the BattleField
        enemyCombatTriggerer.transform.position = player.grid.NearestGridNode(enemySpawn[0].position).worldPosition;
        enemyCombatTriggerer = null;

        //Teleport Camera to the battlefield
        mainCamera.followScript.transform.position = cameraSpawn.transform.position;
        mainCamera.followScript.BattleOffset(cameraSpawn.transform.position);

        //Change the GameState to Combat
        ChangeState(STATE.COMBAT);
        CanvasAnimator.SetTrigger("Loaded");
        AllowPlayerToPause(true);
    }

    #endregion Entering Combat

    /**
     * @brief Decrease/increase skybox light to specified value
     */

    private void skyBoxDirectionalLightLerp()
    {
        skyBoxDirectionalLight.intensity = Mathf.Lerp(skyBoxDirectionalLight.intensity, skyBoxDirectionalLerpValue, 0.05f);
    }

    public void ExitPauseMenuFunction()
    {
        StartCoroutine(ExitPauseMenu());
    }

    public IEnumerator ExitPauseMenu()
    {
        //Play animation
        pauseMenu.SetTrigger("Exit");
        AllowPlayerToPause(false);
        yield return new WaitForSecondsRealtime(0.283f);
        pauseMenu.gameObject.SetActive(false);
        fade.SetActive(false);

        //Unpause
        Player_UnPauseGame();
        AllowPlayerToPause(true);
    }
}
