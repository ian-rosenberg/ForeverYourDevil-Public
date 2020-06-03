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
    [Tooltip("Transform of the gameobject to follow")]
    public Transform target;

    [Tooltip("Does this follow obj's x position?")]
    public bool x_pos;

    [Tooltip("Does this follow obj's y position?")]
    public bool y_pos;

    [Tooltip("Does this follow obj's z position?")]
    public bool z_pos;

    public Vector3 offset;

    private Vector3 new_pos; /**New position for this obj*/

    [Header("DEBUG")]
    [Range(0f, 1f)]
    public float lagTimer = 0.9f; //Camera lagging. 1 = no lag, 0 = stationary

    // Start is called before the first frame update
    private void Start()
    {
        offset = transform.position - target.transform.position;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //Position
        if (x_pos) new_pos += new Vector3(target.transform.position.x, 0, 0);
        if (y_pos) new_pos += new Vector3(0, target.transform.position.y, 0);
        if (z_pos) new_pos += new Vector3(0, 0, target.transform.position.z);

        //Apply final vectors
        transform.position = Vector3.SlerpUnclamped(transform.position, new_pos + offset, lagTimer);

        //Reset new_pos
        new_pos = Vector3.zero;
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset - target.transform.position;
    }
}