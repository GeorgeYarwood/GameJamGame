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
    public GameObject WinPnl;
    public GameObject pickupPnl;

    //The Players current health
    public static float PlayerHealth;

    //The base health that we reset to when we respawn
    public float BaseHealth = 100f;

    public Animator artifactAnim;

    //The gameobject that we resapwn to
    public GameObject RespawnLoc;

    public static Slider stealthslider;

    public static float detectionPercent = 0;

    static bool ded = false;

    GameObject parent;

    public string scr;

    bool completedgame= false;



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
        if(!(PlayerHealth < 0))
         {
            PlayerHealth -= amount;
        }
        else 
        {
            ded = true;
          
        }
      
    }


    IEnumerator waitforenable() 
    {

        yield return new WaitForSeconds(0.5f);
        (GetComponent(scr) as MonoBehaviour).enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Respawn() 
    {
        ded = false;
        //Disable the panel that shows on death
        DeathPnl.SetActive(false);
        //Reset health to the starting value
        PlayerHealth = BaseHealth;
        //Move the player back to the spawnpoint
        transform.position = RespawnLoc.transform.position;
       

        StartCoroutine(waitforenable());
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        for(int i =0; i<enemies.Length; i++) 
        {
            enemies[i].Reset();
        }
         
    }

    // Update is called once per frame
    void Update()
    {

        if (ded)
        {
            if ((GetComponent(scr) as MonoBehaviour).enabled == true)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                (GetComponent(scr) as MonoBehaviour).enabled = false;
            }
            DeathPnl.SetActive(true);
        }
        else 
        {
            DeathPnl.SetActive(false);
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f))
        {
            if (hit.transform.tag == "Artifact" && !completedgame)
            {
                //Pickup artifact
                Debug.Log("Looking at artifact");
                //Show UI
                pickupPnl.SetActive(true);

                if (Input.GetKeyDown("f"))
                {
                    Debug.Log("Picked up artifact");
                    WinAudio.Play();

                    artifactAnim.SetTrigger("GameWon");
                    completedgame = true;
                };
            }
            else 
            {
                pickupPnl.SetActive(false);
            }

        }

        //Update text showing health
        HealthTxt.text = "Health " + Mathf.Floor(PlayerHealth).ToString();



        if (completedgame) 
        {
            PlayerHealth = 1000000000f;
            WinPnl.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            (GetComponent(scr) as MonoBehaviour).enabled = false;
        }
    }
}
