using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum for the waffle's state
public enum WaffleStates
{
    Wander,
    Flee,
}

// Class purpose: Defines behavior for the Waffle agent

public class Waffle : Agent
{
    // State enum
    private WaffleStates currentState;

    /// <summary>
    /// Calculates the steering forces for the waffle -- called in parent's Update() function.
    /// </summary>
    protected override void CalcSteeringForces()
    {
        // If the waffle's pancake collision flag was turned on, destroy it
        if (physicsObject.IsCollidingWithAgent)
        {
            Destroy(gameObject);
        }

        // Behave differently depending on the state
        switch (currentState)
        {
            // Wander
            case WaffleStates.Wander:

                // Set the sprite color to normal
                spriteRenderer.color = Color.white;

                // Get a wander force
                wanderForce = Wander(ref wanderAngle, 20f, .2f, .5f) * wanderScalar;

                // Add it to the ultimate force
                ultimaForce += wanderForce;
                
                // If there is a pancake in range
                if (FindPancakesInRange(visionRadius).Count > 0)
                {
                    // Flee
                    currentState = WaffleStates.Flee;
                }
                // Otherwise, continue to wander

                break;

            // Flee -- waffle flees from all pancakes in range
            case WaffleStates.Flee:

                // Change the color to blue
                spriteRenderer.color = Color.cyan;

                // Get all the pancakes in range
                fleeForce = FleeFromClosePancakes(visionRadius) * fleeScalar;

                // Add the force of the fleeing to the ultimate force
                ultimaForce += fleeForce;

                // If there is no longer a pancake in range
                if (FindPancakesInRange(visionRadius).Count == 0)
                {
                    // Wander
                    currentState = WaffleStates.Wander;
                }
                // Otherwise, continue to flee from the pancakes

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
        ultimaForce += SeparateWaffles() * separateScalar;
    }

    /// <summary>
    /// Draws gizmos for a few key vectors.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw the wander force
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + wanderForce);

        // Draw the bounds force
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + boundsForce);

        // Draw a sphere based on the bounding circle collision
        Gizmos.DrawWireSphere(transform.position, visionRadius);

    }
}
