using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Text we show the player's current health on
    public Text HealthTxt;

    public AudioSource WinAudio;

    public GameObject DeathPnl;

    //The Players current health
    public static float PlayerHealth;

    //The base health that we reset to when we respawn
    public float BaseHealth = 100f;

    //The gameobject that we resapwn to
    public GameObject RespawnLoc;

    public static Slider stealthslider;

    // Start is called before the first frame update
    void Start()
    {

        stealthslider = GameObject.FindObjectOfType<Slider>();
    }
    private void Awake()
    {
        PlayerHealth = BaseHealth;
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
        if (Physics.Raycast(ray, out hit, 5f))
        {
            if (hit.transform.tag == "Artifact")
            {
                //Pickup artifact
                Debug.Log("Looking at artifact");
                //Show UI
                if (Input.GetKeyDown("f"))
                {
                    Debug.Log("Picked up artifact");
                    WinAudio.Play();
                };
            }

        }

        //Update text showing health
        HealthTxt.text = "Health " + Mathf.Floor(PlayerHealth).ToString();

        if (PlayerHealth <= 0f) 
        {
            //Player dies if their health gets below zero
            DeathPnl.SetActive(true);

        }

    }
}
