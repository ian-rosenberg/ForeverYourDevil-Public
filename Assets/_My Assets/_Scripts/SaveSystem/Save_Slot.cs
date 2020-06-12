using System;
using System.Collections;
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
        yield return new WaitForSecondsRealtime(1f);
        saveManager.SetAllSaveSlotsInteractable(true);
        saveManager.loadingIcon.SetActive(false);
        //Display information from save.
        saveManager.UpdateAllSaveSlots();
        //Exit menu
        saveManager.disableCanvas(0.333f);
    }

    public void LoadSave()
    {
        StartCoroutine(saveManager.LoadSave(index));
    }

    public void DisplaySaveInfo()
    {
        //Read save.
        //Debug.Log("<color=green>Local Save Slot index: " + index + "</color>");
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

            //Set Party Member Images
            for (int i = 0; i < PartyMember.Length; i++)
            {
                //If member is a blank string, set it active to false
                if (save.partyMembers[i].Equals(""))
                {
                    PartyMember[i].gameObject.SetActive(false);
                    continue;
                }
                //Else, add party member to thing
                else
                {
                    PartyMember[i].gameObject.SetActive(true);
                    PartyMember[i].sprite = Resources.Load<Sprite>("Sprites/" + save.partyMembers[i]);
                }
            }

            //Set Extra Member Images
            for (int i = 0; i < ExtraMember.Length; i++)
            {
                //If member is a blank string, set it active to false
                if (save.extraMembers[i].Equals(""))
                {
                    ExtraMember[i].gameObject.SetActive(false);
                    continue;
                }
                //Else, add party member to thing
                else
                {
                    ExtraMember[i].gameObject.SetActive(true);
                    ExtraMember[i].sprite = Resources.Load<Sprite>("Sprites/" + save.extraMembers[i]);
                }
            }
            //If all checks out, make button interactable
            button.interactable = true;
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

            //If loading, do not allow player to load blank save files
            if (!saveManager.saveMode)
            {
                button.interactable = false;
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