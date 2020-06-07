
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PartyMember : MonoBehaviour
{
    [Header("Player")]
    public Animator anim; // animation controller for player

    public int health = 100; //Current Health of the player; 0 kills player

    public int maxHealth = 100; //Max Health the player is allowed to heal to
    public int tolerance = 25; //Current Tolerance. Max will kill the player
    public int maxTolerance = 100; //Max Tolerance before player is killed
    public int stamina = 6; // Current Stamina, allows player to do actions
    public int maxStamina = 6; //Max stamina player is allowed to have
    public PlayerGUI playerGUI; //Gui menu of the player

    [Tooltip("Specify what layer(s) to use in raycast")]
    public LayerMask layerMask;

    public NavMeshAgent agent; /**Player Agent Component for pathfinding movement*/
    public float normalSpeed, sprintSpeed;
    public Rigidbody rb;

    [Header("Game Manager")]
    public gameManager gameManager;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();


    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeHealth(health, maxHealth);
        ChangeStamina(stamina, maxStamina);
        ChangeTolerance(tolerance, maxTolerance);
    }

    // Update is called once per frame

    #region Player Stats

    public void ChangeHealth(int newHealth, int newMaxHealth)
    {
        health = newHealth;
        maxHealth = newMaxHealth;
        playerGUI.ChangeHealth(newHealth, newMaxHealth);
    }

    public void ChangeTolerance(int newTolerance, int newMaxTolerance)
    {
        tolerance = newTolerance;
        maxTolerance = newMaxTolerance;
        playerGUI.ChangeTolerance(newTolerance, newMaxTolerance);
    }

    public void ChangeStamina(int newStamina, int newMaxStamina)
    {
        stamina = newStamina;
        maxStamina = newMaxStamina;
        playerGUI.ChangeStamina(newStamina, newMaxStamina);
    }

    #endregion Player Stats

}