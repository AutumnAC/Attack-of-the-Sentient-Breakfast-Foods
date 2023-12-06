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
    // State enum
    private PancakeStates currentState;

    // The target to be used in Seek
    [SerializeField] private Waffle target;

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

                // Get a wander force
                wanderForce = Wander(ref wanderAngle, 20f, .2f, .5f) * wanderScalar;

                // Add it to the ultimate force
                ultimaForce += wanderForce;

                // Flocking -- seek the center point and move towards the shared direction
                ultimaForce += Seek(Manager.Instance.CenterPoint);
                ultimaForce += Manager.Instance.SharedDirection * physicsObject.MaxSpeed - physicsObject.Velocity;

                // Find the closest waffle in range
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

                // Seek the target
                ultimaForce += Seek(target.transform.position) * seekScalar;

                // Check again to find the closest waffle in range
                target = FindClosestWaffle(visionRadius);

                // If there is no target, go back to patrolling
                if (target == null)
                {
                    currentState = PancakeStates.Flock;
                }
                // Otherwise, continue chasing the target

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
}
