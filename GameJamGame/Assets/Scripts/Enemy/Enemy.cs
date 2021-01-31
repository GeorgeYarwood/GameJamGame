using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    float visibilitycone = 30f;

    float attackdist = 3f;
    float approachdist = 10f;

    float dmg = 0.05f;

    GameObject CurrPlayer;

    public GameObject Goal1;
    public GameObject Goal2;


    Animator anim;

    bool walking;

    //Stuff for the AI
    NavMeshAgent agent;
    enum States { Idle, Walking, Attacking};

    States currState;


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


    //Update is called once per frame
    void Update()
    {


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
               

                if (!(Player.detectionPercent <= 0))
                {
                    Player.detectionPercent -= 0.03f;

                }


                //If we're close enough to the player
                if (distance <= attackdist)
                {
                    //Attack dem
                    currState = States.Attacking;
                }

                //If we can be seen
                if (distance <= approachdist)
                {
                    //Approach dem
                    currState = States.Walking;
                }
                break;

            case States.Walking:
                anim.SetTrigger("Walk");

                if (!(Player.detectionPercent <= 0))
                {
                    Player.detectionPercent -= 0.01f;
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
                //anim.SetTrigger("Attack");
                //Set target to current position to stop movement
                agent.destination = transform.position;

                Vector3 targetDir = CurrPlayer.transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);

                if (distance >= attackdist)
                {
                    //Return to walke
                    currState = States.Idle;
                }

                //Debug.Log(angle);



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
                            Debug.Log("Enemy can see player!");

                            //Start adding onto the detection percent
                            Player.detectionPercent += 0.2f;

                            if (Player.detectionPercent >= 100f)
                            {
                                //Take away damage from the players health
                                Player.DmgPlayer(dmg);
                            }
                        }
                    }
                   

                }
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

