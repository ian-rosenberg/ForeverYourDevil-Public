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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
        sentenceIndex = 0;

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
        textDisplay.transform.parent.transform.parent.gameObject.SetActive(true);
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

        foreach (char c in s.ToCharArray())
        {
            textDisplay.text += c;

            if (skip)
            {
                textDisplay.text = s;
                break;
                //yield return new WaitForSeconds(0.00f);
            }
            else
            {
                //Add delay for certain punctuation
                if (new Regex(@"^[,.;:]*$").IsMatch(c.ToString()))
                    yield return new WaitForSeconds(textDelay + 0.5f);
                else if (new Regex(@"^[?!]*$").IsMatch(c.ToString()))
                    yield return new WaitForSeconds(textDelay + 0.2f);
                else
                    yield return new WaitForSeconds(textDelay);
            }
        }
        skip = false;
        Debug.Log("skip = false");
        canPress = true;
        AdvanceSprite.SetActive(true);
    }
}
