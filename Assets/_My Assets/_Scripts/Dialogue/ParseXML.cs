using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

/**
 * @brief A collection of lines to load when a conversation is triggered or an option is chosen
 * @author Omar Ilyas
 */

public class Conversation
{
    public string Id { get; set; }                            /**Conversation name identifier. Cannot be blank*/
    public string VoiceLine { get; set; }                     /**Optional Wav file containing voice line to play with conversation.*/
    public List<DialogueLine> DialogueLines { get; set; }     /**A list of dialogue lines to display*/

    public bool isEssential = true;                           /**False if conversation is skippable*/

    public Conversation()
    {
        Id = "";
        DialogueLines = new List<DialogueLine>();
    }

    public Conversation(string id, List<DialogueLine> dialogueLines)
    {
        Id = id;
        DialogueLines = dialogueLines;
    }

    /**
     * @brief Add Dialogue line to the list
     * @param line Dialogue line object to add
     */

    public void AddLine(DialogueLine line)
    {
        DialogueLines.Add(line);
    }
}

/**
 * @brief A dialogue object containing the line and sprites to display and their information
 * @author Omar Ilyas (edited by Ashley Roesler)
 */

public class DialogueLine
{
    public enum Emotion                           /**Determines which "emotion" you want to trigger*/
    {                                             /**These match the AnimatorController Triggers*/
        MakeDefault, 
        MakeIdle,
        MakeHappy,
        MakeMad,
        MakeSad,
        MakeSadTalk,
        MakeAmazed,
        MakeAmazedTalk,
        MakeConfused,
        MakeConfusedTalk,
        MakeTalking,
        MakeSurprised, 
        NONE
    };                                          

    public string Name { get; set; }              /**Name of Character saying the dialogue*/
                                                  /**If null, there will be no name displayed*/

    public string Content { get; set; }           /**Dialogue text displayed in the DialogueBox*/

    public RuntimeAnimatorController[] AC_Array;  /**Determines characters displayed, [0] is left, [1] is right*/
    public Emotion[] Emotion_Array;               /**Determines the animations played for each character, [0] left and [1] right*/

    public OrderedDictionary Options;             /**Options labels and the conversation id they go to when selected*/

    public bool[] isTalking = { false, false };   /**Determines if characters are talking, [0] left, [1] right*/

    /**
     * @brief Create a DialogueLine without any options or sprites
     * @param name Name of character speaking
     * @param content Dialogue that the character is saying
     */

    public DialogueLine(string name, string content)
    {
        Name = name;
        Content = content;
        AC_Array = null;
        Emotion_Array = null;
    }

    /**
     * Create a DialogueLine without any options but with animations
     * @param name Name of character speaking
     * @param content Dialogue that the character is saying
     * @param ac_array Array of AnimatorControllers to use
     * @param emotion_array Array of Emotions to display
     */
    public DialogueLine(string name, string content, RuntimeAnimatorController[] ac_array, Emotion[] emotion_array)
    {
        Name = name;
        Content = content;
        AC_Array = ac_array;
        Emotion_Array = emotion_array;

        // determine if talking
        if (emotion_array != null)
        {
            isTalking[0] = emotion_array[0].ToString().Contains("Talk");
            isTalking[1] = emotion_array[1].ToString().Contains("Talk");
        }
    }

    /**
     * Create a DialogueLine with options but no animations
     * @param name Name of character speaking
     * @param content Dialogue that the character is saying
     * @param options List of choices and conversationIDs to go to
     */

    public DialogueLine(string name, string content, OrderedDictionary options)
    {
        Name = name;
        Content = content;
        AC_Array = null;
        Emotion_Array = null;
        Options = options;
    }

    /**
     * Create a DialogueLine with options and animations
     * @param name Name of character speaking
     * @param content Dialogue that the character is saying
     * @param options List of choices and conversationIDs to go to
     * @param ac_array Array of AnimatorControllers to use
     * @param emotion_array Array of Emotions to display
     */
    public DialogueLine(string name, string content, RuntimeAnimatorController[] ac_array, Emotion[] emotion_array, OrderedDictionary options)
    {
        Name = name;
        Content = content;
        AC_Array = ac_array;
        Emotion_Array = emotion_array;
        Options = options;

        // determine if talking
        if (emotion_array != null)
        {
            isTalking[0] = emotion_array[0].ToString().Contains("Talk");
            isTalking[1] = emotion_array[1].ToString().Contains("Talk");
        }
    }

