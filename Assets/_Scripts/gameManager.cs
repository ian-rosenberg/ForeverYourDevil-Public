using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    bool canPause = true; //Allow pausing?

    public GameObject pauseMenu;
    public Animator battleAnim; //Canvas for battle transition
    public Image screenCapRegion; //Place to display image on
    public GameObject player; //Reference to player character

    public GameObject normalWorld; //Represents overworld
    public GameObject battleWorld; //Represents battlefield

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
    bool isPaused; //Is the game paused?
    STATE prevState; //Previous State of the game (before pausing)

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        prevState = STATE.START; //Start out of combat
    }

    // Update is called once per frame
    void Update()
    {
        //Pause Game
        if (Input.GetButtonDown("Pause"))
        {
            Debug.Log("Pressed Enter/Escape");
            if (isPaused) {
                pauseMenu.SetActive(false);
                UnPauseGame(); }
            else {
                pauseMenu.SetActive(true);
                PauseGame(); }
        }

    }

    public void PauseGame()
    {
        if (!isPaused && canPause)
        {
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
        if (isPaused && canPause)
        {
            gameState = prevState;
            prevState = STATE.PAUSED;
            isPaused = false;
            Debug.Log("gameState = " + gameState);
            Debug.Log("prevState = " + prevState);
            Time.timeScale = 1;
        }
    }

    public void SetCanPause(bool pause)
    {
        canPause = pause;
    }

    public void TriggerCombat(GameObject enemy)
    {
        // StartCoroutine(ScreenCap());
        Debug.Log("Triggering Combat");
        PauseGame();
        SetCanPause(false);
        battleAnim.SetTrigger("Battle");
    }

    IEnumerator ScreenCap()
    {
        {
            yield return new WaitForEndOfFrame();
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            Sprite sprite = Sprite.Create(texture,
                new Rect(0, 0, Screen.currentResolution.width,
                Screen.currentResolution.height),
                new Vector2(0, 0)
                );

            // do something with texture
            screenCapRegion.sprite = sprite;

            // cleanup
            Object.Destroy(texture);
        }
    }
}
