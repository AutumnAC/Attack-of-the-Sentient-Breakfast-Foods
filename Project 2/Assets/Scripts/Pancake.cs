using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Class purpose: Defines behavior for the Pancake agent

public class Pancake : Agent
{
    // The target to be used in Seek
    [SerializeField] private PhysicsObject target;

    /// <summary>
    /// Calculates the steering forces that affect the pancake.
    /// </summary>
    protected override void CalcSteeringForces()
    {
        // As long as there is a target
        if (target != null)
        {
            // Seek it
            ultimaForce += Seek(target) * seekScalar;
        }

        // Otherwise, seek the center of the screen
        else
        {
            ultimaForce += Seek(Vector3.zero) * seekScalar;
            Debug.Log(ultimaForce);
            Debug.Log(Seek(Vector3.zero));
        }

        // Get the bounds force and scale it
        boundsForce = StayInBounds(boundsTime) * boundsScalar;

        // Add the bounds force to the ultimate force
        ultimaForce += boundsForce;

        // Add the scaled separation force to the ultimate force
        ultimaForce += Separate() * separateScalar;

    }

    /// <summary>
    /// Initializes the pancake -- called in parent's Awake() function.
    /// </summary>
    protected override void Init()
    {
        // Set up the main camera
        physicsObject.MainCamera = CollisionManager.Instance.MainCamera;
    }

}
