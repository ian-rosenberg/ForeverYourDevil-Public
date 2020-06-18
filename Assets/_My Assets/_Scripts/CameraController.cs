using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    /**
     * Camera Controller - Manager for game's camera. Can be controlled by player, cutscene, enemy, or nothing.
     *
     * Author : Omar Ilyas
     */

    //Singleton creation
    private static CameraController instance;
    public static CameraController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<CameraController>();
            return instance;
        }
    }

    private gameManager gm;

    //Camera controls
    public enum MODE { START, FOLLOWING, STATIONARY, FREE, PAUSED, CUTSCENE, COMBAT };

    [Header("Camera Controls")]
    public MODE cameraMode; //The current state of the camera

    public MODE prevMode; //The previous mode that the camera was in.
    public FollowObject followScript;
    public Transform FollowY, FollowX, CameraResetPoint;

    private Action cameraBehavior; //Current Behavior function of the camera

    [Header("Speeds")]
    public float rotateSpeed;
    public float cameraResetSpd;
    private float zoom = 0f;

    private Quaternion orig_rot_x, orig_rot_y;
    private float mouseX, mouseY;

    private bool isCameraReseting;

    public GameObject ResetCameraNotification;

    [Header("DEBUG")]
    public Transform enemyLockOn;
    private Vector3 dampVelocity = Vector3.zero;

    [Header("Player Input")]
    private bool camReset = false;
    private bool camLockState = false;
    public PlayerControls pControls;

    private void OnEnable()
    {
        pControls = new PlayerControls();

        pControls.Player.ResetCamera.performed += context => camReset = true;
        pControls.Player.LockTarget.performed += context => camLockState = !camLockState;

        pControls.Player.ResetCamera.Enable();
        pControls.Player.LockTarget.Enable();
    }

    private void OnDisable()
    {
        pControls.Player.ResetCamera.performed -= context => camReset = true;
        pControls.Player.LockTarget.performed -= context => camLockState = !camLockState;

        pControls.Player.ResetCamera.Disable();
        pControls.Player.LockTarget.Disable();
    }

    private void Awake()
    {
        gm = gameManager.Instance;
    }

    // Start is called before the first frame update
    private void Start()
    {
        InitializeCamera();
        ResetCameraNotification.SetActive(false);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        //Run chosen behavior function
        cameraBehavior();
    }

    public void Camera_Following()
    {
        //Camera Reset
        if (camReset && !isCameraReseting && gm.gameState != gameManager.STATE.PAUSED)
        {
            StartCoroutine(ResetCamera());
        }

        //Move camera with right click and hold
        if ((Mathf.Abs(Mouse.current.rightButton.ReadValue()) != 0) && !isCameraReseting) //If hold right click
        {
            ResetCameraNotification.SetActive(true);
            mouseX += Mouse.current.delta.x.ReadValue() * rotateSpeed;
            mouseY += Mouse.current.delta.y.ReadValue() * rotateSpeed;

            mouseY = Mathf.Clamp(mouseY, -30, 45);

            FollowX.rotation = Quaternion.Euler(0f, mouseX, 0f);
            FollowY.localRotation = Quaternion.Euler(-mouseY, 0f, 0f);
        }

        //Debug.Log(Mouse.current.scroll.ReadValue().y);

        //Zoom in and out with mouse wheel.
        //float z = Mathf.Clamp(Mouse.current.scroll, -.8f, .8f);

        //transform.position += new Vector3(0, 0, z);
        if (gm.gameState != gameManager.STATE.PAUSED)
        {
            if (Mouse.current.scroll.ReadValue().y < 0) // back
            {
                ResetCameraNotification.SetActive(true);
                if (zoom >= -.8f)
                {
                    zoom += -.1f;
                    transform.position -= transform.forward;
                }
            }
            if (Mouse.current.scroll.ReadValue().y > 0) // forward
            {
                if (zoom <= .8f)
                {
                    zoom += .1f;
                    transform.position += transform.forward;
                }
            }
        }

        ////DEBUG - lock onto specified target with keypress
        //if (!camLockState)
        //{
        //    ChangeCameraState(MODE.FOLLOWING, enemyLockOn.transform);
        //}
        //if (camLockState)
        //{
        //    ChangeCameraState(MODE.FOLLOWING, gm.player.transform);
        //}

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

        followScript.target = gm.player.transform;
        cameraBehavior = Camera_Following;
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
        camReset = false;
    }

    /**
    * @brief Reset camera to original position immediately, no interpolation.
    */

    public void QuickResetCamera()
    {
        ResetCameraNotification.SetActive(false);

        FollowX.localRotation = orig_rot_x;
        mouseX = -45;
        FollowY.localRotation = orig_rot_y;
        mouseY = orig_rot_y.x;
        zoom = 0f;
        transform.position = CameraResetPoint.position;
    }

    /**
     * @brief Get gameManager state change to change Camera state
     * @param newState the new state of the gameManager
     */

    //public void ChangedStateTo(gameManager.STATE newState)
    //{
    //    switch (newState)
    //    {
    //        case gameManager.STATE.START:
    //            Debug.LogError("Cannot switch GM State to START. This should not happen.");
    //            break;

    //        case gameManager.STATE.TRAVELING:
    //            ChangeCameraState(MODE.FOLLOWING, gm.player.transform);
    //            break;

    //        case gameManager.STATE.COMBAT:
    //            ChangeCameraState(MODE.COMBAT);
    //            break;

    //        case gameManager.STATE.PAUSED:
    //            //ChangeCameraState(MODE.PAUSED);
    //            break;

    //        case gameManager.STATE.TALKING:
    //            ChangeCameraState(MODE.STATIONARY, gm.player.transform);
    //            break;
    //    }
    //}

    /**
     * @brief change the state of the camera and assign a target to follow, or a position to warp to if stationary
     * @param newMode the new state to put the camera into
     * @param target the gameobject to focus on. Default null. Behavior changes based on state change
     */

    public void ChangeCameraState(MODE newMode, Transform target = null)
    {
        if (newMode != MODE.START)
        {
            //Change Camera Behavior function
            switch (newMode)
            {
                case MODE.START:
                    Debug.LogError("Cannot switch Camera State to START. This should not happen.");
                    break;

                case MODE.FOLLOWING:
                    cameraBehavior = Camera_Following;
                    followScript.enabled = true;
                    if (target)
                    {
                        //Follow target specified
                        followScript.target = target;
                        followScript.transform.position = gm.player.transform.position;

                    }
                    else
                        //If not specified, follow the player
                        followScript.target = gm.player.transform;
                    followScript.transform.position = gm.player.transform.position;
                    break;

                case MODE.STATIONARY:
                    Debug.Log("Reached Stationary Case.");
                    cameraBehavior = Camera_Stationary;
                    followScript.enabled = false;
                    if (target)
                    {
                        followScript.transform.position = target.position;
                    }
                    break;

                case MODE.PAUSED:
                    cameraBehavior = Camera_Paused;
                    followScript.enabled = false;
                    break;

                case MODE.COMBAT:
                    cameraBehavior = Camera_Combat;
                    followScript.enabled = true;
                    break;

                case MODE.CUTSCENE:
                    cameraBehavior = Camera_Cutscene;
                    followScript.enabled = false;
                    break;
                case MODE.FREE:
                    cameraBehavior = Camera_Free;
                    followScript.enabled = false;
                    break;
            }

            //Change State (only if the new state is not a duplicate
            if (newMode != cameraMode)
            {
                prevMode = cameraMode;
                cameraMode = newMode;
            }
        }
    }

    private void Camera_Combat()
    {
    }

    private void Camera_Free()
    {

    }

    private void Camera_Cutscene()
    {
    }

    private void Camera_Paused()
    {
    }

    private void Camera_Stationary()
    {

    }
}