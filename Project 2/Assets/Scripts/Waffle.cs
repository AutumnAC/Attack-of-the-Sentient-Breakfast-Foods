using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum for the Waffle's state
public enum WaffleStates
{
    Wander,
    Flee,
}

// Class purpose: Defines behavior for the Waffle agent

public class Waffle : Agent
{
    // The target for Flee
    [SerializeField] private GameObject target;

    // State enum
    private WaffleStates currentState;


    protected override void CalcSteeringForces()
    {
        // Check the state -- does very little at present
        switch(currentState)
        {
            // If the waffle is wandering
            case WaffleStates.Wander:

                // Get a wander force
                wanderForce = Wander(ref wanderAngle, 20f, .2f, .5f);

                // Multiply it by its weight
                wanderForce *= wanderScalar;

                // Add it to the wander force
                ultimaForce += wanderForce;

                break;

            // If the waffle is fleeing
            case WaffleStates.Flee:

                // For now, do nothing -- just getting out ahead of the next checkpoint

                /*target = FindClosestWaffle(); // will be updated to pancake later

                // must check if target is null-- be careful!

                fleeForce = Flee(target.GetComponent<PhysicsObject>());

                fleeForce *= fleeScalar;

                ultimaForce += fleeForce;*/

                break;
        }

        // Always do these things, no matter the state:

        // Get the bounds force and scale it
        boundsForce = StayInBounds(boundsTime) * boundsScalar;

        // Add the bounds force to the ultimate force
        ultimaForce += boundsForce;

        // Add the scaled separation force to the ultimate force
        ultimaForce += Separate() * separateScalar;
    }

    /// <summary>
    /// Initializes the waffle -- called in parent's Awake() function.
    /// </summary>
    protected override void Init()
    {
        // Set a reference to the main camera
        physicsObject.MainCamera = CollisionManager.Instance.MainCamera;

        // Initialize a random angle for the wander angle
        wanderAngle = UnityEngine.Random.Range(-maxWanderAngle, maxWanderAngle);
    }

    /// <summary>
    /// Draws gizmos for a few key vectors.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + wanderForce);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + boundsForce);
    }
}
