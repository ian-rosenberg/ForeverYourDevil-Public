using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSnapToPoint : MonoBehaviour
{

    [Tooltip("Specify what layer(s) to use in raycast")]
    public LayerMask layerMask;
    public Camera cam; /**Main camera to raycast to floor to determine if hit is possible*/
    public GameObject block;
    public TileGrid grid;
    gameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameManager.Instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameManager.gameState == gameManager.STATE.COMBAT)
        {
            block.gameObject.SetActive(true);

            //Raycast to see if hit the grid
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //create ray obj from camera to click point
            RaycastHit hit;

            //If ray hit walkable area
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) //cast ray. if hit land, move
            {
                
                Debug.DrawLine(cam.ScreenToWorldPoint(Input.mousePosition), hit.point, Color.green);
                //Get Grid Position
                Vector3 gridPoint = grid.NearestGridPoint(hit.point);

                //Place indicator
                block.transform.position = Vector3.Lerp(block.transform.position, gridPoint + grid.bounds.center, 0.5f);

            }
        }
        else block.SetActive(false);
    }
}
