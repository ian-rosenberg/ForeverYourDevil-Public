using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button resume;
    public Button inventory;
    public Button settings;
    public Button quit;
    
    // Start is called before the first frame update
    void Awake()
    {
        resume.onClick.AddListener( delegate { gameManager.Instance.UnPauseGame(); } );

        inventory.onClick.AddListener(delegate { gameManager.Instance.OpenInventory(); });

        //settings.onClick.AddListener();

        resume.onClick.AddListener(delegate { Application.Quit(); });
    }
}
