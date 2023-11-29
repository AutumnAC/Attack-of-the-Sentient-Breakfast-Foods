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

    // Sprite renderer
    protected SpriteRenderer spriteRenderer;

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
    [SerializeField] protected float boundsScalar = 0f;

    // Scalar for separation force
    [SerializeField] protected float separateScalar = 1f;

    // Amount of time for bounds
    [SerializeField] protected float boundsTime = 1f;

    // Scalar for flow field force
    [SerializeField] protected float flowFieldScalar = 1f;

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

    /// <summary>
    /// Creates a force that makes the agent stay in bounds.
    /// </summary>
    /// <param name="time">The time ahead to look to see if the agent's current trajectory will take it out of bounds.</param>
    /// <returns>The steering force.</returns>
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

    /// <summary>
    /// Separates the agent from waffles.
    /// </summary>
    /// <returns>The steering force.</returns>
    protected Vector3 SeparateWaffles()
    {
        Vector3 separateForce = Vector3.zero;

        // Go through all waffles
        foreach (Agent a in Manager.Instance.Waffles)
        {
            // Add each separate force to the overall separate force
            separateForce += Separate(a);
        }

        return separateForce;
    }

    /// <summary>
    /// Separates the agent from pancakes. Should likely be the same method as above since the logic is identical,
    /// but I don't have the time at the moment to sort out how the type-checking logic should work.
    /// </summary>
    /// <returns></returns>
    protected Vector3 SeparatePancakes()
    {
        Vector3 separateForce = Vector3.zero;

        // Go through all pancakes
        foreach (Agent a in Manager.Instance.Pancakes)
        {
            // Add each separate force to the overall separate force
            separateForce += Separate(a);
        }

        return separateForce;
    }

    /// <summary>
    /// Helper method that calculates the separation force between two agents.
    /// </summary>
    /// <param name="a">The agent to separate from.</param>
    /// <returns></returns>
    private Vector3 Separate(Agent a)
    {
        Vector3 separateForce = Vector3.zero;

        // Get the square of the distance between the two agents
        float dist = CalcSquaredDistance(transform.position, a.transform.position);

        // As long as the agents aren't on top of each other
        if (Mathf.Epsilon < dist)
        {
            // Flee from the agent proportional to its distance
            separateForce = Flee(a.physicsObject) * separateRange / dist;
        }

        return separateForce;
    }

    /// <summary>
    /// Gets the waffle closest to the agent.
    /// </summary>
    /// <returns>The nearest waffle.</returns>
    protected Waffle FindClosestWaffle(int radius)
    {
        // Set the minimum distance to as high as possible
        float minDist = Mathf.Infinity;

        // Set the nearest game object to null
        Waffle nearest = null;

        // Loop through each waffle
        foreach (Waffle waffle in Manager.Instance.Waffles)
        {
            // Calculate the distance between the agent and the waffle
            float dist = CalcSquaredDistance(transform.position, waffle.transform.position);

            // If that distance is less than the minimum distance
            if (dist < minDist && dist < radius)
            {
                // Set the min distance equal the distance between the two, and set the nearest waffle to the current waffle
                minDist = dist;
                nearest = waffle;
            }

        }

        // Return the nearest waffle
        return nearest;
    }

    /// <summary>
    /// Gets the pancake closest to the agent.
    /// </summary>
    /// <returns>The nearest pancake.</returns>
    protected Pancake FindClosestPancake()
    {
        // Set the minimum distance to as high as possible
        float minDist = Mathf.Infinity;

        // Set the nearest game object to null
        Pancake nearest = null;

        // Loop through each waffle
        foreach (Pancake pancake in Manager.Instance.Pancakes)
        {
            // Calculate the distance between the agent and the pancake
            float dist = CalcSquaredDistance(transform.position, pancake.transform.position);

            // If that distance is less than the minimum distance
            if (dist < minDist)
            {
                // Set the min distance equal the distance between the two, and set the nearest pancake to the current pancake
                minDist = dist;
                nearest = pancake;
            }

        }

        // Return the nearest pancake
        return nearest;
    }

    /// <summary>
    /// Experimental method for flow field following. Currently unfinished.
    /// </summary>
    protected Vector3 FollowFlowField()
    {
        //Vector3[][] flowField = new Vector3[columns][];

        return FlowField.Instance.GetFlowFieldPosition(transform.position);
    }

    /// <summary>
    /// Helper method that calculates the distance squared between two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns></returns>
    private float CalcSquaredDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2);
    }

}
