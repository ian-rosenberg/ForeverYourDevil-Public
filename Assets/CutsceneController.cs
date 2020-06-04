using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public Animator[] PennyAnim;

    public void SetBreathing(int whichOne)
    {
        PennyAnim[whichOne].SetTrigger("Breathe");
    }
    public void SetGreet(int whichOne)
    {
        PennyAnim[whichOne].SetTrigger("Greet");
    }
    public void SetDancePose(int whichOne)
    {
        PennyAnim[whichOne].SetTrigger("Dance Pose");
    }
    public void SetLoco(int whichOne)
    {
        PennyAnim[whichOne].SetTrigger("Loco");
    }
    public void SetLook(int whichOne)
    {
        PennyAnim[whichOne].SetTrigger("Look");
    }

    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
