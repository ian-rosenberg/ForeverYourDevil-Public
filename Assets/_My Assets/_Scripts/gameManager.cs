﻿using System.Collections;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    #region Main Variables

    private bool canPause = true; //Allow pausing?

    //public GameObject CameraX, CameraY;
    [Header("Common")]
    public CameraController mainCamera; //Main parent object for camera

    public Animator battleAnim; //Canvas for battle transition
    public PlayerController player; //Reference to player script

    [Header("Combat")] //Will most likely move to combat manager
    public Transform[] playerSpawn, enemySpawn;

    public Transform cameraSpawn;
    private GameObject enemyCombatTriggerer; //The enemy that triggered combat last (temporary value)

    [Header("Level-Specific")]
    public GameObject normalWorld; //Represents overworld

    public GameObject battleWorld; //Represents battlefield

    [Header("Menus")]
    public GameObject pauseMenu;

    public Animator CanvasAnimator;

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

    public enum STATE { START, TRAVELING, COMBAT, PAUSED, TALKING }

    public STATE gameState; //Current State of the game
    private STATE prevState; //Previous State of the game (before pausing)

    #endregion Main Variables

    // Start is called before the first frame update
    private void Start()
    {
        ChangeState(STATE.TRAVELING);
        prevState = STATE.START; //Start out of combat
    }

    // Update is called once per frame
    private void Update()
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

    public void ToggleInventory()
    {
        InventoryManagement.Instance.SetSharedInventoryActive(!InventoryManagement.Instance.isActiveAndEnabled);
    }

    public void SetCanPause(bool pause)
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
        PauseGame();
        SetCanPause(false);
        battleAnim.SetTrigger("Battle");
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
        UnPauseGame();

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
        mainCamera.followScript.SetOffset(cameraSpawn.transform.position);

        //Change the GameState to Combat
        ChangeState(STATE.COMBAT);
        battleAnim.SetTrigger("Loaded");
        SetCanPause(true);
    }

    #endregion Entering Combat
}