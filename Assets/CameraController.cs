using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public FollowObject followScript;
    GameObject player;

    public float rotateSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        player = followScript.obj;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1)) //If hold right click
        {
            transform.RotateAround(player.transform.position, Vector3.up, Input.GetAxis("Mouse X") *  rotateSpeed);
        }
    }
}
