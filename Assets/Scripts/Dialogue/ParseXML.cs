using System.Collections;
using System.Collections.Generic;
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
    public Dictionary<string, string> Options;           /**Options labels and the conversation id they go to when selected*/

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
    public DialogueLine(string name, string content, Dictionary<string, string> options)
    {
        Name = name;
        Content = content;
        //Sprites = sprites;
        Options = options;
    }

    /**
     * Create a DialogueLine with options and sprites
     */
    public DialogueLine(string name, string content, List<Sprite> sprites, Dictionary<string, string> options)
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
                List<Sprite> spriteList = new List<Sprite>();

                //Store Character Name
                string characterName = "";
                if (HasAttributes(character, "name"))
                    characterName = character.Attributes["name"].Value;
                //Debug.Log(character.Attributes["name"].Value);

                //Get their lines
                XmlNodeList lineList = character.SelectNodes("line");
                Debug.Log("Dialog line nodes created: " + lineList.Count);
                foreach (XmlNode line in lineList)
                {
                    //Debug.Log("sprite1 = " + HasAttributes(line, "sprite1"));

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
                    ////Print sprites read
                    //string debug = "";
                    //for (int i = 0; i < spriteList.Count; i++)
                    //    debug += spriteList[i].name + ", ";
                    //Debug.Log("Spritelist: " + debug);
                    
                    //Create a new dialogue line
                    DialogueLine d = new DialogueLine(
                        characterName,
                        line.InnerText,
                        spriteList     //List of sprites to show
                        );
                    Debug.Log("Dialog line created");

                    //Add line to dialogue list
                    Debug.Log(d.Name);
                    Debug.Log(d.Content);
                    
                    dialogueList.Add(d);

                    Debug.Log("Dialog line stored");

                }//end get lines

            }//end get characters
            //Store local lines into conversation
            conversation.DialogueLines = dialogueList;
            //Store conversation in conversationList
            conversationList.Add(conversation.Id, conversation);

        }//end get conversation

    }//end ParseXML

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

}
