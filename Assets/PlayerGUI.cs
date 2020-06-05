using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI healthText; //Text showing health
    [SerializeField]
    TextMeshProUGUI staminaText; //Text showing stamina
    [SerializeField]
    TextMeshProUGUI toleranceText; //Text showing stamina
    [SerializeField]
    Image toleranceBar; //Text showing stamina

    //  [SerializeField]
    //public Image healthBar; //Text showing stamina
    //    [SerializeField]
    //public Image CharacterPortrait;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeHealth(int health, int maxHealth)
    {
        healthText.text = health + "/" + maxHealth;
    }
    public void ChangeTolerance(int tolerance, int maxTolerance)
    {
        //if tolerance = 100, kill player
        toleranceText.text = tolerance + "/" + maxTolerance;
        toleranceBar.fillAmount = (float)tolerance / maxTolerance;
    }
    public void ChangeStamina(int stamina, int maxStamina)
    {
        staminaText.text = stamina + "";
    }

}
