using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * @brief A structure that defines what a Save File is.
 */

[Serializable]
public struct Save
{
    //General properties
    public int notNull; /**Flag to check if save file is populated. 0 = not populated, non-zero = populated*/

    public int index; /**Name/number of the save file*/
    public string areaID; /**Name of the area that the player was in last*/
    public string sceneName; /**Name of the scene where the player will load/spawn*/
    public string chapterName; /**Name of the scene where the player will load/spawn*/
    public float playTime; /**How long the player has been playing total.*/

    //Player properties
    public float[] playerPosition; /**Convert Vector3 to floats for serializability*/

    public float[] playerRotation; /**Convert Quaternion to floats for serializability*/

    public int playerHealth; /**Current Health of the player; 0 kills player*/
    public int playerMaxHealth; /**Max Health the player is allowed to heal to*/
    public int playerTolerance; /**Current Tolerance. Max will kill the player*/
    public int playerMaxTolerance; /**Max Tolerance before player is killed*/
    public int playerStamina; /** Current Stamina, allows player to do actions*/
    public int playerMaxStamina; /**Max stamina player is allowed to have*/

    //Party properties
    public string currentLeader;

    public string[] partyMembers;
    public string[] extraMembers;

    //Camera Properties (Assuming player always saves at save point)
    public int cameraMode;
    //public float[] cameraTarget; //Necessary for stationary camera saving

    //World Properties
    public string skybox; //String name of the current skybox in use

    //Enemies alive in area
    //Quest status
    //Quests completed
    //Health/tolerance/maxes of every party member

    /**
     * @brief create a blank save that has a notNull value of 0;
     */

    public Save(int index)
    {
        this.index = index;
        areaID = null;
        sceneName = null;
        chapterName = null;
        playTime = 0;
        playerPosition = new float[0];
        playerRotation = new float[0];
        playerHealth = 0;
        playerMaxHealth = 0;
        playerTolerance = 0;
        playerMaxTolerance = 0;
        playerStamina = 0;
        playerMaxStamina = 0;
        currentLeader = null;
        partyMembers = null;
        extraMembers = null;
        cameraMode = 0;
        skybox = null;

        //If not successful, set notNull flag
        notNull = 0;
    }

    public Save(int index, string areaID, string sceneName, string chapterName, float playTime, Vector3 playerPosition, Quaternion playerRotation, int health, int maxHealth, int tolerance, int maxTolerance, int stamina, int maxStamina, string currentLeader, string[] partyMembers, string[] extraMembers, CameraController.MODE cameraMode, Material skybox)
    {
        this.index = index;
        this.areaID = areaID;
        this.sceneName = sceneName;
        this.chapterName = chapterName;
        this.playTime = playTime;

        this.playerPosition = new float[] { playerPosition.x, playerPosition.y, playerPosition.z };
        this.playerRotation = new float[] { playerRotation.x, playerRotation.y, playerRotation.z, playerRotation.w };

        this.playerHealth = health;
        this.playerMaxHealth = maxHealth;
        this.playerTolerance = tolerance;
        this.playerMaxTolerance = maxTolerance;
        this.playerStamina = stamina;
        this.playerMaxStamina = maxStamina;
        this.currentLeader = currentLeader;

        this.partyMembers = new string[3];
        for (int i = 0; i < partyMembers.Length; i++)
        {
            this.partyMembers[i] = partyMembers[i];
        }
        this.extraMembers = new string[4];
        for (int i = 0; i < extraMembers.Length; i++)
        {
            this.extraMembers[i] = extraMembers[i];
        }            //party Members

        this.cameraMode = (int)cameraMode;

        this.skybox = skybox.name;
        //If successful, set notNull flag
        notNull = 1;
    }
}

public class SaveManager : MonoBehaviour
{
    //NOTE TO SELF: Add a warning to protect saving/loading corrupt or non working saves.

    //Display
    public Animator SavingCanvas;

    public TextMeshProUGUI Title;
    public GameObject loadingIcon;

    public Save_Slot[] saveSlotList;

    private gameManager gm;
    private StatManager stat;
    public bool saveMode = true; //true = Save, false = load;

