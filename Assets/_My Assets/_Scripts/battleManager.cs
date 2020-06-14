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
    public PartyMember pm;
    private bool playerTurn = false;
    public GameObject respawn;
    public battleState currentState;
    private battleState prevState;
    public List<AStarNode> path; //current path - x == x, y == z
    public List<AStarNode> prevPath; //The last path to un-highlight - x == x, y == z
    private float elapsedTime = 0;
    private float waitTime = 0.25f;
    public Vector3 offset = Vector3.one / 2;
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

    private void Awake()
    {
        gm = gameManager.Instance;
    }

    // Start is called before the first frame update
    private void Start()
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

    private void Update()
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
            BroadcastMessage("ChangedBattleStateTo", state);
            Debug.Log("new combat state: " + currentState);
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
            if (Input.GetKeyDown(KeyCode.F))
            {
                end_combat();
            }

            yield return null;
        }

        Debug.Log("go to next turn");

        StartCoroutine(enemy_turn());
        yield return null;
    }

    /*if player turn && keycode space; start enemy coroutine*/

    private IEnumerator enemy_turn()
    {
        enemy.agent.enabled = true;
        Debug.Log("enemy");
        ChangeBattleState(battleState.ENEMY_TURN);

        //enemy.transform.position = new Vector3(10, 0, 0);
        prevPath = path;
        path = enemy.enemyPathfinder.AStarSearch(enemy.enemyPathfinder.grid.NearestGridNode(enemy.transform.position), player.pathfinder.grid.NearestGridNode(player.GetComponent<Transform>().position));
        Debug.Log("path count=" + path.Count);
        Debug.Log("player position = " + player.transform.position);
        Vector2Int newGridPos = new Vector2Int(path[path.Count - 1].gridZ + (int)Random.Range(-1f, 1f), path[path.Count - 1].gridX + (int)Random.Range(-1f, 1f));
        Vector3 goalPos = Vector3.one;
        foreach (AStarNode node in path)
        {
            Debug.Log("path" + node.gridX + "," + node.gridZ);
        }
        bool flag = false;
        for (int i = 1; i < path.Count; i++)
        {
            Debug.Log("node=" + path[i].worldPosition);
            Debug.Log(path);
            //enemy.agent.ResetPath();
            enemy.agent.SetDestination(path[i].worldPosition);
            if (i == path.Count - 1)
            {
                goalPos = player.pathfinder.grid.nodeGrid[newGridPos.x, newGridPos.y].worldPosition;
                flag = true;
            }
            if (flag)
            {
                while (elapsedTime < waitTime)
                {
                    transform.position = Vector3.Lerp(enemy.transform.position, path[1].worldPosition - offset, (elapsedTime / waitTime));
                    elapsedTime += Time.deltaTime;

                    // Yield here
                    yield return null;
                }
            }
        }
        yield return new WaitForSecondsRealtime(2f);
        enemy.agent.enabled = false;
        player.ChangeStamina(3, 9);
        StartCoroutine(player_turn());

        //Debug.Log("enemy turn");
    }

    public void end_combat()
    {
        gm.normalWorld.SetActive(true);
        gm.battleWorld.SetActive(false);

        player.agent.ResetPath();
        player.agent.enabled = false;
        player.transform.position = respawn.transform.position;
        gm.ChangeState(gameManager.STATE.TRAVELING);
        gm.mainCamera.followScript.TravelOffset(gm.mainCamera.followScript.startOffset);
        ///player.currentBehavior = player.Player_Travelling;
        player.transform.position = respawn.transform.position;
        player.agent.enabled = true;
    }
}

/*while not playerturn
 *  yield return null;
 *  when player turn ==true
 *  do something. switch turn to enemy
 *  */

/*while the player has stamina and has not hit a button, is player turn*/