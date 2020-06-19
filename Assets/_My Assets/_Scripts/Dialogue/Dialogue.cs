using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

/**
 * @brief Manager for displaying images, text, etc from parsed dialogue from ParseXML
 * @author Omar Ilyas (edited by Ashley Roesler)
 */

public class Dialogue : MonoBehaviour
{
    ParseXML parser;                        /**Parser index containing all sentences in List*/
    gameManager gm;                         /**Master manager controlling game state*/

    private static Dialogue instance;       /**Create singleton instance*/

    public static Dialogue Instance         /**Create singleton instance*/
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Dialogue>();
            return instance;
        }
    }

    private const float NAME_BOX_WIDTH = 370.0f;

    private string currentId;                       /**Conversation id to choose*/
    private int sentenceIndex;                      /**Index of sentence to go to*/

    [Header("Audio")]
    public FMOD.Studio.EventInstance dialogueAudio;              /**Voice line audio source*/

    [Header("Display")]
    public GameObject Canvas;               /**Canvas holding dialogue box, etc*/

    public TextMeshProUGUI nameBox;         /**Display name if given*/
    public TextMeshProUGUI textDisplay;     /**Display for text*/
    public RectTransform nameRect;          /**NameBox rectangle*/
    public Image nameBoxFrame;              /**Background frame of textbox*/
    public Image textBoxFrame;              /**Background frame of textbox*/
    public Animator LeftmostChar;           /**Leftmost Character*/
    public Animator RightmostChar;          /**Rightmost Character*/
    public Animator canvasAnim;             /**Animator object to turn on exit animation*/

    [Header("Text and choices")]
    public float textDelay = 0.001f;        /**Delay between each character while displaying text*/

    public Transform choiceArea;            /**Area where choice objects are spawned*/
    public GameObject choicePrefab;
    public float choiceDist;                /**Distance between choices*/
    public GameObject AdvanceSprite;        /**Set Active if line is done*/

    //Colors
    public Color tanOrange = new Color(0.8018868f, 0.304684f, 0.1777768f); /**Text color*/

    public Color pennyGreen = new Color(0.8018868f, 0.304684f, 0.1777768f); /**Text color*/
    public Color cerulianBlue = new Color(0.1764706f, 0.6452591f, 0.8f);   /**Text color*/

    private bool canPress = false;                  /**Is the user allowed to advance the text?*/
    private bool skip = false;                      /**Display all characters at once if true, one at a time if false*/

    private bool[] isTalking = { false, false };    /**Is one of the characters talking?*/

    [Header("Player Controls")]
    public PlayerControls pControls;


    //private void OnEnable()
    //{
    //    pControls = new PlayerControls();

    //    pControls.UI.Interact.performed += AdvanceSkipDialogue;

    //    pControls.UI.Interact.Enable();
    //}

    //private void OnDisable()
    //{
    //    pControls.UI.Interact.performed -= AdvanceSkipDialogue;

    //    pControls.UI.Interact.Disable();
    //}

    /**
        * @brief Initialize dialogue manager and get parsed dialogue from ParseXML
        */
    private void Start()
    {
        canPress = false;
        sentenceIndex = 0;

        parser = ParseXML.Instance;
        gm = gameManager.Instance;
        //source = /*spectrumManager.source =*/ GetComponent<AudioSource>();
        InitializeDialogue();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            //Advance/Skip Dialogue on KeyPress
            if (gm.gameState == gameManager.STATE.TALKING) //Return = enter key
            {
                if (canPress)
                {
                    AdvanceLine(); //Display line of text
                }
                else if (textDisplay.text.Length > 5)
                {
                    skip = true;
                    Debug.Log("Skip = true");
                }
            }
        }
    }

    /**
     * @brief Main game loop. Advance line of text or skip it depending on input.
     */

    private void AdvanceSkipDialogue(InputAction.CallbackContext context)
    {
        //Advance/Skip Dialogue on KeyPress
        if (gm.gameState == gameManager.STATE.TALKING) //Return = enter key
        {
            if (canPress)
            {
                AdvanceLine(); //Display line of text
            }
            else if (textDisplay.text.Length>5)
            {
                skip = true;
                Debug.Log("Skip = true");
            }
        }
    }

    /**
     * @brief Initialize/Clear dialogue box for a new set of dialogue
     */
    void InitializeDialogue()
    {
        //Clear and hide Namebox
        nameBox.gameObject.transform.parent.gameObject.SetActive(false); //Replace with fade out animation
        nameBox.text = "";

        //Clear dialogue box and reset color
        textDisplay.text = "";
        SetFrameTextColor(tanOrange, tanOrange);
    }

    /**
     * @brief Activate dialogue box and load from conversation specified.
     * @param conversationID id of conversation to load
     */
    public void TriggerDialogue(string conversationID)
    {
        InitializeDialogue();
        StartCoroutine(StartDialogue(conversationID, true));
    }

    /**
     * @brief Waits for animation to finish before starting dialogue. (private)
     */

    private IEnumerator StartDialogue(string convID, bool start)
    {
        AdvanceSprite.SetActive(false);
        canPress = false;
        sentenceIndex = 0;

        //Set gamestate
        if (start)
        {
            gm.ChangeState(gameManager.STATE.TALKING);

            //Turn on Canvas
            Canvas.SetActive(true);
            textDisplay.transform.parent.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            //Destroy Buttons
            foreach (Transform button in choiceArea)
            {
                Debug.Log(button.gameObject.name);
                button.gameObject.GetComponent<Animator>().SetTrigger("Off");
            }

            //Wait for fadeout animation to end
            canvasAnim.SetTrigger("Choice");
            yield return new WaitForSeconds(1.533f);
        }

        //Get conversation
        currentId = convID;

        // set animations
        SetAnimations(parser.conversationList[currentId].DialogueLines, 0);

        //Wait for animation to end before starting line and voice
        if (start) yield return new WaitForSeconds(1.717f);

        if (parser.conversationList[currentId].VoiceLine != null)
        {
            dialogueAudio = RuntimeManager.CreateInstance(parser.conversationList[currentId].VoiceLine); //Set voiceline

            //Initialise FMOD Parameters
            RuntimeManager.StudioSystem.setParameterByName("LineNumber", 0);
            RuntimeManager.StudioSystem.setParameterByName("SectionNumber", 0);
            RuntimeManager.StudioSystem.setParameterByName("DialogueEnd", 0);

            //Play Audio
            dialogueAudio.start();

            Debug.Log(LeftmostChar.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            Debug.Log(RightmostChar.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        }
        //Go to next line
        AdvanceLine();
    }

    /**
     * @brief Waits for animation to finish before turning off dialogue canvas
     */
    private IEnumerator EndDialogue()
    {
        Debug.Log("End Dialogue Called");
        textDisplay.transform.parent.transform.parent.gameObject.SetActive(false);
        canvasAnim.SetTrigger("Exit");
        yield return new WaitForSeconds(0.633f);
        Canvas.SetActive(false);
        RuntimeManager.StudioSystem.setParameterByName("DialogueEnd", 1);
        gm.ChangeState(gameManager.STATE.TRAVELING);
    }

    /**
     * @brief Advance one line in conversation dialogue list chosen and display on screen
     */

    public void AdvanceLine()
    {
        Debug.Log("Advance Line Called");
        Debug.Log("SentenceIndex: " + sentenceIndex);

        canPress = false;
        AdvanceSprite.SetActive(false);

        //Get current conversation
        List<DialogueLine> dialog = parser.conversationList[currentId].DialogueLines;

        //If there are no more lines
        if (sentenceIndex >= dialog.Count)
        {
            Debug.Log("END");
            //Disable conversation box (replace with animation)
            StartCoroutine(EndDialogue());
        }

        //else if the next line is a set of options
        else if (dialog[sentenceIndex].Options != null)
        {
            float multiplier = 1;
            foreach (DictionaryEntry option in dialog[sentenceIndex].Options)
            {
                //Place each subsequent choice higher than the other
                Vector2 pos = new Vector2(0, choiceDist * multiplier);

                //Create button and set properties
                var choice = Instantiate(choicePrefab, pos, Quaternion.identity);
                choice.transform.SetParent(choiceArea.transform, false); //Set child into parent object
                choice.GetComponent<DialogueChoice>().convID = (string)option.Value; //Set Conversation ID of button
                choice.GetComponentInChildren<TextMeshProUGUI>().text = (string)option.Key; //Set Button Text

                //Increment position
                multiplier++;
            }
        }
        else //If there are more lines
        {
            //Play voice line (stop and start for workaround)
            RuntimeManager.StudioSystem.setParameterByName("LineNumber", sentenceIndex); //Advance Line Number In FMOD
            dialogueAudio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            dialogueAudio.start(); // <---DUMB

            // set animations
            SetAnimations(dialog, sentenceIndex);

            Debug.Log(LeftmostChar.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            Debug.Log(RightmostChar.GetCurrentAnimatorClipInfo(0)[0].clip.name);

            //Set name
            if (!dialog[sentenceIndex].Name.Equals(""))
            {
                nameBox.gameObject.transform.parent.gameObject.SetActive(true); //Replace with fade in animation
                nameBox.text = dialog[sentenceIndex].Name;
                    
                //Give special color/frame image to special names
                switch(nameBox.text)
                {
                    case "Adult":
                        SetFrameTextColor(cerulianBlue, cerulianBlue);
                        break;
                    case "Penny":
                        SetFrameTextColor(pennyGreen, pennyGreen);
                        break;
                    default:
                        SetFrameTextColor(tanOrange, tanOrange);
                        break;
                }

                // resize name box according to length of name (sets right bound of name box)
                float size = nameBox.GetPreferredValues(nameBox.text).x - NAME_BOX_WIDTH;
                nameRect.offsetMax = new Vector2(size, nameRect.offsetMax.y);
            }
            else //if no name
            {
                nameBox.gameObject.transform.parent.gameObject.SetActive(false); //Replace with fade out animation
                SetFrameTextColor(Color.black, Color.black);
            }
            textDisplay.text = ""; //Reset Text to blank

            //Display line to read from conversationlist
            StartCoroutine(TypeText(dialog[sentenceIndex].Content));
            sentenceIndex++;
        }
    }

    /**
     * @brief Sets the current left and right animations
     */
    void SetAnimations(List<DialogueLine> dialog, int index)
    {
        // set animation for leftmost character
        if (dialog[index].AC_Array[0])
        {
            LeftmostChar.runtimeAnimatorController = dialog[index].AC_Array[0];
        }
        if (dialog[index].Emotion_Array[0] != DialogueLine.Emotion.NONE)
        {
            if (ContainsParam(LeftmostChar, dialog[index].Emotion_Array[0].ToString()))
            {
                // reset triggers
                ClearTriggers(LeftmostChar);

                LeftmostChar.SetTrigger(dialog[index].Emotion_Array[0].ToString());
                isTalking[0] = dialog[index].isTalking[0];
            }
            else
            {
                LeftmostChar.SetTrigger("MakeDefault");
                isTalking[0] = false;
            }
        }

        // reset talking
        else if (isTalking[0])
        {
            LeftmostChar.SetTrigger("StartTalk");
        }

        // set animation for rightmost character
        if (dialog[index].AC_Array[1])
        {
            RightmostChar.runtimeAnimatorController = dialog[index].AC_Array[1];
        }
        if (dialog[index].Emotion_Array[1] != DialogueLine.Emotion.NONE)
        {
            if (ContainsParam(RightmostChar, dialog[index].Emotion_Array[1].ToString()))
            {
                // reset triggers
                ClearTriggers(RightmostChar);

                RightmostChar.SetTrigger(dialog[index].Emotion_Array[1].ToString());
                isTalking[1] = dialog[index].isTalking[1];
            }
            else
            {
                RightmostChar.SetTrigger("MakeDefault");
                isTalking[1] = false;
            }
        }

        // reset talking
        else if (isTalking[1])
        {
            RightmostChar.SetTrigger("StartTalk");
        }
    }

    /**
     * @brief resets the triggers of the given animator
     */
    private void ClearTriggers(Animator anim)
    {
        foreach(AnimatorControllerParameter p in anim.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Trigger)
            {
                anim.ResetTrigger(p.name);
            }
        }
    }

    /**
     * @brief Checks if the given animator has a parameter with the given string name
     */
    bool ContainsParam(Animator anim, string param)
    {
        foreach (AnimatorControllerParameter acp in anim.parameters)
        {
            if (acp.name == param)
            {
                return true;
            }
        }
        return false;
    }

    /**
     * @brief Change the conversation and conversation id upon clicking a choice
     * @param convID the conversation to go to upon button click
     */

    public void ChangeConversation(string convID)
    {
        Debug.Log("Running Change Conversation : " + convID);
        //Change ID and reset sentence index to the beginning
        currentId = convID;
        sentenceIndex = 0;

        //Start new dialogue
        dialogueAudio.release();
        dialogueAudio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        RuntimeManager.StudioSystem.setParameterByName("DialogueEnd", 1);
        StartCoroutine(StartDialogue(convID, false));
    }

    /**
     * @brief Coroutine that displays text char by char until line is exhausted or skipped
     * @param s string to display
     */
    private IEnumerator TypeText(string s)
    {
        Debug.Log("Running coroutine.");

        char[] chars = s.ToCharArray();

        //Increment through all characters in string
        for (int i = 0; i < chars.Length; i++)
        {
            if (skip)
            {
                //Add all text and stop loop
                textDisplay.text = s;
                break;
            }
            else
            {
                textDisplay.text += chars[i];
                ////Add delay for certain punctuation
                //if (new Regex(@"^[,.;:]*$").IsMatch(chars[i].ToString()))
                //    yield return new WaitForSeconds(textDelay + 0.37f);
                //else if (new Regex(@"^[?!]*$").IsMatch(chars[i].ToString()))
                //    yield return new WaitForSeconds(textDelay + 0.16f);
                // else
                yield return new WaitForSeconds(textDelay);
            }
        }

        // stop talking animations
        if (isTalking[0])
        {
            LeftmostChar.SetTrigger("StopTalk");
        }

        if (isTalking[1])
        {
            RightmostChar.SetTrigger("StopTalk");
        }

        //Allow advancement
        skip = false;
        Debug.Log("skip = false");
        canPress = true;
        AdvanceSprite.SetActive(true);
    }

    /**
     * @brief Change color of background frame and text
     * @param frameColor the new color of the background frame
     * @param textColor the new color of the text
     */

    private void SetFrameTextColor(Color frameColor, Color textColor)
    {
        textDisplay.color = textColor;
        nameBox.color = textColor;
        textBoxFrame.color = frameColor;
        nameBoxFrame.color = frameColor;
    }
}