    //Singleton creation
    private static SaveManager instance;

    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<SaveManager>();
            return instance;
        }
    }

    [SerializeField]
    private string OutputDirectory; /**Output directory to save files in*/

    private void Awake()
    {
        gm = gameManager.Instance;
        stat = StatManager.Instance;
    }

    #region Saving/Loading

    /**
     * @brief Saves the game status to a bin file
     * @param num the current save file index to save to (0 = autosave)
     */

    public void SaveGame(int num)
    {
        //Error check all variables
        string export = Application.streamingAssetsPath + "/" + OutputDirectory;
        if (!Directory.Exists(export))
            Directory.CreateDirectory(export);

        //Set not null flag if all variables are valid
        //Create new save
        Save save = new Save(
        num,
        gm.areaId,
        gm.sceneName,
        "Chapter _: The Antithesis of Graphic Design",
        stat.GetTimeInSeconds(),
        gm.player.transform.position,
        gm.player.transform.rotation,
        gm.player.health,
        gm.player.maxHealth,
        gm.player.tolerance,
        gm.player.maxTolerance,
        gm.player.stamina,
        gm.player.maxStamina,
        gm.Leader,
        gm.partyMembers,
        gm.extraMembers,
        gm.mainCamera.prevMode, //Camera will be paused, so previous state is what we're after
        RenderSettings.skybox);

        //Write save to bin file
        export += "/Save (" + num + ")";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(export, FileMode.Create);
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("File Saved!");
        // DebugLogSaveProperties(save);
        //Debug.Log("EXPORT: " + export);
    }

    public Save ReadSave(int num)
    {
        string import = Application.streamingAssetsPath + "/" + OutputDirectory + "/Save (" + num + ")";
        //Debug.Log("IMPORT: " + import);

        if (!File.Exists(import))
        {
            Debug.LogWarning("Save at " + import + "does not exist.");
            return new Save(num);
        }

        FileStream file = new FileStream(import, FileMode.Open);
        BinaryFormatter br = new BinaryFormatter();
        Save save = (Save)br.Deserialize(file);
        file.Close();
        return save;
    }

    public IEnumerator LoadSave(int num)
    {
        SetAllSaveSlotsInteractable(false);
        //Play transition animation
        //Load level
        Save save = ReadSave(num);

        DebugLogSaveProperties(save);
        //If scene has NO errors, load as normal. Otherwise, prompt message saying that save is corrupt.

        //Change scene if it is not the same as the current scene.
        if (!save.sceneName.Equals(gm.sceneName))
        {
            //Play fade animation followed by scene transition
            SceneManager.LoadScene(save.sceneName);
        }
        //If same scene, reload player position, properties, etc.
        else
        {
            //Play fade animation followed by transition and loading icon
            SavingCanvas.SetBool("isLoading", true);
            yield return new WaitForSecondsRealtime(.25f);
            gm.fade.SetActive(false);
            loadingIcon.SetActive(true);
            gm.AllowPlayerToPause(false);
            gm.ExitPauseMenuFunction();

            gm.areaId = save.areaID;
            stat.startTime = save.playTime;

            //Adjust player properties
            gm.player.health = save.playerHealth;
            gm.player.maxHealth = save.playerMaxHealth;
            gm.player.tolerance = save.playerTolerance;
            gm.player.maxTolerance = save.playerMaxTolerance;
            gm.player.stamina = save.playerStamina;
            gm.player.maxStamina = save.playerMaxStamina;

            //Adjust player position
            gm.player.agent.isStopped = true;
            gm.player.agent.ResetPath();
            gm.player.agent.enabled = false;
            gm.player.anim.SetBool("StayIdle", true);
            gm.player.transform.position = new Vector3(save.playerPosition[0], save.playerPosition[1], save.playerPosition[2]); //May have to make gameManager start position for sceneName
            gm.player.transform.rotation = new Quaternion(save.playerRotation[0], save.playerRotation[1], save.playerRotation[2], save.playerRotation[3]);

            //current leader
            gm.Leader = save.currentLeader;
            for (int i = 0; i < gm.partyMembers.Length; i++)
            {
                gm.partyMembers[i] = save.partyMembers[i];
            }
            for (int i = 0; i < gm.extraMembers.Length; i++)
            {
                gm.extraMembers[i] = save.extraMembers[i];
            }            //party Members

            //Adjust camera (Assuming camera is traveling and following player)
            //gm.mainCamera.ChangeCameraState((CameraController.MODE)save.cameraMode, gm.player.transform);
            gm.mainCamera.ChangeCameraState(CameraController.MODE.FOLLOWING, gm.player.transform);
            gm.mainCamera.QuickResetCamera();

            //Adjust world settings
            //RenderSettings.skybox = Resources.Load<Material>("/Skyboxes/SkySerie Freebie/" + save.skybox);
            //DynamicGI.UpdateEnvironment();

            //Wait a bit
            yield return new WaitForSecondsRealtime(3f);

            //Play unfade animation and unpause
            loadingIcon.SetActive(false);
            gm.Player_UnPauseGame();
            gm.AllowPlayerToPause(true);
            SetAllSaveSlotsInteractable(true);

            //Show unfade and resume (NEED TO BLOCK PLAYER INPUT TILL FADE IS DONE.)
            yield return new WaitForSecondsRealtime(1f);
            SavingCanvas.SetBool("isLoading", false);
            yield return new WaitForSecondsRealtime(0.183f);
            
            //Unpause Player
            gm.player.agent.enabled = true;
            gm.player.agent.isStopped = false;
            gm.player.anim.SetBool("StayIdle", false);
            //Turn off Canvas
            SavingCanvas.gameObject.SetActive(false);

            
        }
    }

    #endregion Saving/Loading

    #region Displaying Save Slots

    public void UpdateSaveSlot(int index)
    {
        saveSlotList[index].DisplaySaveInfo();
    }

    public void SetAllSaveSlotsInteractable(bool active)
    {
        foreach (Save_Slot slot in saveSlotList)
        {
            slot.button.interactable = active;
        }
    }

    public void UpdateAllSaveSlots()
    {
        foreach (Save_Slot slot in saveSlotList)
        {
            slot.DisplaySaveInfo();
        }
    }

    #endregion Displaying Save Slots

    #region Displaying Menu

    public void ShowSaveMenu()
    {
        saveMode = true;
        SavingCanvas.gameObject.SetActive(true);
        gm.fade.SetActive(true);
        Title.text = "SAVE GAME";
        gm.Game_PauseGame();
        gm.AllowPlayerToPause(false);
        UpdateAllSaveSlots();
    }

    public void ShowLoadMenu()
    {
        saveMode = false;
        SavingCanvas.gameObject.SetActive(true);
        gm.fade.SetActive(true);
        Title.text = "LOAD GAME";
        gm.Player_PauseGame();
        UpdateAllSaveSlots();
    }

    public void disableCanvas(float delay)
    {
        StartCoroutine(delayedDisable(delay));
    }

    private IEnumerator delayedDisable(float delay)
    {
        //Exit menu
        //Play animation
        SavingCanvas.SetTrigger("Exit");
        yield return new WaitForSecondsRealtime(delay);

        //Turn off canvas
        SavingCanvas.gameObject.SetActive(false);

        //Unpause if exiting save menu, not if exiting load menu
        if (saveMode)
        {
            gm.fade.SetActive(false);
            gm.Game_UnPauseGame();
            gm.AllowPlayerToPause(true);
        }
    }

    #endregion Displaying Menu

    public void DebugLogSaveProperties(Save save)
    {
        Debug.Log("<color=blue>Save Properties:</color>");
        Debug.Log("<color=blue>Save Name/ Index: " + save.index + "</color>");

        Debug.Log("notNull: " + save.notNull);
        Debug.Log("areaID: " + save.areaID);
        Debug.Log("sceneName: " + save.sceneName);
        Debug.Log("chapterName: " + save.chapterName);
        Debug.Log("playTime: " + stat.timeToString(save.playTime));
        Debug.Log("playerPosition: " + save.playerPosition[0] + ", " + save.playerPosition[1] + ", " + save.playerPosition[2]);
        Debug.Log("playerRotation: " + save.playerRotation[0] + ", " + save.playerRotation[1] + ", " + save.playerRotation[2] + ", " + save.playerRotation[3]);
        Debug.Log("playerHealth: " + save.playerHealth);
        Debug.Log("playerMaxHealth: " + save.playerMaxHealth);
        Debug.Log("playerTolerance: " + save.playerTolerance);
        Debug.Log("playerMaxTolerance: " + save.playerMaxTolerance);
        Debug.Log("playerStamina: " + save.playerStamina);
        Debug.Log("playerMaxStamina: " + save.playerMaxStamina);

        Debug.Log("Party:");
        Debug.Log("currentLeader: " + save.currentLeader);
        for (int i = 0; i < save.partyMembers.Length; i++)
        {
            Debug.Log("PartyMember (" + i + "): " + save.partyMembers[i]);
        }
        Debug.Log("Current skybox: " + save.skybox);
    }
}