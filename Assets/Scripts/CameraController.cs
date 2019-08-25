using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public FollowObject followScript;
    GameObject player;

    public float rotateSpeed;
    public float cameraResetSpd;

    Vector3 orig_pos;
    Quaternion orig_rot;

    bool isCameraReseting;

    // Start is called before the first frame update
    void Awake()
    {
        player = followScript.obj;
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializeCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Camera Reset") && !isCameraReseting)
        {
            StartCoroutine(ResetCamera());
        }

        if (Input.GetMouseButton(1)) //If hold right click
        {
            transform.RotateAround(player.transform.position, Vector3.up, Input.GetAxis("Mouse X") * rotateSpeed);
        }
    }

    /**
     * @brief Set default values of the camera at start of game (or start of scene)
     */
    void InitializeCamera()
    {
        orig_pos = transform.localPosition; // Set default pos to current pos
        orig_rot = transform.localRotation; // Set default rot to current rot
        isCameraReseting = false;
    }

    /**
     * @brief Reset camera to original position smoothly
     */
    public IEnumerator ResetCamera()
    {
        isCameraReseting = true;
        while (transform.localPosition != orig_pos && transform.localRotation != orig_rot)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, orig_pos, cameraResetSpd);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, orig_rot, cameraResetSpd);
            yield return null; //advance frame
        }
        isCameraReseting = false;
    }
}
