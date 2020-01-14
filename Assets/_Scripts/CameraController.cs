using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Camera controls
    public FollowObject followScript;
    public Transform FollowY, FollowX;
    GameObject player;

    public float rotateSpeed;
    public float cameraResetSpd;
    float zoom = 0f;

    Quaternion orig_rot_x, orig_rot_y;
    Vector3 orig_zoom;
    float mouseX, mouseY;

    bool isCameraReseting;

    public GameObject ResetCameraNotification;

    // Start is called before the first frame update
    void Awake()
    {
        player = followScript.obj;
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializeCamera();
        ResetCameraNotification.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        //TRAVELING CAMERA
        if (Input.GetButtonDown("Camera Reset") && !isCameraReseting)
        {
            StartCoroutine(ResetCamera());
        }

        //Move camera with right click and hold
        if (Input.GetMouseButton(1) && !isCameraReseting) //If hold right click
        {
            ResetCameraNotification.SetActive(true);
            mouseX += Input.GetAxis("Mouse X") * rotateSpeed;
            mouseY += Input.GetAxis("Mouse Y") * rotateSpeed;

            mouseY = Mathf.Clamp(mouseY, -30, 45);

            FollowX.rotation = Quaternion.Euler(0f, mouseX, 0f);
            FollowY.localRotation = Quaternion.Euler(-mouseY, 0f, 0f);

            //transform.RotateAround(player.transform.position, Vector3.up, mouseX);
            // FollowY.RotateAround(player.transform.position, transform.right, mouseY);
        }

        //Zoom in and out with mouse wheel.
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            zoom -= 1f;
            zoom = Mathf.Clamp(zoom, -6, 7);
            transform.localPosition = orig_zoom + (transform.forward * zoom);

        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            zoom += 1f;
            zoom = Mathf.Clamp(zoom, -6, 7);
            transform.localPosition = orig_zoom + (transform.forward * zoom);
        }

    }

    /**
     * @brief Set default values of the camera at start of game (or start of scene)
     */
    void InitializeCamera()
    {
        orig_rot_x = FollowX.rotation; // Set default pos to current pos
        mouseX = -45;
        orig_rot_y = FollowY.localRotation; // Set default rot to current rot
        mouseY = orig_rot_y.x;
        orig_zoom = transform.localPosition;
        isCameraReseting = false;
    }

    /**
     * @brief Reset camera to original position smoothly
     */
    public IEnumerator ResetCamera()
    {
        float limit = 0;
        isCameraReseting = true;
        ResetCameraNotification.SetActive(false);
        while (true)
        {
            limit += Time.deltaTime;
            FollowX.localRotation = Quaternion.Lerp(FollowX.rotation, orig_rot_x, cameraResetSpd);
            mouseX = Mathf.Lerp(mouseX, -45, cameraResetSpd);
            FollowY.localRotation = Quaternion.Lerp(FollowY.localRotation, orig_rot_y, cameraResetSpd);
            mouseY = Mathf.Lerp(mouseY, orig_rot_y.x, cameraResetSpd);
            transform.localPosition = Vector3.Lerp(transform.localPosition, orig_zoom, cameraResetSpd);

            if (limit >= .6f)
                break;
            yield return null; //advance frame
        }
        isCameraReseting = false;
    }
}
