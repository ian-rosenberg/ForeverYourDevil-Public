using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//On Click
//Play animation of button being selected
//Play animation of sprites moving and going to background
//Advance to the specified conversation id
//Play animation of characters moving back to foreground
//Make button disappear

public class DialogueChoice : MonoBehaviour
{
    Dialogue dialogueManager;
    public string convID;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = Dialogue.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClick()
    {
        Debug.Log("Hit button");
        anim.SetTrigger("Select");
    }

    public void FadeOut()
    {
        Debug.Log("FadeOut Called");
        anim.SetTrigger("Off");
    }

    public void ChangeConversation()
    {
        dialogueManager.ChangeConversation(convID);
    }

    public void DestroyButton()
    {
        Destroy(gameObject);
    }
}
