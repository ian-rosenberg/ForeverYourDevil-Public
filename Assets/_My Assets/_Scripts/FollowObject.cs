using UnityEngine;

/**
 * FolowObject.cs - Have script attached obj follow the specified axis/axies of a given obj (x,y,z)
 * Applies for global position.
 *
 * Author: Omar Ilyas
 *
 */

public class FollowObject : MonoBehaviour
{
    [Tooltip("Gameobject to follow")]
    public GameObject target;

    [Tooltip("Does this follow obj's x position?")]
    public bool x_pos;

    [Tooltip("Does this follow obj's y position?")]
    public bool y_pos;

    [Tooltip("Does this follow obj's z position?")]
    public bool z_pos;

    public Vector3 offset;

    private Vector3 new_pos; /**New position for this obj*/

    // Start is called before the first frame update
    private void Start()
    {
        offset = transform.position - target.transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        //Position
        if (x_pos) new_pos += new Vector3(target.transform.position.x, 0, 0);
        if (y_pos) new_pos += new Vector3(0, target.transform.position.y, 0);
        if (z_pos) new_pos += new Vector3(0, 0, target.transform.position.z);

        //Apply final vectors
        transform.position = Vector3.Lerp(transform.position, new_pos + offset, 0.9f);

        //Reset new_pos
        new_pos = Vector3.zero;
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset - target.transform.position;
    }
}