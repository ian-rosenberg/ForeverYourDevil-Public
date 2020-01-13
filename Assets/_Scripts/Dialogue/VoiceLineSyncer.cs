using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Syncs dialogue display with a voice line given.
 * @author Omar Ilyas
 */

public class VoiceLineSyncer : AudioSyncer
{

    public bool canAddChar; //Controls if character can be added to a line

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (m_isBeat)
        { canAddChar = true; } //Returns when coroutine is running

        else
        {
            canAddChar = false;
        }

        Debug.Log("canAddChar: " + canAddChar);
    }

    public override void OnBeat()
    {
        base.OnBeat();
        canAddChar = true;
    }

}
