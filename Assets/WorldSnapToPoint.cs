using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSnapToPoint : MonoBehaviour
{

    [Tooltip("Specify what layer(s) to use in raycast")]
    public LayerMask layerMask;
    public Camera cam; /**Main camera to raycast to floor to determine if hit is possible*/
    public GameObject block, sphere;
    public grid grid;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Raycast to see if hit the grid
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); //create ray obj from camera to click point
        RaycastHit hit;

        //If ray hit walkable area
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) //cast ray. if hit land, move
        {

            //Get Grid Position
            Vector3 gridPoint = grid.NearestGridPoint(hit.point);

            //Place indicator
            sphere.transform.position = hit.point;
            block.transform.position = Vector3.Lerp(block.transform.position, gridPoint + new Vector3(0, 0.5f, 0), 0.5f);
            Debug.Log(gridPoint);
            //Check hit layer
            //if (hit.transform.gameObject.layer == 9) //If click ground
            //{
            //}
        }
    }
}
