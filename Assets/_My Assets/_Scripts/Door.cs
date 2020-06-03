using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    /** Door - Activate this in order to teleport the player to another location/scene
    * Author - Omar Ilyas
    */
    protected gameManager gm;

    [Header("Properties")]
    public bool locked = false; //Will this door work?

    public string areaID = "Tan Pill's Domain"; //A name describing what location we're entering
    public CameraController.MODE newCameraMode = CameraController.MODE.FOLLOWING; //New Camera state to put the camera into
    public Transform SpawnPoint; //Where to teleport the player

    [Header("Optional")]
    public Material skybox; //Skybox to load in when teleporting (optional)

    public Transform cameraSpawnPoint; //New camera position to spawn camera in (only mandatory/applied for MODE.STATIONARY) (optional)

    // Start is called before the first frame update
    private void Start()
    {
        gm = gameManager.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    protected void Activate()
    {
        StartCoroutine(Teleport());
    }

    //Play animation and teleport player to spawnPoint
    protected IEnumerator Teleport()
    {
        Debug.Log("Activated Door!");
        //Teleport player
        gm.player.agent.isStopped = true;
        gm.player.agent.ResetPath();
        gm.player.agent.enabled = false;
        gm.player.anim.SetBool("StayIdle", true);
        //Pause game
        gm.PauseGame();
        gm.SetCanPause(false);
        //Play canvas animation
        gm.CanvasAnimator.SetTrigger("Door");
        yield return new WaitForSecondsRealtime(1f);
        gm.player.transform.position = SpawnPoint.transform.position;
        gm.player.transform.rotation = SpawnPoint.transform.rotation;

        //Change skybox
        if (skybox)
        {
            RenderSettings.skybox = skybox;
            DynamicGI.UpdateEnvironment();
        }
        
        //Change CameraMode + Location
        gm.mainCamera.ChangeCameraState(newCameraMode, cameraSpawnPoint);

        //Reset Camera Quickly
        gm.mainCamera.QuickResetCamera();

        //Play unfade animation
        yield return new WaitForSecondsRealtime(0.5f);
        //Unpause Player
        gm.player.agent.enabled = true;
        gm.player.anim.SetBool("StayIdle", false);
        gm.UnPauseGame();
        gm.player.agent.isStopped = false;
        gm.SetCanPause(true);

        //Change music
        //Insert FMOD code here
    }
}