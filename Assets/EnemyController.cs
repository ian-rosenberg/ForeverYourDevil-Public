using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class EnemyController : MonoBehaviour
{
    gameManager gameManager;

    public float enemySpeed;
    public GameObject encounter;
    Animator anim;
    public Action currentBehavior;
    public FieldOfView fov;

    public enum enemyState { IDLE, SPOTTED, RUNNING, COMBAT }
    public enemyState state;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameManager.Instance;
        state = enemyState.IDLE;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Apply current behavior
        //currentBehavior();
        //If see player, change to spotted and running
        if (state == enemyState.IDLE)
        {
            if (fov.canSee)
            {
                state = enemyState.SPOTTED;
                anim.SetTrigger("Spotted");
            }
        }
        if (state == enemyState.RUNNING)
        {
            //Move towards seen target
            transform.position = Vector3.Lerp(transform.position,
                fov.visibleTargets[fov.visibleTargets.Count - 1].position
                , enemySpeed);

            //If lost sight, go back to idle
            if (!fov.canSee)
            {
                state = enemyState.IDLE;
                anim.SetTrigger("Unspotted");
            }
        }

        //Have sprite always look toward the camera to appear 2d
        if (encounter.activeSelf)
        {
            encounter.transform.LookAt(Camera.main.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10 && gameManager.gameState != gameManager.STATE.COMBAT) //Player
        {
            Debug.Log("Call Combat Trigger");
            state = enemyState.COMBAT;
            gameManager.TriggerCombat(gameObject);
        }

    }

    public void Enemy_Combat()
    {
        Debug.Log("enemy turn");
    }

    //0 = Idle, 1 = Spotted, 2 = running
    public void ChangeState(int desiredState)
    {
        if (desiredState == 0) state = enemyState.IDLE;
        if (desiredState == 1) state = enemyState.SPOTTED;
        if (desiredState == 2) state = enemyState.RUNNING;
        if (desiredState == 3) state = enemyState.COMBAT;
    }
}
