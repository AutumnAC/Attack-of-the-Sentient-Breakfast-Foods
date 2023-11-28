using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Enum for the pancake's state
public enum PancakeStates
{
    Patrol,
    Seek,
}


// Class purpose: Defines behavior for the Pancake agent

public class Pancake : Agent
{
    // The target to be used in Seek
    [SerializeField] private GameObject target;

    // State enum
    private PancakeStates currentState;

    /// <summary>
    /// Calculates the steering forces that affect the pancake.
    /// </summary>
    protected override void CalcSteeringForces()
    {
        switch (currentState)
        {
            case PancakeStates.Patrol:

                // Temporary code to test the seeking more effectively
                //currentState = PancakeStates.Seek;

                // Follow the flow field
                ultimaForce += FollowFlowField() * flowFieldScalar;

                // No transition to Seek yet -- will check for any waffles within a certain radius

                break;

            case PancakeStates.Seek:

                target = FindClosestWaffle().gameObject;

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
                }

                break;
        }

        // Always do these things, no matter the state:

        // Get the bounds force and scale it
        boundsForce = StayInBounds(boundsTime) * boundsScalar;

        // Add the bounds force to the ultimate force
        ultimaForce += boundsForce;

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
    }

}