    /**
     * @brief Converts the given string into an Emotion (case insensitive)
     */
    public static Emotion StringToEmotion(string str)
    {
        str = "Make" + str;

        try
        {
            return (Emotion)Enum.Parse(typeof(Emotion), str, true);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning("Missing emotion: " + e.Message);
            return Emotion.MakeDefault;
        }
    }
}

/**
 * @brief Manager that parses lines from an XML file with the current scene name and sorts them into a list
 * @author Omar Ilyas (edited by Ashley Roesler)
 */

public class ParseXML : MonoBehaviour
{
    private static ParseXML instance;                          /**Create singleton instance*/

    public static ParseXML Instance                            /**Create singleton instance*/
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ParseXML>();
            return instance;
        }
    }

    public Dictionary<string, Conversation> conversationList;   /**A list of conversations to store parsed info*/

    /**
     * @brief Parse an XML file with the current scene name and store info into a list to send to Dialogue.cs
     */

    private void Awake()
    {
        UnityEngine.Debug.Log("Started");

        // Initialize Conversation List
        conversationList = new Dictionary<string, Conversation>();

        // Load XML FILE into script
        var file = Resources.Load<TextAsset>(SceneManager.GetActiveScene().name);
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(file.text);
        UnityEngine.Debug.Log("LoadXml");

        // Get a list of all conversations
        XmlNodeList nodelist = xml.SelectSingleNode("/game").SelectNodes("conversation"); // get all <conversation> nodes
        UnityEngine.Debug.Log("/conversations: " + nodelist.Count);
        foreach (XmlNode conv in nodelist) // for each <conversation> node
        {
            // Create conversation Obj
            Conversation conversation = new Conversation();
            List<DialogueLine> dialogueList = new List<DialogueLine>();
            UnityEngine.Debug.Log("Lists created");

            // Set conversationID
            if (HasAttributes(conv, "id"))
            {
                conversation.Id = conv.Attributes["id"].Value;
                UnityEngine.Debug.Log(conv.Attributes["id"].Value);
            }

            // Set voice line (if present)
            if (HasAttributes(conv, "voice"))
            {
                //conversation.VoiceLine = Resources.Load<StudioEventEmitter>("Audio/" + conv.Attributes["voice"].Value);
                conversation.VoiceLine = conv.Attributes["voice"].Value;
                UnityEngine.Debug.Log(conv.Attributes["voice"].Value);
            }

            // Set if essential
            if (HasAttributes(conv, "isEssential"))
            {
                conversation.isEssential = conv.Attributes["isEssential"].Value.ToLower() == "true";
            }

            // Get characters
            XmlNodeList characterList = conv.SelectNodes("character");
            UnityEngine.Debug.Log("characters: " + characterList.Count);
            foreach (XmlNode character in characterList)
            {
                // Store text from before in case options are specified
                string previousLine = "";

                // Store Character Name
                string characterName = "";
                if (HasAttributes(character, "name"))
                    characterName = character.Attributes["name"].Value;

                // Get their lines (could be options or lines)
                XmlNodeList lineList = character.ChildNodes;
                UnityEngine.Debug.Log("Dialog line nodes created: " + lineList.Count);
                foreach (XmlNode line in lineList)
                {
                    // create new animation value arrays (AnimatorControllers and Emotions)
                    RuntimeAnimatorController[] ac_array = new RuntimeAnimatorController[2] { null, null };
                    DialogueLine.Emotion[] emotion_array = new DialogueLine.Emotion[2] { DialogueLine.Emotion.NONE, DialogueLine.Emotion.NONE };

                    // set default animation values as a starting point if no previous lines of dialogue
                    if (!dialogueList.Any())
                    {
                        ac_array[0] = Resources.Load<RuntimeAnimatorController>("Animations/Default_AC");
                        ac_array[1] = Resources.Load<RuntimeAnimatorController>("Animations/Default_AC");

                        emotion_array[0] = DialogueLine.Emotion.MakeDefault;
                        emotion_array[1] = DialogueLine.Emotion.MakeDefault;
                    }

                    // Parse dialogue lines
                    if (line.Name == "line")
                    {
                        previousLine = line.InnerText; // used when dealing with options

                        // get animations from line
                        GetAnimations(ac_array, emotion_array, line);

                        // create a new dialogue line
                        DialogueLine d = new DialogueLine(characterName, line.InnerText, ac_array, emotion_array);
                        UnityEngine.Debug.Log("Dialog line created");

                        // add line to dialogue list
                        dialogueList.Add(d);
                        UnityEngine.Debug.Log("Dialog line stored");
                    }

                    // Parse options and choices
                    else if (line.Name == "options")
                    {
                        // Create New Options List
                        OrderedDictionary optionList = new OrderedDictionary();

                        // Get all choices within option tag
                        XmlNodeList choiceList = line.ChildNodes;
                        UnityEngine.Debug.Log("Option nodes created: " + choiceList.Count);
                        foreach (XmlNode choice in choiceList)
                        {
                            // Parse options into list
                            if (HasAttributes(choice, "id"))
                            {
                                optionList.Add(choice.InnerText, choice.Attributes["id"].Value);
                            }
                        }

                        // Create a new dialogue line
                        DialogueLine d = new DialogueLine(characterName, previousLine, optionList);
                        UnityEngine.Debug.Log("Option list created");

                        // Add line to dialogue list
                        dialogueList.Add(d);
                        UnityEngine.Debug.Log("Option list stored");
                    }
                } // end get lines
            } // end get characters

            // Store local lines into conversation
            conversation.DialogueLines = dialogueList;
            // Store conversation in conversationList
            conversationList.Add(conversation.Id, conversation);
        } // end get conversation
    } // end ParseXML

    /**
     * @brief Check if there are attributes within the XMLNode given
     * @param node node to check for attributes
     * @param attribute optional specific attribute to search for
     * @return true if attributes present and optional attribute is found, else false
     */
    private bool HasAttributes(XmlNode node, string attribute = "")
    {
        // Check if any attributes at all
        if (node.Attributes != null)
        {
            // Check for specific attribute given (if given)
            if (attribute != "")
            {
                var nameAttribute = node.Attributes[attribute];
                if (nameAttribute != null)
                    return true;
                else
                    return false;
            }
            return true;
        }
        else return false;
    }

    /**
     * @brief Parse all animation attributes in given line
     * @param ac_array The array of AnimatorControllers. If none are specified, the last given are used.
     * @param emotion_array The array of Emotions. If none are specified, the last given are used.
     * @param line The line from the XML script to get sprites from.
     */
    void GetAnimations(RuntimeAnimatorController[] ac_array, DialogueLine.Emotion[] emotion_array, XmlNode line)
    {
        // if AC_L
        if (HasAttributes(line, "AC_L"))
        {
            ac_array[0] = Resources.Load<RuntimeAnimatorController>("Animations/" + line.Attributes["AC_L"].Value);

            // reset emotion for new AC
            emotion_array[0] = DialogueLine.Emotion.MakeDefault;

            if (ac_array[0])
            {
                UnityEngine.Debug.Log(ac_array[0].name);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Error loading AC: " + line.Attributes["AC_L"].Value);
            }
        }

        // if AC_R
        if (HasAttributes(line, "AC_R"))
        {
            ac_array[1] = Resources.Load<RuntimeAnimatorController>("Animations/" + line.Attributes["AC_R"].Value);

            // reset emotion for new AC
            emotion_array[1] = DialogueLine.Emotion.MakeDefault;

            if (ac_array[1])
            {
                UnityEngine.Debug.Log(ac_array[1].name);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Error loading AC: " + line.Attributes["AC_R"].Value);
            }
        }

        // if emotion_L
        if (HasAttributes(line, "emotion_L"))
        {
            emotion_array[0] = DialogueLine.StringToEmotion(line.Attributes["emotion_L"].Value);
            UnityEngine.Debug.Log(emotion_array[0]);
        }

        // if emotion_R
        if (HasAttributes(line, "emotion_R"))
        {
            emotion_array[1] = DialogueLine.StringToEmotion(line.Attributes["emotion_R"].Value);
            UnityEngine.Debug.Log(emotion_array[1]);
        }
    }
}