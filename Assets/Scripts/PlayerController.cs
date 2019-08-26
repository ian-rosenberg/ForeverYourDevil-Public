//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/**
 * PlayerController - Script for player movement out-ofcombat and within combat
 * 
 * Author - Omar Ilyas
 * 
 * Tutorials used: Brackys - Navmesh Tutorials - https://www.youtube.com/watch?v=CHV1ymlw-P8&t=11s
 */

public class PlayerController : MonoBehaviour
{
    gameManager gameManager;

    public Animator anim;

    [Tooltip("Specify what layer(s) to use in raycast")]
    public LayerMask layerMask;

    public Camera cam; /**Main camera to raycast to floor to determine if hit is possible*/
    NavMeshAgent agent; /**Player Agent Component for pathfinding movement*/
    //public Rigidbody rb;

    public GameObject clickIndicator;

    // Awake is called before start
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        gameManager = gameManager.Instance;
        //rb = GetComponent<Rigidbody>();
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
            //Set if anim is in run or idle
            anim.SetFloat("Speed", agent.velocity.magnitude);

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
                        clickIndicator.transform.position = hit.point + new Vector3(0, 2f, 0);

                        //Move player/agent to hit point
                        agent.SetDestination(hit.point);
                    }
                }

            }
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
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ClickIndicator"))
        {
            agent.ResetPath(); //Stop agent if it hits indicator
            clickIndicator.SetActive(false);
        }
    }
}
