using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /**
     * Camera Controller - Manager for game's camera. Can be controlled by player, cutscene, enemy, or nothing.
     *
     * Author : Omar Ilyas
     */

    //Camera controls
    public enum MODE { START, FOLLOWING, STATIONARY, PAUSED, CUTSCENE };

    public MODE cameraMode; //The current state of the camera
    private MODE prevMode; //The previous mode that the camera was in.
    public FollowObject followScript;
    public Transform FollowY, FollowX, CameraResetPoint;

    Action cameraBehavior; //Current Behavior function of the camera

    public float rotateSpeed;
    public float cameraResetSpd;
    private float zoom = 0f;

    private Quaternion orig_rot_x, orig_rot_y;
    private float mouseX, mouseY;

    private bool isCameraReseting;

    public GameObject ResetCameraNotification;

    // Start is called before the first frame update
    private void Start()
    {
        InitializeCamera();
        ResetCameraNotification.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        //Run chosen behavior function
        cameraBehavior();
    }

    public void Camera_Travelling()
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
        }

        //Zoom in and out with mouse wheel.
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            ResetCameraNotification.SetActive(true);
            if (zoom >= -.8f)
            {
                zoom += Input.GetAxisRaw("Mouse ScrollWheel");
                transform.position -= transform.forward;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            if (zoom <= .8f)
            {
                zoom += Input.GetAxisRaw("Mouse ScrollWheel");
                transform.position += transform.forward;
            }
        }
    }

    /**
     * @brief Set default values of the camera at start of game (or start of scene)
     */

    private void InitializeCamera()
    {
        orig_rot_x = FollowX.rotation; // Set default pos to current pos
        mouseX = -45;
        orig_rot_y = FollowY.localRotation; // Set default rot to current rot
        mouseY = orig_rot_y.x;
        isCameraReseting = false;

        cameraBehavior = Camera_Travelling;
        cameraMode = MODE.FOLLOWING;
        prevMode = MODE.START;
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
            zoom = 0f;
            transform.position = Vector3.Lerp(transform.position, CameraResetPoint.position, cameraResetSpd);

            if (limit >= .7f)
                break;
            yield return null; //advance frame
        }
        isCameraReseting = false;
    }

    /**
     * @brief change the state of the camera and assign a target
     * @param newMode the new state to put the camera into
     * @param target the gameobject to focus on. Default null. Behavior changes based on state change
     */

    public void ChangeState(MODE newMode, GameObject target = null)
    {
        if (newMode != cameraMode) //No duplicates
        {
            //Change Camera Behavior function
            switch (newMode)
            {
                case MODE.CUTSCENE:
                    break;
                case MODE.FOLLOWING:
                    cameraBehavior = Camera_Travelling;
                    break;
                default: return;
            }

            //Change State and target
            followScript.target = target;
            prevMode = cameraMode;
            cameraMode = newMode;
        }
    }
}