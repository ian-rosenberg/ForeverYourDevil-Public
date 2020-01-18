using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;


/**
 * @brief Manager for displaying images, text, etc from parsed dialogue from ParseXML
 * @author Omar Ilyas
 */

public class Dialogue : MonoBehaviour
{
    ParseXML parser;                        /**Parser index containing all sentences in List*/
    gameManager gameManager;                /**Master manager controlling game state*/

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
    string currentId;                       /**Conversation id to choose*/
    int sentenceIndex;                      /**Index of sentence to go to*/

    [Header("Audio")]
    //public AudioSpectrum spectrumManager;
    //public VoiceLineSyncer voiceManager;
    public AudioSource source;              /**Voice line audio source*/

    [Header("Display")]
    public GameObject Canvas;               /**Canvas holding dialogue box, etc*/

    public TextMeshProUGUI nameBox;         /**Display name if given*/
    public TextMeshProUGUI textDisplay;     /**Display for text*/
    public Image nameBoxFrame;              /**Background frame of textbox*/
    public Image textBoxFrame;              /**Background frame of textbox*/
    public Image LeftmostChar;              /**Leftmost Character*/
    public Image RightmostChar;             /**Rightmost Char*/
    public Animator canvasAnim;             /**Animator object to turn on exit animation*/

    [Header("Text and choices")]
    public float textDelay = 0.001f;        /**Delay between each character while displaying text*/

    public Transform choiceArea;            /**Area where choice objects are spawned*/
    public GameObject choicePrefab;
    public float choiceDist;                /**Distance between choices*/
    public GameObject AdvanceSprite;        /**Set Active if line is done*/

    //Colors
    Color tanOrange = new Color(0.8018868f, 0.304684f, 0.1777768f); /**Text color*/
    Color cerulianBlue = new Color(0.1764706f, 0.6452591f, 0.8f);   /**Text color*/

    bool canPress = false;                  /**Is the user allowed to advance the text?*/
    bool skip = false;                      /**Display all characters at once if true, one at a time if false*/

    /**
     * @brief Initialize dialogue manager and get parsed dialogue from ParseXML
     */

    private void Start()
    {
        canPress = false;
        sentenceIndex = 0;

        parser = ParseXML.Instance;
        gameManager = gameManager.Instance;
        source = /*spectrumManager.source =*/ GetComponent<AudioSource>();
        InitializeDialogue();
    }

    /**
     * @brief Main game loop. Advance line of text or skip it depending on input.
     */
    void FixedUpdate()
    {
        //Advance/Skip Dialogue no KeyPress
        if (Input.GetButtonDown("Interact") && gameManager.gameState == gameManager.STATE.TALKING) //Return = enter key
        {
            if (canPress)
            {
                AdvanceLine(); //Display line of text
            }
            else if (!textDisplay.text.Equals(""))
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
    IEnumerator StartDialogue(string convID, bool start)
    {
        AdvanceSprite.SetActive(false);
        canPress = false;
        sentenceIndex = 0;

        //Set gamestate
        if (start)
        {
            gameManager.gameState = gameManager.STATE.TALKING;

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

        //Set sprites
        if (parser.conversationList[currentId].DialogueLines[0].Sprites.Any())
        {
            if (parser.conversationList[currentId].DialogueLines[0].Sprites[0])
                LeftmostChar.sprite = parser.conversationList[currentId].DialogueLines[0].Sprites[0];
            if (parser.conversationList[currentId].DialogueLines[0].Sprites[1])
                RightmostChar.sprite = parser.conversationList[currentId].DialogueLines[0].Sprites[1];
        }

        //Wait for animation to end before starting line and voice
        if (start) yield return new WaitForSeconds(1.717f);

        if (parser.conversationList[currentId].VoiceLine)
            source.clip = parser.conversationList[currentId].VoiceLine; //Set voiceline

        //Play voice
        if (source.clip)
            source.Play();

        //Go to next line
        AdvanceLine();
    }

    /**
     * @brief Waits for animation to finish before turning off dialogue canvas
     */
    IEnumerator EndDialogue()
    {
        Debug.Log("End Dialogue Called");
        textDisplay.transform.parent.transform.parent.gameObject.SetActive(false);
        canvasAnim.SetTrigger("Exit");
        yield return new WaitForSeconds(0.633f);
        Canvas.SetActive(false);
        gameManager.gameState = gameManager.STATE.TRAVELING;
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
            //Set sprites
            if (dialog[sentenceIndex].Sprites.Any())
            {
                if (dialog[sentenceIndex].Sprites[0])
                    LeftmostChar.sprite = dialog[sentenceIndex].Sprites[0];
                if (dialog[sentenceIndex].Sprites[1])
                    RightmostChar.sprite = dialog[sentenceIndex].Sprites[1];
            }

            //Set name
            if (!dialog[sentenceIndex].Name.Equals(""))
            {
                nameBox.gameObject.transform.parent.gameObject.SetActive(true); //Replace with fade in animation
                nameBox.text = dialog[sentenceIndex].Name;
                //Give special color/frame image to special names
                if (nameBox.text == "Adult")
                    SetFrameTextColor(cerulianBlue, cerulianBlue);
                else
                    SetFrameTextColor(tanOrange, tanOrange);
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
        StartCoroutine(StartDialogue(convID, false));
    }

    /**
     * @brief Coroutine that displays text char by char until line is exhausted or skipped
     * @param s string to display
     */
    IEnumerator TypeText(string s)
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
                //Add delay for certain punctuation
                if (new Regex(@"^[,.;:]*$").IsMatch(chars[i].ToString()))
                    yield return new WaitForSeconds(textDelay + 0.37f);
                else if (new Regex(@"^[?!]*$").IsMatch(chars[i].ToString()))
                    yield return new WaitForSeconds(textDelay + 0.16f);
                else
                    yield return new WaitForSeconds(textDelay);
            }
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
    void SetFrameTextColor(Color frameColor, Color textColor)
    {
        textDisplay.color = textColor;
        nameBox.color = textColor;
        textBoxFrame.color = frameColor;
        nameBoxFrame.color = frameColor;
    }
}
