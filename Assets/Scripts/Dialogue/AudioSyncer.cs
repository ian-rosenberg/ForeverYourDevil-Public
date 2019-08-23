using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief A modified tutorial that extracts all data from AudioSpectrum.cs into different properties
 * @author @RenaissanceCode | modified by Omar Ilyas
 * https://github.com/coderDarren/RenaissanceCoders_UnityScripting
 */

/// <summary>
/// Parent class responsible for extracting beats from..
/// ..spectrum value given by AudioSpectrum.cs
/// </summary>
public class AudioSyncer : MonoBehaviour
{
    private int count = 0;
    /// <summary>
    /// Inherit this to cause some behavior on each beat
    /// </summary>
    public virtual void OnBeat()
    {
        Debug.Log("beat (" + count + ")" );
        count++;
        m_timer = 0;
        m_isBeat = true;
    }

    /// <summary>
    /// Inherit this to do whatever you want in Unity's update function
    /// Typically, this is used to arrive at some rest state..
    /// ..defined by the child class
    /// </summary>
    public virtual void OnUpdate()
    {
        // update audio value
        m_previousAudioValue = m_audioValue;
        m_audioValue = AudioSpectrum.spectrumValue;

        // if audio value went below the bias during this frame, beat detected!
        if (m_previousAudioValue > bias &&
            m_audioValue <= bias)
        {
            // if minimum beat interval is reached
            if (m_timer > timeStep)
                OnBeat();
        }

        // if audio value went above the bias during this frame, beat detected!
        if (m_previousAudioValue <= bias &&
            m_audioValue > bias)
        {
            // if minimum beat interval is reached
            if (m_timer > timeStep)
                OnBeat();
        }

        //if rest time reached, reset isBeat to false;
        if (m_timer > restSmoothTime) {
            m_isBeat = false;
        }

        m_timer += Time.deltaTime;
    }

    private void Update()
    {
        OnUpdate();
    }

    public float bias;
    public float timeStep;
    public float timeToBeat;
    public float restSmoothTime;

    private float m_previousAudioValue;
    private float m_audioValue;
    private float m_timer;

    protected bool m_isBeat;
}

