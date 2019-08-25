using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightRotator : MonoBehaviour
{
    [Tooltip("Speed at which light rotates at")]
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, -speed);
    }
}
