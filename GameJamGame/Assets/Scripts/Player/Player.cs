using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Text we show the player's current health on
    public Text HealthTxt;

    public GameObject DeathPnl;

    //The Players current health
    public static float PlayerHealth;

    //The base health that we reset to when we respawn
    public float BaseHealth = 100f;

    //The gameobject that we resapwn to
    public GameObject RespawnLoc;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void DmgPlayer(float amount) 
    {
        //Take damage from player's health
        PlayerHealth -= amount;
    }

    public void Respawn() 
    {
        //Disable the panel that shows on death
        DeathPnl.SetActive(true);
        //Reset health to the starting value
        PlayerHealth = BaseHealth;
        //Move the player back to the spawnpoint
        transform.position = RespawnLoc.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 20f))
        {
            if (hit.transform.tag == "Artifact")
            {
                //Pickup artifact
                Debug.Log("Looking at artifact");
                //Show UI
                if (Input.GetKeyDown("f"))
                {
                    Debug.Log("Picked up artifact");
                };
            }

        }



        if (PlayerHealth <= 0f) 
        {
            //Player dies if their health gets below zero
            DeathPnl.SetActive(true);

        }

    }
}
