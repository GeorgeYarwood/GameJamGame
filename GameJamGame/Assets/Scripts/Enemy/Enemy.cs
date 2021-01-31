using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    float visibilitycone = 50f;

    float attackdist = 3f;
    float approachdist = 15f;

    float dmg = 0.10f;

    GameObject CurrPlayer;

    public GameObject Goal1;
    public GameObject Goal2;

    

    Animator anim;

    bool walking;

    //Stuff for the AI
    NavMeshAgent agent;
    enum States { Idle, Walking, Attacking};

    States currState;

    bool detectioncool;

    // Start is called before the first frame update
    void Start()
    {
        //Find our player in the scene
        CurrPlayer = GameObject.FindGameObjectWithTag("Player");

        //Set up navmesh
        agent = GetComponent<NavMeshAgent>();

        anim = GetComponentInChildren<Animator>();

        walking = false;

        //Start off as idle
        currState = States.Idle;

    }

    void OnDrawGizmosSelected()
    {
        //Draws gizmo in editor
        float totalFOV = visibilitycone;
        float rayRange = 10.0f;
        float halfFOV = totalFOV / 2.0f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);
    }

    IEnumerator patrolWalk() 
    {
        walking = true;
        yield return new WaitForSeconds(5f);
        walking = false;
    }


    public void Reset() 
    {
        transform.position = Goal1.transform.position;
        Player.detectionPercent = 0;
    }

    IEnumerator detectionCooldown() 
    {
        detectioncool = false;
        yield return new WaitForSeconds(5f);
        detectioncool = true;
    }
    


    //Update is called once per frame
    void Update()
    {
        if (detectioncool) 
        {
            Player.detectionPercent -= 0.02f;
            
        }

        //Distance between enemy and the player
        float distance = Vector3.Distance(CurrPlayer.transform.position, transform.position);


        switch (currState)
        {
            case States.Idle:
                anim.SetTrigger("Idle");

                //Set target to current position to stop movement
                //agent.destination = transform.position;

                //Distance between us and goal 1
                float goal1dist = Vector3.Distance(transform.position, Goal1.transform.position);
                //Distance between us and goal 2
                float goal2dist = Vector3.Distance(transform.position, Goal2.transform.position);

                Vector3 targetDir = CurrPlayer.transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);



                //If the player is within the enemies cone of vision
                if (angle < visibilitycone)
                {
                    Ray ray = new Ray(transform.position, transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 10f))
                    {

                        //If our ray isn't blocked (Player is not hiding behind something)
                        if (hit.transform.tag == "Player")
                        {
                            detectioncool = false;

                            Debug.Log("Enemy can see player!");

                           
                  

                            if (Player.detectionPercent >= 100f)
                            {
                               



                                //If we can be seen
                                if (distance <= approachdist)
                                {
                                    //Approach dem
                                    currState = States.Walking;
                                }
                                //If we're close enough to the player
                                else if (distance <= attackdist)

                                {
                                    //Attack dem
                                    currState = States.Attacking;
                                }

                             
                            }
                            //Start adding onto the detection percent
                            else
                            {
                                Player.detectionPercent += 0.8f;
                            }

                            StartCoroutine(detectionCooldown());
                        }
                    }
                }

                if (!walking) 
                {

                    anim.SetTrigger("Walk");

                    if (goal1dist < goal2dist)
                    {
                        agent.destination = Goal2.transform.position;
                    }
                    else if (goal2dist < goal1dist)
                    {
                        agent.destination = Goal1.transform.position;

                    }
                    StartCoroutine(patrolWalk());
                }
               



             

               
                break;

            case States.Walking:
                anim.SetTrigger("Walk");

                //If detection less than 60, stop searching for player
                if (Player.detectionPercent <= 70f)
                {
                    //Return to walke
                    currState = States.Idle;
                }

                    if (distance <= approachdist)
                {
                    //Vector3 target = new Vector3(CurrPlayer.transform.position.x - 4f, CurrPlayer.transform.position.y, CurrPlayer.transform.position.z);
                    //Young AI go for it
                    agent.destination = CurrPlayer.transform.position;
                }


                else
                {
                    //Return to walke
                    currState = States.Idle;
                }

                if (distance <= attackdist)
                {
                    //Return to walke
                    currState = States.Attacking;
                }

                break;

            case States.Attacking:
                FaceTarget();

                if (Player.detectionPercent <= 100f)
                {
                    //Return to walke
                    currState = States.Walking;
                }

                //Set target to current position to stop movement
                agent.destination = transform.position;

                //Take away damage from the players health
                Player.DmgPlayer(dmg);
                anim.SetTrigger("Attack");


                if (distance >= attackdist)
                {
                    //Return to walke
                    currState = States.Walking;
                }

                //Debug.Log(angle);

                break;

        }

                Player.stealthslider.value = Player.detectionPercent;







                //Make our agent face the target   
                void FaceTarget()
                {
                    Vector3 direction = (CurrPlayer.transform.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }



        }

    }

