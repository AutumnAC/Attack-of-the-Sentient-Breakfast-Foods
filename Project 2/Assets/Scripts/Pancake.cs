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

    // The countdown timer
    [SerializeField] private int cooldownTimerAmount;
    private int cooldownTimer = 0;

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

                // If the counter has finished counting down
                if (cooldownTimer == 0)
                {
                    // Set the collision flag to false
                    physicsObject.IsCollidingWithAgent = false;

                    // Reset the countdown timer
                    cooldownTimer = cooldownTimerAmount;

                    // Set the max speed and max force back to the default
                    physicsObject.MaxSpeed = physicsObject.DefaultMaxSpeed;
                    maxForce = defaultMaxForce;

                    // Give the pancake a boost in the direction of the current velocity so that it can speed up again
                    physicsObject.ApplyForce(physicsObject.Velocity);
                }

                // If the counter isn't at 0 but the pancake's collision flag is still on
                else if (physicsObject.IsCollidingWithAgent)
                {
                    spriteRenderer.color = Color.green; // temp debugging code

                    // Subtract from the countdown timer
                    cooldownTimer--;

                    // Slow the pancake down
                    physicsObject.MaxSpeed = physicsObject.MaxSpeed / 5;

                    // Decrease max force to decrease jitter
                    maxForce = maxForce / 3;
                }

                // If there is a target in range and the timer isn't currently counting down, switch states
                if (target != null && cooldownTimer == cooldownTimerAmount)
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

                // If there is no target or if the pancake has just collided with a waffle, go back to patrolling
                if (target == null || physicsObject.IsCollidingWithAgent)
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
