using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum battleState { START, AMBUSH, ENEMY_TURN, PLAYER_TURN };
//CHANGE STATE WITH THE GAME MANAGER
public class battleManager : MonoBehaviour
{
    public EnemyController enemy; //Reference to enemy script
    public PlayerController player; //Reference to player script
    public gameManager gm; //reference to gameManager for player states
    private bool playerTurn=false;
    public battleState currentState;
    battleState prevState;
    
    private static battleManager instance;
    public static battleManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<battleManager>();
            return instance;
        }
    }
    void Awake()
    {
        gm = gameManager.Instance;

    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("battle manager loaded");

       
    }
    /*
     MUST SET THE TURN STATE ONCE, BUT ONLY WHEN COMBAT STARTS(COMBAT FUNCTION?)
     IF IT IS IN THE UPDATE FUNCTION, THE PLAYER'S TURN WILL BE RESET CONSTANTLY
     HAVE TO ONLY UPDATE WHEN CERTAIN CONDITIONS ARE MET, BUT ALSO UPDATE FREQUENTLY ENOUGH TO CHANGE TO COMBAT
     CHANGE STATE ON TRIGGER?

         */
    // Update is called once per frame

    

    void Update()
    {
        if (gm.gameState == gameManager.STATE.COMBAT)
        {
            //StartCoroutine(player_turn());
            //Debug.Log("running manager");
            //combat();
            //return;

        }
        




    }

    public void ChangedStateTo(gameManager.STATE newState)
    {
        switch (newState)
        {
            case gameManager.STATE.START:

                break;
            case gameManager.STATE.TRAVELING:

                break;
            case gameManager.STATE.COMBAT:
                playerTurn = true;
                StartCoroutine(player_turn());
                break;
            case gameManager.STATE.PAUSED:

                break;
            case gameManager.STATE.TALKING:

                break;
            default:
                Debug.Log("how did i get here?");
                break;
        }
    }

    public void ChangeBattleState(battleState state)
    {
        if (state != currentState && state != battleState.START) //Make sure state is not a duplicate
        {
            //If state is valid, change it.
            prevState = currentState;
            currentState = state;

            //Send message to dependant components within GameManager
            BroadcastMessage("ChangedStateTo", state);
            Debug.Log("new combat state: "+currentState);
        }
    }

    public IEnumerator player_turn()
    {
        Debug.Log("player");
        ChangeBattleState(battleState.PLAYER_TURN);
        while (player.stamina >= 1)
        {
            //Debug.Log("has stamina");
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(enemy_turn());
                yield return null;
            }

            
            yield return null;

        }


        Debug.Log("go to next turn");

        StartCoroutine(enemy_turn());
        yield return null;

    }
    
    
    /*if player turn && keycode space; start enemy coroutine*/
    IEnumerator enemy_turn()
    {
        Debug.Log("enemy");
        ChangeBattleState(battleState.ENEMY_TURN);
        yield return new WaitForSecondsRealtime(10f);
        player.ChangeStamina(3, 9);
        StartCoroutine(player_turn());
        

        //Debug.Log("enemy turn");
    }
}
/*while not playerturn
 *  yield return null;
 *  when player turn ==true
 *  do something. switch turn to enemy
 *  */

    /*while the player has stamina and has not hit a button, is player turn*/
