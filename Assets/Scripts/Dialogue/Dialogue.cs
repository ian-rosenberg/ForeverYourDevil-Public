using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class Dialogue : MonoBehaviour
{
    ParseXML parser; //Parser index containing all sentences in List
    gameManager gameManager;

    private static Dialogue instance;

    public static Dialogue Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Dialogue>();
            return instance;
        }
    }
    string currentId; //Conversation id to choose;
    int sentenceIndex; //Index of sentence to go to

    [Header("Audio")]
    //public AudioSpectrum spectrumManager;
    //public VoiceLineSyncer voiceManager;
    public AudioSource source;

    [Header("Display")]
    public GameObject Canvas; //Canvas holding dialogue box and stuff

    public TextMeshProUGUI nameBox; //Display name if given
    public TextMeshProUGUI textDisplay; //Display for text
    public Image nameBoxFrame; //Background frame of textbox
    public Image textBoxFrame; //Background frame of textbox
    public Animator canvasAnim; //Animator object to turn on exit animation

    public float textDelay = 0.001f;

    public GameObject AdvanceSprite; //Set Active if line is done

    //Colors
    Color tanOrange = new Color(0.8018868f, 0.304684f, 0.1777768f);
    Color cerulianBlue = new Color(0.1764706f, 0.6452591f, 0.8f);

    bool canPress = false;
    bool skip = false; //Skip text

    private void Start()
    {
        parser = ParseXML.Instance;
        gameManager = gameManager.Instance;
        canPress = false;
        sentenceIndex = 0;
        source = /*spectrumManager.source =*/ GetComponent<AudioSource>();
        InitializeDialogue();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Advance/Skip Dialogue no KeyPress
        if (Input.anyKeyDown && gameManager.gameState == gameManager.STATE.TALKING) //Return = enter key
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
     * @brief Clear dialogue box for a new set of dialogue
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
     * @brief Activate dialogue box and load from this conversation.
     * @param conversationID id of conversation to load
     */

    public void TriggerDialogue(string conversationID)
    {
        InitializeDialogue();
        StartCoroutine(StartDialogue(conversationID));
    }

    /**
     * @brief Waits for animation to finish before starting dialogue. (private)
     */
    IEnumerator StartDialogue(string convID)
    {
        //Set gamestate
        gameManager.gameState = gameManager.STATE.TALKING;

        //Turn on Canvas
        Canvas.SetActive(true);
        textDisplay.transform.parent.transform.parent.gameObject.SetActive(true);

        currentId = convID;
        // Debug.Log(parser.conversationList[currentId].VoiceLine.name); //Set voiceline

        //Wait for animation to end before starting line and voice
        yield return new WaitForSeconds(1.917f);

        if (parser.conversationList[currentId].VoiceLine)
            source.clip = parser.conversationList[currentId].VoiceLine; //Set voiceline
        sentenceIndex = 0;

        //Play voice
        if (source.clip)
            source.Play();

        AdvanceLine();
    }

    /**
     * @brief Waits for animation to finish before turning off dialogue canvas
     */
    IEnumerator EndDialogue()
    {
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
        canPress = false;
        AdvanceSprite.SetActive(false);

        //Get current conversation
        List<DialogueLine> dialog = parser.conversationList[currentId].DialogueLines;

        //If there are no more lines
        if (sentenceIndex >= dialog.Count)
        {
            Debug.Log("END");
            //Disable conversation box (replace with animation)
            textDisplay.transform.parent.transform.parent.gameObject.SetActive(false);
            StartCoroutine(EndDialogue());
        }
        else //If there are more lines
        {
            //Set name
            if (!dialog[sentenceIndex].Name.Equals(""))
            {
                nameBox.gameObject.transform.parent.gameObject.SetActive(true); //Replace with fade in animation
                nameBox.text = dialog[sentenceIndex].Name;
                //Give special color/frame image to special names
                if (nameBox.text == "Blue Pill")
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
     * @brief Coroutine that displays text char by char until line is exhausted
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
                //yield return new WaitForSeconds(0.00f);
            }
            else/* if (voiceManager.canAddChar) //Set by VoiceLineSyncer*/
            {
                textDisplay.text += chars[i];
                //yield return new WaitForSeconds(textDelay);

                //Add delay for certain punctuation
                if (new Regex(@"^[,.;:]*$").IsMatch(chars[i].ToString()))
                    yield return new WaitForSeconds(textDelay + 0.37f);
                else if (new Regex(@"^[?!]*$").IsMatch(chars[i].ToString()))
                    yield return new WaitForSeconds(textDelay + 0.16f);
                else
                    yield return new WaitForSeconds(textDelay);
                //}
            }
            //else
            //{
            //    i--;
            //    yield return new WaitForFixedUpdate();
            //}
        }
        skip = false;
        Debug.Log("skip = false");
        canPress = true;
        AdvanceSprite.SetActive(true);
    }

    /**
     * @brief Change color of background frame and text
     */
    void SetFrameTextColor(Color frameColor, Color textColor)
    {
        textDisplay.color = textColor;
        nameBox.color = textColor;
        textBoxFrame.color = frameColor;
        nameBoxFrame.color = frameColor;
    }
}
