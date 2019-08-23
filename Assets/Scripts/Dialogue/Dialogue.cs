using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class Dialogue : MonoBehaviour
{
    ParseXML parser; //Parser index containing all sentences in List

    string currentId; //Conversation id to choose;
    int sentenceIndex; //Index of sentence to go to

    [Header("Audio")]
    public AudioSpectrum spectrumManager;
    public VoiceLineSyncer voiceManager;
    public AudioSource source;

    [Header("Display")]
    public TextMeshProUGUI textDisplay; //Display for text

    public float textDelay = 0.001f;

    public TextMeshProUGUI nameBox; //Display name if given

    public GameObject AdvanceSprite; //Set Active if line is done

    bool canPress = false;
    bool skip = false; //Skip text

    private void Start()
    {
        parser = ParseXML.Instance;
        canPress = false;
        sentenceIndex = 0;
        source = spectrumManager.source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Advance/Skip Dialogue no KeyPress
        if (Input.anyKeyDown)
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
     * @brief Activate dialogue box and load from this conversation.
     */

    public void TriggerDialogue(string conversationID)
    {
        textDisplay.transform.parent.transform.parent.gameObject.SetActive(true);

        //Play animation, then activate first line of text
        currentId = conversationID;
        Debug.Log(parser.conversationList[currentId].VoiceLine.name); //Set voiceline

        source.clip = parser.conversationList[currentId].VoiceLine; //Set voiceline
        sentenceIndex = 0;

        //Play voice
        source.Play();

        AdvanceLine();
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
        if (sentenceIndex == dialog.Count - 1)
        {
            Debug.Log("END");
            //Disable conversation box (replace with animation)
            textDisplay.transform.parent.transform.parent.gameObject.SetActive(false);
        }

        //Set name
        if (!dialog[sentenceIndex].Name.Equals(""))
        {
            nameBox.gameObject.transform.parent.gameObject.SetActive(true); //Replace with fade in animation
            nameBox.text = dialog[sentenceIndex].Name;
        }
        else nameBox.gameObject.transform.parent.gameObject.SetActive(false); //Replace with fade out animation

        textDisplay.text = ""; //Reset Text to blank

        //Display line to read from conversationlist

        StartCoroutine(TypeText(dialog[sentenceIndex].Content));
        sentenceIndex++;
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
            else if (voiceManager.canAddChar) //Set by VoiceLineSyncer
            {
                textDisplay.text += chars[i];
                yield return new WaitForSeconds(textDelay);

                //    //Add delay for certain punctuation
                //    if (new Regex(@"^[,.;:]*$").IsMatch(c.ToString()))
                //        yield return new WaitForSeconds(textDelay + 0.5f);
                //    else if (new Regex(@"^[?!]*$").IsMatch(c.ToString()))
                //        yield return new WaitForSeconds(textDelay + 0.2f);
                //    else
                //        yield return new WaitForSeconds(textDelay);
                //}
            }
            else
            {
                i--;
                yield return new WaitForFixedUpdate();
            }
        }
        skip = false;
        Debug.Log("skip = false");
        canPress = true;
        AdvanceSprite.SetActive(true);
    }
}
