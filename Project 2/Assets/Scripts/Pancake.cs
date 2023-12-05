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
        // Behave differently depending on the state
        switch (currentState)
        {
            // Flock -- wander within bounds and stay close to the other pancakes
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

                // Flocking -- seek the center point and move towards the shared direction
                ultimaForce += Seek(Manager.Instance.CenterPoint);
                ultimaForce += Manager.Instance.SharedDirection * maxSpeed - physicsObject.Velocity;

                // Find the closest waffle
                target = FindClosestWaffle(visionRadius);

                // If there is a target in range, switch states
                if (target != null)
                {
                    currentState = PancakeStates.Seek;
                }
                // Otherwise, stay in the same state

                break;

            // Seek -- chase after the closest pancake
            case PancakeStates.Seek:

                // Set the sprite color to red
                spriteRenderer.color = Color.red;

                // Find the closest waffle
                target = FindClosestWaffle(visionRadius);

                // As long as there is a target
                if (target != null)
                {
                    // Seek it
                    ultimaForce += Seek(target.transform.position) * seekScalar;
                }

                // If there is no target, go back to patrolling
                else
                {
                    currentState = PancakeStates.Flock;
                }

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

        // Set up the sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize maxForce
        maxForce = defaultMaxForce;

    }

}
