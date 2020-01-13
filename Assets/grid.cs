using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tutorial Used: https://www.youtube.com/watch?v=VBZFYGWvm4A
public class grid : MonoBehaviour
{
    public float size = 1f;
    public GameObject gridThing;
    //Get nearest grid point to whichever position is specified (ideally mouse position)
    public Vector3 NearestGridPoint(Vector3 position)
    {
        //Subtract offset from math, then add it back in
        position -= transform.position;

        //Get the point closest to where your mouse is via rounding
        int xCount = Mathf.RoundToInt(position.x / size);
        int yCount = Mathf.RoundToInt(position.y / size);
        int zCount = Mathf.RoundToInt(position.z / size);

        //Form vector with this point and separate by size
        Vector3 result = new Vector3(
            (float)xCount * size,
            (float)yCount * size,
            (float)zCount * size);

        //Add offset back
        return result += transform.position;
    }

    //Draw the grid in Editor. 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (float x = 0; x < 40; x += size)
        {
            for (float z = 0; z < 40; z += size)
            {
                var point = NearestGridPoint(new Vector3(transform.position.x + x, 0f, transform.position.z + z));
                //Gizmos.DrawSphere(point, size * 0.1f);
                Gizmos.DrawWireCube(point, new Vector3(size * 1, 0f, size * 1));
            }
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        makeGrid();
    }

    // Update is called once per frame
    void makeGrid()
    {
        for (float x = 0; x < 40; x += size)
        {
            for (float z = 0; z < 40; z += size)
            {
                var point = NearestGridPoint(new Vector3(transform.position.x + x, 0f, transform.position.z + z));
                //Gizmos.DrawSphere(point, size * 0.1f);
                Instantiate(gridThing, point,gridThing.transform.rotation);
            }
        }

    }
}
