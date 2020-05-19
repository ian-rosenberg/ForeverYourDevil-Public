using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    #region Main Variables

    bool canPause = true; //Allow pausing?

    //public GameObject CameraX, CameraY;
    [Header("Common")]
    public FollowObject mainCamera; //Main parent object for camera
    public Animator battleAnim; //Canvas for battle transition
    public Image screenCapRegion; //Place to display image on

    public PlayerController player; //Reference to player script

    [Header("Combat")]
    public Transform[] playerSpawn, enemySpawn;
    public Transform cameraSpawn;
    GameObject enemyCombatTriggerer; //The enemy that triggered combat last (temporary value)

    [Header("Level-Specific")]
    public GameObject normalWorld; //Represents overworld
    public GameObject battleWorld; //Represents battlefield

    [Header("Menus")]
    public GameObject pauseMenu;


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

    public enum STATE { START, TRAVELING, COMBAT, PAUSED, TALKING }

    public STATE gameState; //Current State of the game
    STATE prevState; //Previous State of the game (before pausing)
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(STATE.TRAVELING);
        prevState = STATE.START; //Start out of combat
    }

    // Update is called once per frame
    void Update()
    {
        //Pause Game
        if (Input.GetButtonDown("Pause") && canPause)
        {
            if (gameState == STATE.PAUSED)
            {
                pauseMenu.SetActive(false);
                UnPauseGame();
            }
            else
            {
                pauseMenu.SetActive(true);
                PauseGame();
            }
        }
    }

    //Change the state of the game and update all dependant classes's game states
    public void ChangeState(STATE state)
    {
        if (state != gameState) //Make sure state is not a duplicate
        {
            switch (state)
            {
                case STATE.TALKING:
                    player.currentBehavior = player.Player_Talking;
                    break;
                case STATE.TRAVELING:
                    player.currentBehavior = player.Player_Travelling;
                    break;
                case STATE.COMBAT:
                    player.currentBehavior = player.Player_Combat;
                    break;
                case STATE.START:
                    Debug.LogError("Cannot go back to Start. Don't collect $200.");
                    return;
            }
            //If state is valid, change it.
            prevState = gameState;
            gameState = state;
        }
    }

    #region Pausing
    public void PauseGame()
    {
        if (gameState != STATE.PAUSED)
        {
            ChangeState(STATE.PAUSED);
            Time.timeScale = 0;
        }
    }
    public void UnPauseGame()
    {
        if (gameState == STATE.PAUSED)
        {
            ChangeState(prevState);
            Time.timeScale = 1;
        }
    }
    public void SetCanPause(bool pause)
    {
        canPause = pause;
    }
    #endregion

    #region Entering Combat
    public void TriggerCombat(GameObject enemy)
    {
        // StartCoroutine(ScreenCap());
        Debug.Log("Triggering Combat");
        enemyCombatTriggerer = enemy;
        PauseGame();
        SetCanPause(false);
        battleAnim.SetTrigger("Battle");
        StartCoroutine(LoadCombatDelay());
    }

    IEnumerator LoadCombatDelay()
    {
        Debug.Log("Loading Combatant Delay");
        yield return new WaitForSecondsRealtime(3f);
        LoadCombatants();
    }

    public void LoadCombatants()
    {
        PlayerController pc = player.GetComponent<PlayerController>();

        Debug.Log("Loading Combatants");
        UnPauseGame();

        normalWorld.SetActive(false);
        battleWorld.SetActive(true);

        NavMeshAgent playerAgent = player.GetComponent<NavMeshAgent>();
        //playerAgent.enabled = true;
        playerAgent.ResetPath();
        playerAgent.enabled = false;

        player.transform.position = pc.grid.NearestGridNode(playerSpawn[0].position).worldPosition;
        pc.combatPosition = pc.grid.NearestGridNode(player.transform.position);
        playerAgent.enabled = true;
        //playerAgent.ResetPath();
        enemyCombatTriggerer.transform.position = pc.grid.NearestGridNode(enemySpawn[0].position).worldPosition;
        enemyCombatTriggerer = null;
        mainCamera.SetOffset(cameraSpawn.transform.position);

        ChangeState(STATE.COMBAT);

        battleAnim.SetTrigger("Loaded");
        SetCanPause(true);
    }
    #endregion


}
