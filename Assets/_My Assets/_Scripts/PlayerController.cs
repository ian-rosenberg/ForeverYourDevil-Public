using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AStarPathfinding;
using System;

/**
 * PlayerController - Script for player movement out-ofcombat and within combat
 * 
 * Authors - Omar Ilyas, Ian Rosenberg
 * 
 * Tutorials used: Brackys - Navmesh Tutorials - https://www.youtube.com/watch?v=CHV1ymlw-P8&t=11s
 */

public class PlayerController: MonoBehaviour
{
    private gameManager gameManager;

    public AStarNode combatPosition; // node representing the grid position of the player

    public CharacterPathfinding pathfinder;//pathfinding script

    public TileGrid grid; // the grid we are currently navigating

    public Animator anim; // animation controller for player

    [Tooltip("Specify what layer(s) to use in raycast")]
    public LayerMask layerMask;

    public Camera cam; /**Main camera to raycast to floor to determine if hit is possible*/
    NavMeshAgent agent; /**Player Agent Component for pathfinding movement*/
    public float normalSpeed, sprintSpeed;
    //public Rigidbody rb;

    public GameObject clickIndicator; //Has 2 particle effects, one for normal and one for turning off.
    public Animator clickIndicAnim;

    public List<Vector2Int> xzPath;

    public List<AStarNode> prevPath;//The last path to un-highlight
    public List<AStarNode> pathToTake;//The path that is returned by the pathfinding script

    public Vector3 nodeBattlePos; // node position of player on battlefield

    public LayerMask combatTravelLayerMask; // Only check for floor clicks, walls are no longer valid for combat

    private AStarNode selected;

    private bool autoMove = false;


    // Awake is called before start
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        gameManager = gameManager.Instance;
        pathfinder.SetObj(this.gameObject);

        selected = null;

        pathToTake = null;
        
        prevPath = pathToTake;
    }

    // Start is called before the first frame update
    void Start()
    {
        clickIndicator.SetActive(false);
        //agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //Traveling Movement
        if (gameManager.gameState == gameManager.STATE.TRAVELING)
        {
            if (!anim.GetBool("Traveling"))
            {
                anim.SetBool("Traveling", true);
                anim.SetTrigger("TravelingTrigger");
                anim.SetBool("Combat", false);
            }

            //Click to move (w/pathfinding)
            if (Input.GetMouseButtonDown(0)) //If left click (not hold)
            {
                //Determine if walkable
                Ray ray = cam.ScreenPointToRay(Input.mousePosition); //create ray obj from camera to click point
                RaycastHit hit;

                //If ray hit walkable area
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) //cast ray. if hit land, move
                {
                    //Check hit layer
                    if (hit.transform.gameObject.layer == 9) //If click ground
                    {
                        //Set indicator where clicked
                        clickIndicator.SetActive(true);
                        clickIndicAnim.SetTrigger("On");
                        clickIndicator.transform.position = hit.point + new Vector3(0, 2f, 0);

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
                clickIndicator.SetActive(false);
            }
        }

        else if (gameManager.gameState == gameManager.STATE.TALKING)
        {
            agent.ResetPath(); //Resets directions to agent to stop it
        }

        else if (gameManager.gameState == gameManager.STATE.COMBAT)
        {
            if (!anim.GetBool("Combat"))
            {
                anim.SetBool("Traveling", false);
                anim.SetTrigger("CombatTrigger");
                anim.SetBool("Combat", true);
            }

            //Click to move (w/pathfinding)
            if (Input.GetMouseButtonDown(0)) //If left click (not hold)
            {
                //Determine if walkable
                Ray ray = cam.ScreenPointToRay(Input.mousePosition); //create ray obj from camera to click point
                RaycastHit hit;

                //If ray hit walkable area
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) //cast ray. if hit land, move
                {
                    //Check hit layer
                    if (hit.transform.gameObject.layer == 9) //If click ground
                    {
                        //Set indicator where clicked
                        clickIndicator.SetActive(true);
                        clickIndicAnim.SetTrigger("On");

                        Vector3 gridPoint = grid.NearestGridPoint(hit.point);

                        clickIndicator.transform.position = gridPoint + new Vector3(0, 2f, 0);

                        //if (selected == null || selected != grid.NearestGridNode(hit.point))
                        //{
                            prevPath = pathToTake;

                            pathToTake = pathfinder.GetCharacterPath(combatPosition, grid.NearestGridNode(hit.point), grid);

                            if (pathToTake != prevPath)
                            {
                                grid.HighlightPath(pathToTake);

                                if (prevPath != null)
                                {
                                    grid.RemoveHighlightedPath(prevPath);
                                }

                                selected = grid.NearestGridNode(hit.point);
                            }
                       // }
                    }
                }

            }

            agent.speed = normalSpeed;
            //Set if anim is in run or idle (set by number in blend tree)
            anim.SetFloat("Speed", (agent.velocity.magnitude / agent.speed));
        }
    }

    public bool ToggleAutoMove()
    {
        return !autoMove;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ClickIndicator"))
        {
            agent.ResetPath(); //Stop agent if it hits indicator
            StartCoroutine(ClickOff());
        }
    }

    //Turn Click Indicator off (called by anim event)
    IEnumerator ClickOff()
    {
        clickIndicAnim.SetTrigger("Off");
        yield return new WaitForSeconds(0.25f);
        //clickIndicator.SetActive(false);

    }

    public Vector3 GetDefaultGridSpawn()
    {
        return grid.defaultSpawn;
    }
}
