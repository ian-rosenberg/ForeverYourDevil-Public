using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System.IO;
using System.Xml;
using UnityEngine.SceneManagement;

/**
 * @brief A collection of lines to load when a conversation is triggered or an option is chosen
 */
public class Conversation
{
    public string Id { get; set; }                            /**Conversation name identifier. Cannot be blank*/
    public AudioClip VoiceLine { get; set; }                  /**Optional Wav file containing voice line to play with conversation.*/
    public List<DialogueLine> DialogueLines { get; set; }     /**A list of dialogue lines to display*/

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
 */
public class DialogueLine
{
    public string Name { get; set; }              /**Name of Character saying the dialogue
                                                  /**If null, there will be no name displayed*/

    public string Content { get; set; }           /**Dialogue text displayed in the DialogueBox*/
    public List<Sprite> Sprites;          /**Sprites to show/hide on screen*/
    public OrderedDictionary Options;           /**Options labels and the conversation id they go to when selected*/

    /**
     * Create a DialogueLine without any options or sprites
     */
    public DialogueLine(string name, string content)
    {
        Name = name;
        Content = content;
        //Sprites = sprites;
    }

    /**
     * Create a DialogueLine without any options but with sprites
     */
    public DialogueLine(string name, string content, List<Sprite> sprites)
    {
        Name = name;
        Content = content;
        Sprites = sprites;
    }

    /**
     * Create a DialogueLine with options but no sprites
     */
    public DialogueLine(string name, string content, OrderedDictionary options)
    {
        Name = name;
        Content = content;
        //Sprites = sprites;
        Options = options;
    }

    /**
     * Create a DialogueLine with options and sprites
     */
    public DialogueLine(string name, string content, List<Sprite> sprites, OrderedDictionary options)
    {
        Name = name;
        Content = content;
        Sprites = sprites;
        Options = options;
    }

}

public class ParseXML : MonoBehaviour
{
    private static ParseXML instance;

