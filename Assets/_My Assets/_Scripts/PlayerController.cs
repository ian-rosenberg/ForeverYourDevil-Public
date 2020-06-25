using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * PlayerController - Script for player movement out-ofcombat and within combat
 *
 * Authors - Omar Ilyas, Ian Rosenberg
 *
 * Tutorials used: Brackys - Navmesh Tutorials - https://www.youtube.com/watch?v=CHV1ymlw-P8&t=11s
 */

public class PlayerController : PartyMember
{
    [Header("Player Combat")]
    public TileGrid grid; // the grid we are currently navigating

    private bool combatMoving;

    [Header("Inventory Management")]
    private InventoryManagement invManager;

    //Pathfinding
    [Header("AStar Pathfinding")]
    public AStarNode combatPosition; // node representing the grid position of the player

    public Vector3 nodeBattlePos; // node position of player on battlefield
    public CharacterPathfinding pathfinder;//pathfinding script

    public List<AStarNode> path; //current path - x == x, y == z
    public List<AStarNode> prevPath; //The last path to un-highlight - x == x, y == z
    public List<AStarNode> lockedPath; //The path to walk along on click - x == x, y == z

    private AStarNode selected;

    [Header("Behavior")]
    [SerializeField]
    private bool canPickup = false;

    public Action currentBehavior; //Function pointer for player behavior (changed by gameManager)

    private bool sprint = false;

    [Header("Player Actions")]
    public PlayerControls pControls;

    private Vector2 axes;

    //Singleton creation
    private static PlayerController instance;

    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PlayerController>();
            return instance;
        }
    }

    private void OnEnable()
    {
        pControls = new PlayerControls();

        pControls.Player.LeftClick.performed += AutoTravel;
        pControls.Player.LeftClick.performed += CombatTravel;
        pControls.Player.Use.performed += Pickup;
        pControls.Player.Sprint.performed += Sprint;
        pControls.Player.ManualTravel.performed += context => axes = context.ReadValue<Vector2>();
        pControls.Player.ManualTravel.canceled += context => axes = Vector2.zero;

        pControls.Player.LeftClick.Enable();
        pControls.Player.Sprint.Enable();
        pControls.Player.ManualTravel.Enable();
        pControls.Player.Use.Enable();
    }

    public void Pickup(InputAction.CallbackContext obj)
    {
        StartCoroutine(PickupItem());
    }

    private void OnDisable()
    {
        pControls.Player.LeftClick.performed -= AutoTravel;
        pControls.Player.LeftClick.performed -= CombatTravel;
        pControls.Player.Use.performed -= Pickup;
        pControls.Player.Sprint.started -= Sprint;
        pControls.Player.ManualTravel.performed -= context => axes = context.ReadValue<Vector2>();

        pControls.Player.LeftClick.Disable();
        pControls.Player.Sprint.Disable();
        pControls.Player.ManualTravel.Disable();
        pControls.Player.Use.Disable();
    }

    // Awake is called before start
    private void Awake()
    {
        selected = null;

        combatMoving = false;

        prevPath = null;
        prevPath = path;

        gameManager = gameManager.Instance;
        invManager = InventoryManagement.Instance;

        currentBehavior = Player_Travelling;
    }

    private void Update()
    {
        
        //Apply current behavior
        currentBehavior();

        anim.SetFloat("Speed", (agent.velocity.magnitude / sprintSpeed));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ClickIndicator"))
        {
            agent.ResetPath(); //Stop agent if it hits indicator
            StartCoroutine(gameManager.Instance.ClickOff());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.GetComponent<ItemDropped>() as ItemDropped) != null && canPickup)
        {
            ItemBase item = other.GetComponent<ItemDropped>().GetItem();

            Debug.Log("Picked up " + item.Name);

            invManager.sharedInventory.SetActive(true);
            invManager.GetComponentInChildren<SharedInventory>().AddSingleItem(item);
            invManager.sharedInventory.SetActive(false);

            Destroy(other.gameObject);

            canPickup = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.GetComponent<ItemDropped>() as ItemDropped) != null)
        {
            canPickup = false;
        }
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        sprint = !sprint;

        if (sprint)
            agent.speed = sprintSpeed;
        else
            agent.speed = normalSpeed;
    }

    public Vector3 GetDefaultGridSpawn()
    {
        return grid.defaultSpawn;
    }

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

        //Manual WASD/Gamepad Travel
        float h = axes.x;
        float v = axes.y;
       // Debug.Log("<color=blue>H: " + h + ", V: " + v + "</color>");

        if (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0)
        {
            if (!agent.isStopped)
                agent.ResetPath();

            ////Set Global Direction With Camera
            agent.velocity = ((Camera.main.transform.forward * v) + (Camera.main.transform.right * h)) * agent.speed;

            gameManager.clickIndicator.SetActive(false);
        }
    }

    public void AutoTravel(InputAction.CallbackContext context)
    {
        if (gameManager.gameState != gameManager.STATE.TRAVELING)
            return;

        //Automatic travel via Input System
        if (!anim.GetBool("Traveling"))
        {
            anim.SetBool("Traveling", true);
            anim.SetTrigger("TravelingTrigger");
            anim.SetBool("Combat", false);
        }
        anim.SetBool("StayIdle", false);

        //Determine if walkable
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //create ray obj from camera to click point
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

    public void CombatTravel(InputAction.CallbackContext context)
    {
        //If click, show indicator and move character (accounting for stamina)
        if (gameManager.gameState != gameManager.STATE.COMBAT)
            return;

        //Determine if walkable
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //create ray obj from camera to click point
        RaycastHit hit;

        //If ray hit walkable area
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) //cast ray. if hit land, move
        {
            //Check hit layer
            if (hit.transform.gameObject.layer == 9) //If click ground
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

        agent.speed = normalSpeed;
        //Set if anim is in run or idle (set by number in blend tree
    }

    public void Player_Combat()
    {
        //Determine if walkable
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //create ray obj from camera to click point
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
            }
        }

        agent.speed = normalSpeed;
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
                    playerGUI.ChangeStamina(stamina, maxStamina);

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

        if (newState != gameManager.STATE.TRAVELING)
        {
            pControls.Player.Use.performed -= Pickup;

            if (newState == gameManager.STATE.COMBAT)
            {
                pControls.Player.Sprint.started -= Sprint;
                pControls.Player.ManualTravel.performed -= context => axes = context.ReadValue<Vector2>();
            }
        }
        else
        {
            pControls.Player.Use.performed += Pickup;
            pControls.Player.Sprint.started += Sprint;
            pControls.Player.ManualTravel.performed += context => axes = context.ReadValue<Vector2>();
        }
    }

    private IEnumerator PickupItem()
    {
        canPickup = true;

        yield return new WaitForSeconds(1f);

        canPickup = false;
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

    #endregion Player Behavior Functions
}