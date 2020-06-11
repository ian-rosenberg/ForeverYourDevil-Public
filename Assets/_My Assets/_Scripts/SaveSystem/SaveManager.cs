using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/**
 * @brief A structure that defines what a Save File is.
 */

public struct Save
{
    //General properties
    public int notNull; /**Flag to check if save file is populated. 0 = not populated, non-zero = populated*/

    public int index; /**Name/number of the save file*/
    public string areaID; /**Name of the area that the player was in last*/
    public string sceneName; /**Name of the scene where the player will load/spawn*/
    public float playTime; /**How long the player has been playing total.*/

    //Player properties
    public Vector3 playerPosition;

    public int playerHealth; /**Current Health of the player; 0 kills player*/
    public int playerMaxHealth; /**Max Health the player is allowed to heal to*/
    public int playerTolerance; /**Current Tolerance. Max will kill the player*/
    public int playerMaxTolerance; /**Max Tolerance before player is killed*/
    public int playerStamina; /** Current Stamina, allows player to do actions*/
    public int playerMaxStamina; /**Max stamina player is allowed to have*/

    //Party properties
    public string currentLeader;

    public string[] partyMembers;

    /**
     * @brief create a blank save that has a notNull value of 0;
     */

    public Save(int index)
    {
        this.index = 0;
        areaID = null;
        sceneName = null;
        playTime = 0;
        playerPosition = Vector3.zero;
        playerHealth = 0;
        playerMaxHealth = 0;
        playerTolerance = 0;
        playerMaxTolerance = 0;
        playerStamina = 0;
        playerMaxStamina = 0;
        currentLeader = null;
        partyMembers = null;

        //If not successful, set notNull flag
        notNull = 0;
    }

    public Save(int index, string sceneName, string areaID, float playTime, Vector3 playerPosition, int health, int maxHealth, int tolerance, int maxTolerance, int stamina, int maxStamina, string currentLeader, string[] partyMembers) : this()
    {
        this.index = index;
        this.areaID = areaID;
        this.sceneName = sceneName;
        this.playTime = playTime;
        this.playerPosition = playerPosition;
        this.playerHealth = health;
        this.playerMaxHealth = maxHealth;
        this.playerTolerance = tolerance;
        this.playerMaxTolerance = maxTolerance;
        this.playerStamina = stamina;
        this.playerMaxStamina = maxStamina;
        this.currentLeader = currentLeader;
        this.partyMembers = partyMembers;

        //If successful, set notNull flag
        notNull = 1;
    }
}

public class SaveManager : MonoBehaviour
{
    public Save_Slot[] saveSlotList;

    private gameManager gm;
    private StatManager stat;

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

    private string OutputFilename; /**Output filename to save file*/

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
        stat.GetTimeInSeconds(),
        gm.player.transform.position,
        gm.player.health,
        gm.player.maxHealth,
        gm.player.tolerance,
        gm.player.maxTolerance,
        gm.player.stamina,
        gm.player.maxStamina,
        "Penny_Test_Head",
        null);

        //Write save to bin file
        export += "/Save (" + num + ")";
        if (!File.Exists(export))
        {
            File.Create(export);
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(export, FileMode.Create);
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("File Saved!");
    }

    public Save ReadSave(int num)
    {
        string import = Application.streamingAssetsPath + "/" + OutputDirectory + "/Save (" + num + ")";
        if (!Directory.Exists(import))
        {
            Debug.LogError("Cannot load! Directory/Save doesn't exist.");
            return new Save(0);
        }

        FileStream file = new FileStream(import, FileMode.Open);
        BinaryFormatter br = new BinaryFormatter();
        return (Save)br.Deserialize(file);
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
        gm.player.transform.position = save.playerPosition; //May have to make gameManager start position for sceneName
        //current leader
        //party Members
    }

    public void UpdateSaveSlots()
    {
        foreach (Save_Slot slot in saveSlotList)
        {
            slot.DisplaySaveInfo();
        }
    }
}