using System;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Save_Slot : MonoBehaviour
{
    [Tooltip("The number of the save. CRUCIAL")]
    public int index = 0;

    public SaveManager saveManager;
    public Button button;

    public TextMeshProUGUI GameTime, AreaID, ChapterName, SaveName;
    public Image Leader;
    public Image[] PartyMember, ExtraMember;

    // Start is called before the first frame update
    private void Start()
    {
        saveManager = SaveManager.Instance;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        DisplaySaveInfo();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnClick()
    {

        if (saveManager.saveMode)
            StartCoroutine(SaveAndDisplay());
        else
            LoadSave();

    }

    public IEnumerator SaveAndDisplay()
    {
        saveManager.SaveGame(index);
        saveManager.loadingIcon.SetActive(true);
        saveManager.SetAllSaveSlotsInteractable(false);
        //Wait until game is saved before displaying information
        yield return new WaitForSeconds(1f);
        saveManager.SetAllSaveSlotsInteractable(true);
        saveManager.loadingIcon.SetActive(false);
        //Display information from save.
        DisplaySaveInfo();
    }
    public void LoadSave()
    {
        saveManager.LoadSave(index);
    }
    public void DisplaySaveInfo()
    {
        //Read save.
        Debug.Log("<color=green>Local Save Slot index: " + index + "</color>");
        Save save = saveManager.ReadSave(index);

        //saveManager.DebugLogSaveProperties(save);

        //If Save is populated, set all parameters
        if (save.notNull != 0)
        {
            //Set all parameters
            GameTime.text = timeToString(save.playTime);
            AreaID.text = save.areaID;
            ChapterName.text = save.chapterName;

            //Set Save name - 0 = autosave
            if (index == 0) SaveName.text = "Autosave";
            else SaveName.text = "Save " + index;

            //Set Leader Image
            if (!save.currentLeader.Equals(null))
            {
                Leader.gameObject.SetActive(true);
                Leader.sprite = Resources.Load<Sprite>("Sprites/" + save.currentLeader);
            }
            else
                Leader.gameObject.SetActive(false);

            //Set Party Images
            for (int i = 0; i < save.partyMembers.Length; i++)
            {
                if (i < 3)
                {
                    PartyMember[i].gameObject.SetActive(true);
                    PartyMember[i].sprite = Resources.Load<Sprite>("Sprites/" + save.partyMembers[i]);
                }
                else
                {
                    ExtraMember[i].gameObject.SetActive(true);
                    ExtraMember[i].sprite = Resources.Load<Sprite>("Sprites/" + save.partyMembers[i]);
                }
            }
        }
        //If null save file, or save not found, use default values
        else
        {
            GameTime.text = "___:__:__";
            AreaID.text = "...";
            ChapterName.text = "...";
            SaveName.text = "NO DATA";
            Leader.gameObject.SetActive(false);
            //Set Party Images
            for (int i = 0; i < PartyMember.Length; i++)
            {
                PartyMember[i].gameObject.SetActive(false);
            }
            //Set Extra Members
            for (int i = 0; i < ExtraMember.Length; i++)
            {
                ExtraMember[i].gameObject.SetActive(false);
            }
        }
    }


    public string timeToString(float time)
    {
        //Separate time into readable numbers
        double hours = Math.Floor(time / 3600); //Should return 0 if time is not above 0
        double minutes = Math.Floor((time / 60) % 60);
        double seconds = Math.Floor(time % 60);

        return hours + ":" + minutes + ":" + seconds;
    }
}