using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

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
        playerHealth = 0;
        playerMaxHealth = 0;
        playerTolerance = 0;
        playerMaxTolerance = 0;
        playerStamina = 0;
        playerMaxStamina = 0;
        currentLeader = null;
        partyMembers = null;
        extraMembers = null;

        //If not successful, set notNull flag
        notNull = 0;
    }

    public Save(int index, string areaID, string sceneName, string chapterName, float playTime, Vector3 playerPosition, int health, int maxHealth, int tolerance, int maxTolerance, int stamina, int maxStamina, string currentLeader, string[] partyMembers, string[] extraMembers)
    {
        this.index = index;
        this.areaID = areaID;
        this.sceneName = sceneName;
        this.chapterName = chapterName;
        this.playTime = playTime;

        this.playerPosition = new float[] { playerPosition.x, playerPosition.y, playerPosition.z };

        this.playerHealth = health;
        this.playerMaxHealth = maxHealth;
        this.playerTolerance = tolerance;
        this.playerMaxTolerance = maxTolerance;
        this.playerStamina = stamina;
        this.playerMaxStamina = maxStamina;
        this.currentLeader = currentLeader;
        this.partyMembers = partyMembers;
        this.extraMembers = extraMembers;

        //If successful, set notNull flag
        notNull = 1;
    }
}

public class SaveManager : MonoBehaviour
{
    public GameObject SavingCanvas;

    public Save_Slot[] saveSlotList;

    private gameManager gm;
    private StatManager stat;
    public bool saveMode = true; //true = Save, false = load;

    //Display stuff
    public TextMeshProUGUI Title;
    public GameObject loadingIcon;

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
        gm.player.health,
        gm.player.maxHealth,
        gm.player.tolerance,
        gm.player.maxTolerance,
        gm.player.stamina,
        gm.player.maxStamina,
        "Penny_Test_Head",
        new string[] { "Player", "", "" },
        new string[] { "", "", "", "" });

        //Write save to bin file
        export += "/Save (" + num + ")";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(export, FileMode.Create);
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("File Saved!");
        DebugLogSaveProperties(save);
        Debug.Log("EXPORT: " + export);
    }

    public Save ReadSave(int num)
    {
        string import = Application.streamingAssetsPath + "/" + OutputDirectory + "/Save (" + num + ")";
        Debug.Log("IMPORT: " + import);

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

    public void LoadSave(int num)
    {
        //Play transition animation
        //Load level
        Save save = ReadSave(num);
        gm.areaId = save.areaID;
        gm.sceneName = save.sceneName;
        stat.startTime = save.playTime;
        gm.player.health = save.playerHealth;
        gm.player.maxHealth = save.playerMaxHealth;
        gm.player.tolerance = save.playerTolerance;
        gm.player.maxTolerance = save.playerMaxTolerance;
        gm.player.stamina = save.playerStamina;
        gm.player.maxStamina = save.playerMaxStamina;
        gm.player.transform.position = new Vector3(save.playerPosition[0], save.playerPosition[1], save.playerPosition[2]); //May have to make gameManager start position for sceneName
        //current leader
        //party Members
    }

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
    }

    public void disableCanvas(float delay)
    {
        StartCoroutine(delayedDisable(delay));
    }

    private IEnumerator delayedDisable(float delay)
    {
        //Play animation
        SavingCanvas.GetComponent<Animator>().SetTrigger("Exit");
        yield return new WaitForSecondsRealtime(delay);
       
        //Turn off canvas
        SavingCanvas.SetActive(false);
        gm.UnPauseGame();
    }
}