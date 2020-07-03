
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PartyMember : MonoBehaviour
{
    [Header("Player")]
    public Animator anim; // animation controller for player

<<<<<<< HEAD
    public Action currentBehavior; //Function pointer for player behavior (changed by gameManager)
=======
>>>>>>> Rebuilding inventory
    public int health = 100; //Current Health of the player; 0 kills player

    public int maxHealth = 100; //Max Health the player is allowed to heal to
    public int tolerance = 25; //Current Tolerance. Max will kill the player
    public int maxTolerance = 100; //Max Tolerance before player is killed
    public int stamina = 6; // Current Stamina, allows player to do actions
    public int maxStamina = 6; //Max stamina player is allowed to have
    public PlayerGUI playerGUI; //Gui menu of the player

    [Tooltip("Specify what layer(s) to use in raycast")]
    public LayerMask layerMask;

    public NavMeshAgent agent; /**Player Agent Component for pathfinding movement*/
    public float normalSpeed, sprintSpeed;
    public Rigidbody rb;

    [Header("Game Manager")]
    public gameManager gameManager;

<<<<<<< HEAD
    //Pathfinding
    [Header("AStar Pathfinding")]
    public AStarNode combatPosition; // node representing the grid position of the player

    public Vector3 nodeBattlePos; // node position of player on battlefield
    public CharacterPathfinding pathfinder;//pathfinding script

    private AStarNode selected;
    public List<AStarNode> path; //current path - x == x, y == z
    public List<AStarNode> prevPath; //The last path to un-highlight - x == x, y == z
    public List<AStarNode> lockedPath; //The path to walk along on click t - x == x, y == z
    private bool autoMove = false;

    [Header("Player Combat")]
    private bool combatMoving;

    public TileGrid grid; // the grid we are currently navigating
=======

    [Header("Inventory")]
    [SerializeField]
    protected Inventory shared;
    protected Inventory personal;

>>>>>>> Rebuilding inventory

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
<<<<<<< HEAD

        selected = null;

        combatMoving = false;

        prevPath = path;
=======
>>>>>>> Rebuilding inventory
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeHealth(health, maxHealth);
        ChangeStamina(stamina, maxStamina);
        ChangeTolerance(tolerance, maxTolerance);
    }

    // Update is called once per frame
<<<<<<< HEAD
    private void Update()
    {
        //Apply current behavior
        currentBehavior();
    }

    public bool ToggleAutoMove()
    {
        return !autoMove;
    }
=======
>>>>>>> Rebuilding inventory

    #region Player Stats

    public void ChangeHealth(int newHealth, int newMaxHealth)
    {
        health = newHealth;
        maxHealth = newMaxHealth;
        playerGUI.ChangeHealth(newHealth, newMaxHealth);
    }

    public void ChangeTolerance(int newTolerance, int newMaxTolerance)
    {
        tolerance = newTolerance;
        maxTolerance = newMaxTolerance;
        playerGUI.ChangeTolerance(newTolerance, newMaxTolerance);
    }

    public void ChangeStamina(int newStamina, int newMaxStamina)
    {
        stamina = newStamina;
        maxStamina = newMaxStamina;
        playerGUI.ChangeStamina(newStamina, newMaxStamina);
    }

    #endregion Player Stats

