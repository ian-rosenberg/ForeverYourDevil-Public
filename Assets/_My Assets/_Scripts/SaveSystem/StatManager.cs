using System;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public float startTime = 0; /**Time from before game was loaded (from save)*/
    public float gameTime; /**Elapsed scaled time that has passed in game*/
    public int encounters;
    public int faints;
    public int gameOvers;
    public int damageDealt;
    public int damageReceived;
    public int enemiesDefeated;

    private float timeUpdateTimer; /**Delay before time is updated*/

    //Singleton creation
    private static StatManager instance;

    public static StatManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<StatManager>();
            return instance;
        }
    }

    private void Start()
    {
    }

    public void FixedUpdate()
    {
        timeUpdateTimer += Time.deltaTime;
        if (timeUpdateTimer > 1) //Update timer every 1 seconds
        { gameTime = Time.time + startTime; }
    }

    public float GetTimeInSeconds()
    {
        gameTime = Time.time + startTime;
        return gameTime;
    }

    public string timeToString(float time)
    {
        //Separate time into readable numbers
        double hours = Math.Floor(time / 3600); //Should return 0 if time is not above 0
        double minutes = Math.Floor((time / 60) % 60);
        double seconds = Math.Floor(time % 60);

        return hours + ":" + minutes + ":" + seconds;
    }
}