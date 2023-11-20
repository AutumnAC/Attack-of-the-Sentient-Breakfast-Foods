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

            // If the waffle is fleeing -- state is currently never reached
            case WaffleStates.Flee:

                // Find the closest pancake -- later, this needs to be modified to be limited to a range
                target = FindClosestPancake().gameObject;

                // If there is a target
                if (target != null)
                {
                    // Flee from the pancake
                    fleeForce = Flee(target.GetComponent<PhysicsObject>()) * fleeScalar;

                    // Add the force of the fleeing to the ultimate force
                    ultimaForce += fleeForce;
                }

                // If there is no pancake, go back to wandering
                else
                {
                    currentState = WaffleStates.Wander;
                }

                break;
        }

        // Always do these things, no matter the state:

        // Get the bounds force and scale it
        boundsForce = StayInBounds(boundsTime) * boundsScalar;

        // Add the bounds force to the ultimate force
        ultimaForce += boundsForce;

        // Flocking -- temp code for experimenting with different behaviors
        //ultimaForce += Seek(Manager.Instance.CenterPoint);
        //ultimaForce += Manager.Instance.SharedDirection * maxSpeed - physicsObject.Velocity;

        // Add the scaled separation force to the ultimate force
        ultimaForce += SeparateWaffles() * separateScalar;

    }

    /// <summary>
    /// Initializes the waffle -- called in parent's Awake() function.
    /// </summary>
    protected override void Init()
    {
        // Set a reference to the main camera
        physicsObject.MainCamera = Manager.Instance.MainCamera;

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
