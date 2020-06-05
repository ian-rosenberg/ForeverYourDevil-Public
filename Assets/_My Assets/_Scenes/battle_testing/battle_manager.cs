using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum battleState {AMBUSH, ENEMY_TURN, PLAYER_TURN};
//CHANGE STATE WITH THE GAME MANAGER
public class battle_manager : MonoBehaviour
{
    public EnemyController enemy; //Reference to enemy script
    public PlayerController player; //Reference to player script
    public gameManager gm; //reference to gameManager for player states
    public battleState battleState;
    battleState prevState;

    private static battle_manager instance;
    public static battle_manager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<battle_manager>();
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

        StartCoroutine(battleStart());
    }
    /*
     MUST SET THE TURN STATE ONCE, BUT ONLY WHEN COMBAT STARTS(COMBAT FUNCTION?)
     IF IT IS IN THE UPDATE FUNCTION, THE PLAYER'S TURN WILL BE RESET CONSTANTLY
     HAVE TO ONLY UPDATE WHEN CERTAIN CONDITIONS ARE MET, BUT ALSO UPDATE FREQUENTLY ENOUGH TO CHANGE TO COMBAT
     CHANGE STATE ON TRIGGER?

         */
    // Update is called once per frame

    IEnumerator battleStart()
    {
        if (gm.gameState == gameManager.STATE.COMBAT)
        {
            StartCoroutine(player_turn());
            //Debug.Log("running manager");
            //combat();
            //return;
        }
        else
        {
            yield return new WaitForSecondsRealtime(10f);
            StartCoroutine(battleStart());
        }

    }
    
    void Update()
    {
        if(gm.gameState == gameManager.STATE.COMBAT)
        {
            //StartCoroutine(player_turn());
            //Debug.Log("running manager");
            //combat();
            //return;

        }
        if (battleState == battleState.PLAYER_TURN)
        {
            //Debug.Log("PLAYER turn");
        }

       

        
    }

    public void ChangeState(battleState state)
    {
        
        switch (state)
        {
            case battleState.PLAYER_TURN:
                player.currentBehavior = player.Player_Combat;
                break;
            case battleState.ENEMY_TURN:
                enemy.currentBehavior = enemy.Enemy_Combat;
                break;
            case battleState.AMBUSH:
                Debug.LogError("Ambush to be added later");
                break;
            default:
                Debug.LogError("Invalid State Change.");
                return;
            }
            //If state is valid, change it.
            prevState = battleState;
            battleState = state;
    }

    IEnumerator player_turn()
    {
        Debug.Log("player");
        ChangeState(battleState.PLAYER_TURN);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(enemy_turn());
            
        }
        else
        {
            StartCoroutine(player_turn());
        }

        yield return new WaitForSecondsRealtime(10f);

    }

    IEnumerator enemy_turn()
    {
        Debug.Log("enemy");
        ChangeState(battleState.ENEMY_TURN);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(player_turn());
            
        }
        else
        {
            StartCoroutine(enemy_turn());
        }
        yield return new WaitForSecondsRealtime(10f);
        //Debug.Log("enemy turn");
    }
}
