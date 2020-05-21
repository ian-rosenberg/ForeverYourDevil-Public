using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //Activate this in order to teleport the player to another location/scene
    //Can be activated through either touch or interaction

    protected gameManager gm;

    public bool locked; //Will this door work?
    public string areaID; //A name describing what location we're entering
    public Transform spawnPoint; //Where to teleport the player

    // Start is called before the first frame update
    void Start()
    {
        gm = gameManager.Instance;
    }

    // Update is called once per frame
    void Update()
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
        //Pause game
        gm.PauseGame();
        gm.SetCanPause(false);
        //Play canvas animation
        yield return new WaitForSecondsRealtime(2f);
        //Teleport player
        gm.playerAgent.ResetPath();
        gm.playerAgent.enabled = false;
        gm.playerAgent.velocity = Vector3.zero;
        gm.player.transform.position = spawnPoint.transform.position;
        gm.player.transform.rotation = spawnPoint.transform.rotation;
        gm.playerAgent.enabled = true;

        //Play unfade animation
        yield return new WaitForSecondsRealtime(1.5f);
        //Unpause Player
        gm.UnPauseGame();
        gm.SetCanPause(true);
    }

}
