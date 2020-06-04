using UnityEngine;

/**
 * FolowObject.cs
 * 
 * Have script attached obj follow the specified axis/axies of a given obj (x,y,z)
 * Applies for global position.
 * 
 * Author: Omar Ilyas
 * 
 */

public class FollowObject : MonoBehaviour
{
    [Tooltip("Gameobject to follow")]
    public GameObject obj;

    [Tooltip("Does this follow obj's x position?")]
    public bool x_pos;
    [Tooltip("Does this follow obj's y position?")]
    public bool y_pos;
    [Tooltip("Does this follow obj's z position?")]
    public bool z_pos;

    public Vector3 offset;

    Vector3 new_pos; /**New position for this obj*/

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - obj.transform.position;


        //Warning if none selected
        if (!x_pos && !y_pos && !z_pos)
            Debug.LogWarning(gameObject.name + " following " + obj.name + " has no axies selected. Consider removing this script from " + gameObject.name + ".");
    }

    // Update is called once per frame
    void Update()
    {
        //Current position = offset

        //Position
        if (x_pos) new_pos += new Vector3(obj.transform.position.x, 0, 0);
        if (y_pos) new_pos += new Vector3(0, obj.transform.position.y, 0);
        if (z_pos) new_pos += new Vector3(0, 0, obj.transform.position.z);

        //Apply final vectors
        transform.position = new_pos + offset;

        //Reset new_pos
        new_pos = Vector3.zero;
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset - obj.transform.position;
    }
}
