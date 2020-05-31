using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI healthText; //Text showing health

    [SerializeField]
    private TextMeshProUGUI staminaText; //Text showing stamina

    [SerializeField]
    private TextMeshProUGUI toleranceText; //Text showing stamina

    [SerializeField]
    private Image toleranceBar; //Text showing stamina

    //  [SerializeField]
    //public Image healthBar; //Text showing stamina
    //    [SerializeField]
    //public Image CharacterPortrait;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
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