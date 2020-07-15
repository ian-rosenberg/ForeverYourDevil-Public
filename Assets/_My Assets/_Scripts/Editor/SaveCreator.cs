using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveCreator : EditorWindow
{
    Vector2 scrollPos;

    private string OutputDirectory = "Saves"; /**Output directory to save files in*/


    static bool notNull = false; //True = 1, Save is valid | False = 0, Save is invalid

    static int index = 0;
    static string areaID = "Level1";
    static string sceneName = "Scene1";
    static string chapterName = "TestChapter";
    static float playTime = 0;
    static Vector3 playerPosition = new Vector3(0, 0, 0);
    static float[] playerRotation = new float[4];
    static int playerHealth = 25;
    static int playerMaxHealth = 25;
    static int playerTolerance = 0;
    static int playerMaxTolerance = 100;
    static int playerStamina = 3;
    static int playerMaxStamina = 3;
    static string currentLeader = null;
    static string[] partyMembers = new string[3];
    static string[] extraMembers = new string[4];
    static CameraController.MODE cameraMode = CameraController.MODE.FOLLOWING;
    static string skybox="CasualDay";

    [MenuItem("FYD Custom/Save Creator")]
    public static void ShowWindow()
    {
        GetWindow<SaveCreator>(false, "Save Creator", true);

        //areaID = null;
        //sceneName = null;
        //chapterName = null;
        //playTime = 0;
        //playerPosition = new float[0];
        //playerRotation = new float[0];
        //playerHealth = 0;
        //playerMaxHealth = 0;
        //playerTolerance = 0;
        //playerMaxTolerance = 0;
        //playerStamina = 0;
        //playerMaxStamina = 0;
        //currentLeader = null;
        //partyMembers = null;
        //extraMembers = null;
        //cameraMode = 0;
        //skybox = null;

        ////If not successful, set notNull flag
        //notNull = 0;

        //EditorGUILayout.IntField("Player Lifes", 3);
        //EditorGUILayout.TextField("Player Two Name", "John");
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));
        //Make button to get current values (where applic        GUILayout.Label("Scene Info");
        if (GUILayout.Button("Read Current Values"))
        {
            gameManager gm = FindObjectOfType<gameManager>();
            PlayerController player = FindObjectOfType<PlayerController>();
            areaID = gm.areaId;
            sceneName = SceneManager.GetActiveScene().name;
            chapterName = gm.chapterName;
            playTime = 420;
          
            playerPosition = player.transform.position;
            playerRotation[0] = player.transform.rotation.x;
            playerRotation[1] = player.transform.rotation.y;
            playerRotation[2] = player.transform.rotation.z;
            playerRotation[3] = player.transform.rotation.w;
            
            playerHealth = player.health;
            playerMaxHealth = player.maxHealth;
            playerTolerance = player.tolerance;
            playerMaxTolerance = player.maxTolerance;
            playerStamina = player.stamina;
            playerMaxStamina = player.maxStamina;
            
            currentLeader = gm.Leader;
            partyMembers = gm.partyMembers;
            extraMembers = gm.extraMembers;
            
            cameraMode = gm.mainCamera.cameraMode;
            skybox = RenderSettings.skybox.name;

            notNull = true;
        }

        index = EditorGUILayout.IntSlider("Save Index", index, 0, 9);
        areaID = EditorGUILayout.TextField("Area ID", areaID);
        sceneName = EditorGUILayout.TextField("Scene Name", sceneName);
        chapterName = EditorGUILayout.TextField("Chapter Name", chapterName);
        playTime = EditorGUILayout.FloatField("Play Time (in Sseconds)", playTime);
        GUILayout.Space(2);

        GUILayout.Label("Player");
        GUILayout.Space(1);
        playerPosition = EditorGUILayout.Vector3Field("Position", playerPosition);
        playerRotation[0] = EditorGUILayout.FloatField("Rotation (X)", playerRotation[0]);
        playerRotation[1] = EditorGUILayout.FloatField("Rotation (Y)", playerRotation[1]);
        playerRotation[2] = EditorGUILayout.FloatField("Rotation (Z)", playerRotation[2]);
        playerRotation[3] = EditorGUILayout.FloatField("Rotation (W)", playerRotation[3]);
        GUILayout.Space(2);

        playerHealth = EditorGUILayout.IntField("Health", playerHealth);
        playerMaxHealth = EditorGUILayout.IntField("Max Health:", playerMaxHealth);
        playerTolerance = EditorGUILayout.IntField("Tolerance", playerTolerance);
        playerMaxTolerance = EditorGUILayout.IntField("Max Tolerance", playerMaxTolerance);
        playerStamina = EditorGUILayout.IntField("Stamina", playerStamina);
        playerMaxStamina = EditorGUILayout.IntField("Max Stamina", playerMaxStamina);
        GUILayout.Space(2);

        GUILayout.Label("Party");
        GUILayout.Space(1);
        currentLeader = EditorGUILayout.TextField("Current Leader", currentLeader);
        partyMembers[0] = EditorGUILayout.TextField("Party Member 0", partyMembers[0]);
        partyMembers[1] = EditorGUILayout.TextField("Party Member 1", partyMembers[1]);
        partyMembers[2] = EditorGUILayout.TextField("Party Member 2", partyMembers[2]);
        GUILayout.Space(2);

        extraMembers[0] = EditorGUILayout.TextField("Extra Member 0", extraMembers[0]);
        extraMembers[1] = EditorGUILayout.TextField("Extra Member 1", extraMembers[1]);
        extraMembers[2] = EditorGUILayout.TextField("Extra Member 2", extraMembers[2]);
        extraMembers[3] = EditorGUILayout.TextField("Extra Member 3", extraMembers[3]);
        GUILayout.Space(2);

        GUILayout.Label("Scene Info (Will reorganize later)");
        GUILayout.Space(1);
        cameraMode = (CameraController.MODE)EditorGUILayout.EnumPopup("Camera Mode", cameraMode);
        skybox = EditorGUILayout.TextField("Skybox Path", skybox);
        notNull = EditorGUILayout.Toggle("Not Null", notNull);
        GUILayout.Space(3);

        OutputDirectory = EditorGUILayout.TextField("Save Directory", OutputDirectory);

        //Create Save Button
        if (GUILayout.Button("Create"))
        {
            //Error check all variables
            string export = Application.streamingAssetsPath + "/" + OutputDirectory;
            if (!Directory.Exists(export))
                Directory.CreateDirectory(export);

            //Set not null flag if all variables are valid
            //Create new save
            Save save = new Save(
                index,
                areaID,
                sceneName,
                chapterName,
                playTime,
                playerPosition,
                new Quaternion(playerRotation[0],
                playerRotation[1],
                playerRotation[2],
                playerRotation[3]),
                playerHealth,
                playerMaxHealth,
                playerTolerance,
                playerMaxTolerance,
                playerStamina,
                playerMaxStamina,
                currentLeader,
                partyMembers,
                extraMembers,
                cameraMode,
                Resources.Load<Material>("Skyboxes/SkySerie Freebie/" + skybox)
                );



            //Write save to bin file
            export += "/Save (" + index + ")";

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(export, FileMode.Create);
            bf.Serialize(file, save);
            file.Close();

            Debug.Log("File Saved!");
            // DebugLogSaveProperties(save);
            //Debug.Log("EXPORT: " + export);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
    }
    void OnInspectorUpdate()
    {
    }
}

