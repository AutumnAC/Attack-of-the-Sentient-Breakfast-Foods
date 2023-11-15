using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waffle : Agent
{
    [SerializeField] private GameObject target;

    protected override void CalcSteeringForces()
    {
        // Get a wander force
        wanderForce = Wander(ref wanderAngle, 20f, .2f, .5f);

        // Multiply it by its weight
        wanderForce *= wanderScalar;

        // Add it to the wander force
        ultimaForce += wanderForce;

        // Do the same thing for the bounds force
        boundsForce = StayInBounds(boundsTime);
        boundsForce *= boundsScalar;
        ultimaForce += boundsForce;

        ultimaForce += Separate();
    }

    protected override void Init()
    {
        physicsObject.MainCamera = CollisionManager.Instance.MainCamera;

        // Initialize a random angle for the wander angle
        wanderAngle = UnityEngine.Random.Range(-maxWanderAngle, maxWanderAngle);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + wanderForce);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + boundsForce);
    }
}
