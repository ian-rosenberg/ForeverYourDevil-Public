using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief A button that when clicked will play an animation and skip to the conversation given
 * @author Omar Ilyas
 */

public class DialogueChoice : MonoBehaviour
{
    Dialogue dialogueManager;   /**Master dialogue manager to display conversation*/
    public string convID;       /**Conversation id to go to when clicked*/
    public Animator anim;       /**Animator to play animations from*/

    /**
     * @brief Initialize dialogue manager
     */
    void Start()
    {
        dialogueManager = Dialogue.Instance;
    }
    
    /**
     * @brief Run an animation when button is clicked (with delay)
     */
    public void OnClick()
    {
        Debug.Log("Hit button");
        anim.SetTrigger("Select");
    }

    /**
     * @brief Run an animation when conversation is being loaded
     */
    public void FadeOut()
    {
        Debug.Log("FadeOut Called");
        anim.SetTrigger("Off");
    }

    /**
     * @brief Change conversation in Dialogue manager (triggered from an animation event)
     */
    public void ChangeConversation()
    {
        dialogueManager.ChangeConversation(convID);
    }

    /**
     * @brief Delete the button gameObject upon having the choice be selected
     */
    public void DestroyButton()
    {
        Destroy(gameObject);
    }
}
