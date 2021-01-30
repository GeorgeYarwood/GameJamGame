﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    float visibilitycone = 20f;

    float attackdist = 20f;
    float approachdist = 30f;

    float dmg = 10f;

    GameObject CurrPlayer;



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

    //Update is called once per frame
    void Update()
    {

        //Distance between enemy and the player
        float distance = Vector3.Distance(CurrPlayer.transform.position, transform.position);


        switch (currState) 
        {
            case States.Idle:

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

                if (distance <= approachdist)
                {
                    Vector3 target = new Vector3(CurrPlayer.transform.position.x - 4f, CurrPlayer.transform.position.y, CurrPlayer.transform.position.z);
                    //Young AI go for it
                    agent.destination = target;
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

                Vector3 targetDir = CurrPlayer.transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);

                if (distance >= attackdist)
                {
                    //Return to walke
                    currState = States.Idle;
                }

                //Debug.Log(angle);
                //If the player can be seen by the enemy
                if (angle < visibilitycone)
                {
                    //Take away damage from the players health
                    Player.DmgPlayer(dmg);
                    Debug.Log("Enemy can see player!");
                }
                break;

        }













        //Make our agent face the target   
        void FaceTarget()
        {
            Vector3 direction = (CurrPlayer.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

       
           
    }

  
}
