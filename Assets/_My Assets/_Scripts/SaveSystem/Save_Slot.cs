using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Save_Slot : MonoBehaviour
{
    [Tooltip("The number of the save. CRUCIAL")]
    public int index = 0;
    public SaveManager saveManager;

    public bool saveMode; //true = Save, false = load;

    public TextMeshProUGUI GameTime, AreaID, ChapterName, SaveName;
    public Image Leader;
    public Image[] PartyMember, ExtraMember;

    // Start is called before the first frame update
    void Start()
    {
        saveManager = SaveManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisplaySaveInfo()
    {
        //Set all parameters
        Save save = saveManager.ReadSave(index);
        GameTime.text = timeToString(save.playTime);
        AreaID.text = save.areaID;
        //  ChapterName.text = save.chapterName;
        
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
        for(int i = 0; i < save.partyMembers.Length; i++) {
            PartyMember[i].gameObject.SetActive(true);
            PartyMember[i].sprite = Resources.Load<Sprite>("Sprites/" + save.partyMembers[i]);
        }

        //Set Extra Members
        for (int i = 4; i < save.partyMembers.Length+4; i++)
        {
            ExtraMember[i].gameObject.SetActive(true);
            ExtraMember[i].sprite = Resources.Load<Sprite>("Sprites/" + save.partyMembers[i]);
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
