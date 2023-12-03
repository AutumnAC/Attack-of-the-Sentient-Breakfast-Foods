using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Enum for the pancake's state
public enum PancakeStates
{
    Flock,
    Seek,
}


// Class purpose: Defines behavior for the Pancake agent

public class Pancake : Agent
{
    // The target to be used in Seek
    [SerializeField] private Waffle target;

    // State enum
    private PancakeStates currentState;

    /// <summary>
    /// Calculates the steering forces that affect the pancake.
    /// </summary>
    protected override void CalcSteeringForces()
    {
        // This is almost certainly bad practice for an FSM and will be revised
        // Check to see if the Waffles are nearby every frame to determine the current state

        target = FindClosestWaffle(4);

        if (target == null)
        {
            currentState = PancakeStates.Flock;
        }

        else
        {
            currentState = PancakeStates.Seek;
        }

        switch (currentState)
        {
            case PancakeStates.Flock:
                // Set the sprite color to normal
                spriteRenderer.color = Color.white;

                // Follow the flow field -- experimental code
                //ultimaForce += FollowFlowField() * flowFieldScalar;

                // Get a wander force
                wanderForce = Wander(ref wanderAngle, 20f, .2f, .5f);

                // Multiply it by its weight
                wanderForce *= wanderScalar;

                // Add it to the wander force
                ultimaForce += wanderForce;

                // Flocking
                ultimaForce += Seek(Manager.Instance.CenterPoint);
                ultimaForce += Manager.Instance.SharedDirection * maxSpeed - physicsObject.Velocity;

                break;

            case PancakeStates.Seek:

                ultimaForce += Seek(target.transform.position) * seekScalar;

                spriteRenderer.color = Color.red;

                /*target = FindClosestWaffle(3).gameObject;

                // As long as there is a target
                if (target != null)
                {
                    // Seek it
                    ultimaForce += Seek(target.transform.position) * seekScalar;
                }

                // If there is no target, go back to patrolling
                else
                {
                    currentState = PancakeStates.Patrol;
                }*/

                break;
        }

        // Always do these things, no matter the state:

        // Get the bounds force and scale it
        boundsForce = StayInBounds(boundsTime) * boundsScalar;

        // Add the bounds force to the ultimate force
        ultimaForce += boundsForce;

        // Add the obstacle avoidance force to the ultimate force
        ultimaForce += AvoidObstacles() * obstaclesScalar;

        // Add the scaled separation force to the ultimate force
        ultimaForce += SeparatePancakes() * separateScalar;
    }

    /// <summary>
    /// Initializes the pancake -- called in parent's Awake() function.
    /// </summary>
    protected override void Init()
    {
        // Set up the main camera
        physicsObject.MainCamera = Manager.Instance.MainCamera;

        // Initialize a random angle for the wander angle
        wanderAngle = UnityEngine.Random.Range(-maxWanderAngle, maxWanderAngle);

        // Initialize a starting velocity so that the velocity is not zero to begin with
        physicsObject.Velocity = Vector3.up;

        // Set up the sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

}
