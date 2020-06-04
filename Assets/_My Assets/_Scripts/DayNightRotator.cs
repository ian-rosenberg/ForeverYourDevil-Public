using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * @brief Directional lightsource rotator to emulate time of day.
 * @author Omar Ilyas
 * @Depreciated
 */

public class DayNightRotator : MonoBehaviour
{
    [Tooltip("Speed at which light rotates at")]
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, -speed);
    }
}
