using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{

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

    public enum STATE { TRAVELING, COMBAT, PAUSED, TALKING }

    public STATE gameState; //Current State of the game
    bool isPaused; //Is the game paused?
    STATE prevState; //Previous State of the game (before pausing)

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        gameState = STATE.TRAVELING; //Start out of combat
        prevState = STATE.TRAVELING; //Start out of combat
    }

    // Update is called once per frame
    void Update()
    {
        //Pause Game
        if (Input.GetButtonDown("Pause"))
        {
            Debug.Log("Pressed Enter/Escape");
            if (isPaused) UnPauseGame();
            else PauseGame();
        }

    }

    public void PauseGame()
    {
        if (!isPaused)
        {
            pauseMenu.SetActive(true);
            prevState = gameState;
            gameState = STATE.PAUSED;
            isPaused = true;
            Debug.Log("gameState = " + gameState);
            Debug.Log("prevState = " + prevState);
            Time.timeScale = 0;
        }
    }
    public void UnPauseGame()
    {
        if (isPaused)
        {
            pauseMenu.SetActive(false);
            gameState = prevState;
            prevState = STATE.PAUSED;
            isPaused = false;
            Debug.Log("gameState = " + gameState);
            Debug.Log("prevState = " + prevState);
            Time.timeScale = 1;
        }
    }

}