    public static ParseXML Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ParseXML>();
            return instance;
        }
    }

    public Dictionary<string, Conversation> conversationList;   /**A list of conversations to store parsed info*/

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Started");
        //Initialize Conversation List
        conversationList = new Dictionary<string, Conversation>();

        //Load XML FILE into script
        var file = Resources.Load<TextAsset>(SceneManager.GetActiveScene().name);
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(file.text);
        Debug.Log("LoadXml");

        //Get a list of all conversations
        XmlNodeList nodelist = xml.SelectSingleNode("/game").SelectNodes("conversation"); // get all <conversation> nodes
        Debug.Log("/conversations: " + nodelist.Count);
        foreach (XmlNode conv in nodelist) // for each <conversation> node
        {
            //Create conversation Obj
            Conversation conversation = new Conversation();
            List<DialogueLine> dialogueList = new List<DialogueLine>();
            Debug.Log("Lists created");

            //Set conversationID
            if (HasAttributes(conv, "id"))
            {
                conversation.Id = conv.Attributes["id"].Value;
                Debug.Log(conv.Attributes["id"].Value);
            }
            //Set voice line (if present)
            if (HasAttributes(conv, "voice"))
            {
                conversation.VoiceLine = Resources.Load<AudioClip>("Audio/" + conv.Attributes["voice"].Value);
                Debug.Log(conv.Attributes["voice"].Value);
            }
            //Get characters
            XmlNodeList characterList = conv.SelectNodes("character");
            Debug.Log("characters: " + characterList.Count);
            foreach (XmlNode character in characterList)
            {
                //Create New SpriteList
                List<Sprite> spriteList = new List<Sprite>();

                //Store text from before in case options are specified
                string previousLine = "";

                //Store Character Name
                string characterName = "";
                if (HasAttributes(character, "name"))
                    characterName = character.Attributes["name"].Value;
                //Debug.Log(character.Attributes["name"].Value);

                //Get their lines (could be options or lines)
                XmlNodeList lineList = character.ChildNodes;
                Debug.Log("Dialog line nodes created: " + lineList.Count);
                foreach (XmlNode line in lineList)
                {
                    //Parse dialogue lines
                    if (line.Name == "line")
                    {
                        previousLine = line.InnerText;

                        //Get sprites from line
                        GetSprites(spriteList, line);

                        //Create a new dialogue line
                        DialogueLine d = new DialogueLine(characterName, line.InnerText, spriteList);
                        Debug.Log("Dialog line created");

                        //Add line to dialogue list
                        dialogueList.Add(d);
                        Debug.Log("Dialog line stored");
                    }

                    //Parse options and choices
                    else if (line.Name == "options")
                    {
                        //Create New Options List
                        OrderedDictionary optionList = new OrderedDictionary();

                        //Get all choices within option tag
                        XmlNodeList choiceList = line.ChildNodes;
                        Debug.Log("Option nodes created: " + choiceList.Count);
                        foreach (XmlNode choice in choiceList)
                        {
                            //Parse options into list
                            if (HasAttributes(choice, "id"))
                            {
                                optionList.Add(choice.InnerText, choice.Attributes["id"].Value);
                            }
                        }

                        //Create a new dialogue line
                        DialogueLine d = new DialogueLine(characterName, previousLine, optionList);
                        Debug.Log("Option list created");

                        //Add line to dialogue list
                        dialogueList.Add(d);
                        Debug.Log("Option list stored");
                    }

                } //end get lines
            } //end get characters

            //Store local lines into conversation
            conversation.DialogueLines = dialogueList;
            //Store conversation in conversationList
            conversationList.Add(conversation.Id, conversation);

        } //end get conversation

    } //end ParseXML

    /**
     * @brief Check if there are attributes within the XMLNode given
     * @param node node to check for attributes
     * @param attribute optional specific attribute to search for 
     * @return true if attributes present and optional attribute is found, else false
     */
    bool HasAttributes(XmlNode node, string attribute = "")
    {
        //Check if any attributes at all
        if (node.Attributes != null)
        {
            //Check for specific attribute given (if given)
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
     * @brief Get all sprite attributes in spriteList and add them to the spriteList given.
     * @param spriteList The list of sprites. If no sprites are specified, the last sprites given are used.
     * @param line The line from the XML script to get sprites from.
     */
    void GetSprites(List<Sprite> spriteList, XmlNode line)
    {
        //If sprite attribute is not specified, use the sprites from the previous line.
        if (HasAttributes(line, "sprite1") || HasAttributes(line, "sprite2") || HasAttributes(line, "sprite3") || HasAttributes(line, "sprite4"))
        {
            spriteList.Clear();
            //Use sprites specified
            {
                //Get sprites from line (if they exist)
                if (HasAttributes(line, "sprite1"))
                {
                    Sprite sprite = Resources.Load<Sprite>("Sprites/" + line.Attributes["sprite1"].Value);
                    Debug.Log(sprite.name);
                    spriteList.Add(sprite);
                }
                //Get sprites from line (if they exist)
                if (HasAttributes(line, "sprite2"))
                {
                    Sprite sprite = Resources.Load<Sprite>("Sprites/" + line.Attributes["sprite2"].Value);
                    spriteList.Add(sprite);
                }
                //Get sprites from line (if they exist)
                if (HasAttributes(line, "sprite3"))
                {
                    Sprite sprite = Resources.Load<Sprite>("Sprites/" + line.Attributes["sprite3"].Value);
                    spriteList.Add(sprite);
                }
                //Get sprites from line (if they exist)
                if (HasAttributes(line, "sprite4"))
                {
                    Sprite sprite = Resources.Load<Sprite>("Sprites/" + line.Attributes["sprite4"].Value);
                    spriteList.Add(sprite);
                }
            }
        }
    }

}
