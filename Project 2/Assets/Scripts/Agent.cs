using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    // The physics object
    [SerializeField] protected PhysicsObject physicsObject;

    // Maximum speed and force
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float maxForce;


    // Update is called once per frame
    void Update()
    {
        CalcSteeringForces();
    }

    // Protected method to calculate the steering forces
    protected abstract void CalcSteeringForces();

    /// <summary>
    /// Makes the agent seek a target.
    /// </summary>
    /// <param name="targetPos">The position of the target to seek.</param>
    /// <returns>The steering force.</returns>
    protected Vector3 Seek(Vector3 targetPos)
    {
        // Calculate desired velocity
        Vector3 desiredVelocity = targetPos - gameObject.transform.position;

        // Set desired = max speed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate seek steering force
        Vector3 seekingForce = desiredVelocity - physicsObject.Velocity;

        // Return seek steering force
        return seekingForce;
    }

    /// <summary>
    /// Makes the agent seek a target.
    /// </summary>
    /// <param name="target">The target to seek.</param>
    /// <returns>The steering force.</returns>
    protected Vector3 Seek(GameObject target)
    {
        // Call the other version of Seek
        //  which returns the seeking steering force
        //  and then return that returned vector.
        return Seek(target.transform.position);
    }

    /// <summary>
    /// Makes the agent flee from a target.
    /// </summary>
    /// <param name="targetPos">The position of the target to flee from.</param>
    /// <returns>The steering force.</returns>
    protected Vector3 Flee(Vector3 targetPos)
    {
        // Calculate desired velocity
        Vector3 desiredVelocity = gameObject.transform.position - targetPos;

        // Set desired = max speed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate seek steering force
        Vector3 seekingForce = desiredVelocity - physicsObject.Velocity;

        // Return seek steering force
        return seekingForce;
    }

    /// <summary>
    /// Makes the agent flee from a target.
    /// </summary>
    /// <param name="target">The target to flee from.</param>
    /// <returns>The steering force.</returns>
    protected Vector3 Flee(GameObject target)
    {
        // Call the other version of Seek
        //  which returns the seeking steering force
        //  and then return that returned vector.
        return Flee(target.transform.position);
    }
}
