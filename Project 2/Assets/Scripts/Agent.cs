using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class purpose: Abstract parent class for the different types of agents

public abstract class Agent : MonoBehaviour
{
    /* FIELDS AND PROPERTIES */

    // The physics object
    [SerializeField] protected PhysicsObject physicsObject;

    // Maximum speed and force
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float maxForce;

    // Vector for wander force
    protected Vector3 wanderForce;

    // Scalar value for wander
    [SerializeField] protected float wanderScalar = 1f;

    // The angle for wandering
    [SerializeField] protected float wanderAngle = 0f;

    // The maximum angle for wandering
    [SerializeField] protected float maxWanderAngle = 360f;

    // Vector for flee force
    protected Vector3 fleeForce;

    // Scalar value for fleeing
    [SerializeField] protected float fleeScalar = 1f;

    // Scalar for seek force
    [SerializeField] protected float seekScalar = 1f;

    // Vector for stay-in-bounds force
    protected Vector3 boundsForce;

    // Scalar for bounds force
    [SerializeField] protected float boundsScalar = 5f;

    // Scalar for separation force
    [SerializeField] protected float separateScalar = 1f;

    // Amount of time for bounds
    [SerializeField] protected float boundsTime = 1f;

    // The sum total of all the forces
    protected Vector3 ultimaForce;

    // Agent manager
    //[SerializeField] protected CollisionManager manager;

    // Range
    [SerializeField] float separateRange = 1f;

    public PhysicsObject PhysicsObject
    {
        get { return physicsObject; }
    }


    /* METHODS */

    // Update is called once per frame
    void Update()
    {
        // Start with the ultimate force at zero
        ultimaForce = Vector3.zero;

        // Calculate the steering forces (handled in the child classes)
        CalcSteeringForces();

        // Clamp the magnitude to the maximum force
        Vector3.ClampMagnitude(ultimaForce, maxForce);

        // Apply the ultimate force to the agent
        physicsObject.ApplyForce(ultimaForce);
    }

    private void Awake()
    {
        // Initialize the agent
        Init();
    }

    // Protected method to calculate the steering forces
    protected abstract void CalcSteeringForces();

    // Protected method to initialize agents
    protected abstract void Init();

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
    protected Vector3 Seek(PhysicsObject target)
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
    protected Vector3 Flee(PhysicsObject target)
    {
        // Call the other version of Seek
        //  which returns the seeking steering force
        //  and then return that returned vector.
        return Flee(target.transform.position);
    }

    /// <summary>
    /// Predicts the future position of an object after a certain amount of time.
    /// </summary>
    /// <param name="time">The time, in seconds, in between the current and future positions.</param>
    /// <returns>The predicted future position.</returns>
    public Vector3 CalcFuturePosition(float time)
    {
        return physicsObject.Velocity * time + transform.position;
    }

    /// <summary>
    /// Determine the force to wander ahead a bit
    /// </summary>
    /// <param name="currentWanderAngle">A reference param to the current wander angle (in degrees). 
    /// Reference params allow us to change the value here and whatever 
    /// method called this will get that change!</param>
    /// <param name="wanderRange">How big an angle delta to allow (in degrees)</param>
    /// <param name="time">look ahead time when projecting the circle</param>
    /// <param name="radius">radius when projecting the circle</param>
    /// <returns>The steering force.</returns>
    protected Vector3 Wander(ref float currentWanderAngle, float wanderRange, float time, float radius)
    {
        // Choose a distance ahead
        Vector3 futurePosOffset = CalcFuturePosition(time);

        // Get a random angle by adding on a bit to what we used last time
        currentWanderAngle += Random.Range(-wanderRange, wanderRange);

        // Stay inside a given max range
        if (currentWanderAngle > maxWanderAngle)
        {
            currentWanderAngle = maxWanderAngle;
        }
        else if (currentWanderAngle < -maxWanderAngle)
        {
            currentWanderAngle = -maxWanderAngle;
        }

        // Where would that displacement vector end?
        Vector3 targetOffset = futurePosOffset;
        targetOffset.x += Mathf.Cos(currentWanderAngle * Mathf.Deg2Rad) * radius;
        targetOffset.y += Mathf.Sin(currentWanderAngle * Mathf.Deg2Rad) * radius;

        // Return a force -- seek the target position
        return Seek(targetOffset);
    }

    protected Vector3 StayInBounds(float time)
    {
        // Get the future position
        Vector3 futurePos = CalcFuturePosition(time);

        // If the agent is approaching the screen bounds
        if (futurePos.x > physicsObject.ScreenMax.x
            || futurePos.x < -physicsObject.ScreenMax.x
            || futurePos.y > physicsObject.ScreenMax.y
            || futurePos.y < -physicsObject.ScreenMax.y)
        {
            // Seek the center of the screen
            return Seek(Vector3.zero);
        }

        // Otherwise, return the zero vector -- the agent doesn't need to do anything
        else
        {
            return Vector3.zero;
        }
    }


    protected Vector3 Separate()
    {
        Vector3 separateForce = Vector3.zero;

        // Go through all agents -- should probably decrease range so I'm not checking all of them every time
        foreach (Agent a in CollisionManager.Instance.Waffles)
        {
            // Get the square of the distance between the two agents
            float dist = Mathf.Pow(a.transform.position.x - transform.position.x, 2) + Mathf.Pow(a.transform.position.y - transform.position.y, 2);

            // As long as the agents aren't on top of each other
            if (Mathf.Epsilon < dist)
            {
                // Flee from the agent proportional to its distance
                separateForce += Flee(a.physicsObject) * separateRange / dist;
            }
        }

        return separateForce;
    }

    /// <summary>
    /// Gets the waffle closest to the agent.
    /// </summary>
    /// <returns>The nearest waffle.</returns>
    protected Waffle FindClosestWaffle()
    {
        // Set the minimum distance to as high as possible
        float minDist = Mathf.Infinity;

        // Set the nearest game object to null
        Waffle nearest = null;

        // Loop through each waffle
        foreach (Waffle waffle in CollisionManager.Instance.Waffles)
        {
            // Calculate the distance between the agent and the waffle
            float dist = Mathf.Pow(waffle.transform.position.x - transform.position.x, 2) + Mathf.Pow(waffle.transform.position.y - transform.position.y, 2);

            // If that distance is less than the minimum distance
            if (dist < minDist)
            {
                // Set the min distance equal the distance between the two, and set the nearest waffle to the current waffle
                minDist = dist;
                nearest = waffle;
            }

        }

        // Return the nearest waffle
        return nearest;
    }

}