<<<<<<< HEAD
    #region Player Behavior Functions
    public void Player_Travelling()
    {
        //Set animator (need to change)
        if (!anim.GetBool("Traveling"))
        {
            anim.SetBool("Traveling", true);
            anim.SetTrigger("TravelingTrigger");
            anim.SetBool("Combat", false);
        }
        anim.SetBool("StayIdle", false);

        //Click to move (w/pathfinding)
        if (Input.GetMouseButtonDown(0)) //If left click (not hold)
        {
            //Determine if walkable
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //create ray obj from camera to click point
            RaycastHit hit;

            //If ray hit walkable area
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) //cast ray. if hit land, move
            {
                //Check hit layer
                if (hit.transform.gameObject.layer == 9) //If click ground
                {
                    gameManager.clickIndicator.SetActive(true);
                    gameManager.clickIndicAnim.SetTrigger("On");
                    gameManager.clickIndicator.transform.position = hit.point + new Vector3(0, 2f, 0);

                    //Move player/agent to hit point
                    agent.SetDestination(hit.point);
                }
            }
        }
        //Right click to sprint
        if (Input.GetButton("Sprint"))
        {
            agent.speed = sprintSpeed;
            //Set if anim is in run or idle (set by number in blend tree)
            anim.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
        }
        else
        {
            agent.speed = normalSpeed;
            //Set if anim is in run or idle (set by number in blend tree)
            anim.SetFloat("Speed", (agent.velocity.magnitude / agent.speed) * .5f);
        }

        //Keyboard movement
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(vertical) > 0 || Mathf.Abs(horizontal) > 0)
        {
            agent.ResetPath();
            agent.velocity = new Vector3(horizontal, 0, vertical) * agent.speed;
            gameManager.clickIndicator.SetActive(false);
        }
    }

    public void Player_Combat()
    {
        if (!anim.GetBool("Combat"))
        {
            anim.SetBool("StayIdle", false);
            anim.SetBool("Traveling", false);
            anim.SetTrigger("CombatTrigger");
            anim.SetBool("Combat", true);
        }
        //Click to move (w/pathfinding)

        //Determine if walkable
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //create ray obj from camera to click point
        RaycastHit hit;

        //If ray hit walkable area
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) //cast ray. if hit land, move
        {
            //Check hit layer
            if (hit.transform.gameObject.layer == 9) //If click ground
            {
                //if (selected == null || selected != grid.NearestGridNode(hit.point))
                //{
                prevPath = path;

                path = pathfinder.AStarSearch(grid.NearestGridNode(transform.position), grid.NearestGridNode(hit.point));

                if (path != prevPath && !combatMoving)
                {
                    grid.RemoveHighlights();
                    grid.HighlightPath(path, stamina);

                    selected = grid.NearestGridNode(hit.point);
                }

                //If click, show indicator and move character (accounting for stamina)
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject clickIndicator = gameManager.Instance.clickIndicator;
                    
                    //Show indicator
                    clickIndicator.SetActive(true);
                    gameManager.Instance.clickIndicAnim.SetTrigger("On");

                    Vector3 gridPoint = grid.NearestGridNode(hit.point).worldPosition;
                    clickIndicator.transform.position = gridPoint + new Vector3(0, 2f, 0);

                    //Move character along path
                    if (!combatMoving)
                    {
                        lockedPath = path;
                        combatMoving = true;
                        StartCoroutine(CombatMove());
                    }
                }
            }
        }

        agent.speed = normalSpeed;
        //Set if anim is in run or idle (set by number in blend tree)
        anim.SetFloat("Speed", (agent.velocity.magnitude / agent.speed));
    }

    public IEnumerator CombatMove()
    {
        //If path is greater than stamina, do not move
        if (lockedPath.Count > stamina + 1 || lockedPath.Count <= 0)
        {
            Debug.Log("<color=red>lockedPath.Count = " + lockedPath.Count + "</color>");
            Debug.Log("<color=red>Cannot move here. Not enough Stamina!</color>");
            combatMoving = false;
            yield return null;
        }
        else
        {
            Debug.Log("CombatMoving");
            int i = 1;
            //Move along path with NavMesh
            while (i < lockedPath.Count)
            {
                agent.SetDestination(lockedPath[i].worldPosition);
                yield return null;
                //If reached destination node (with some forgiveness)
                if (Math.Abs(transform.position.x - lockedPath[i].worldPosition.x) < 2f && Math.Abs(transform.position.z - lockedPath[i].worldPosition.z) < 2f)
                {
                    Debug.Log("Player position: " + transform.position);
                    Debug.Log("Node position: " + lockedPath[i].worldPosition);
                    Debug.Log("Player to Node distance= " + Vector3.Distance(transform.position, lockedPath[i].worldPosition));

                    Debug.Log("Arrived at node: " + i);
                    i++; //Go to next node
                    stamina--; //Subtract stamina and text update

                    //agent.ResetPath();
                }
            }

            //When movement is done
            Debug.Log("agent Reset. This shouldn't happen until the clicked node is reached");
            agent.ResetPath();
            combatMoving = false;
        }
    }

    public void ChangedStateTo(gameManager.STATE newState)
    {
        switch (newState)
        {
            case gameManager.STATE.START:
                Debug.LogError("Cannot switch GM State to START. This should not happen.");
                break;
            case gameManager.STATE.TRAVELING:
                currentBehavior = Player_Travelling;
                break;
            case gameManager.STATE.COMBAT:
                currentBehavior = Player_Combat;
                break;
            case gameManager.STATE.PAUSED:
                currentBehavior = Player_Paused;
                break;
            case gameManager.STATE.TALKING:
                currentBehavior = Player_Talking;
                break;
        }
    }

    public void Player_Talking()
    {
        agent.ResetPath(); //Resets directions to agent to stop it
        anim.SetBool("StayIdle", true);
    }


    public void Player_Paused()
    {
        //Disable Colliders, Rigidbodies, etc.
    }
    #endregion
=======
>>>>>>> Rebuilding inventory
}