using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class TEST_FMOD_guid_calling : MonoBehaviour
{

    private FMOD.Studio.EventInstance guidtest;
    [FMODUnity.EventRef]
    public string Event;

    // Start is called before the first frame update
    void Start()
    {
        Event = "{b631fc69-0919-4842-b828-99a5eeb67804}";
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
     if (Input.GetKeyDown(KeyCode.O))
=======
     /*if (Input.GetKeyDown(KeyCode.O))
>>>>>>> Rebuilding inventory
        {
            guidtest = RuntimeManager.CreateInstance(Event);
            guidtest.start();
            guidtest.release();
<<<<<<< HEAD
        }   
=======
        }*/   
>>>>>>> Rebuilding inventory
    }
}
