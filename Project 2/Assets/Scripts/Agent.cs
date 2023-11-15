using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    protected float wanderScalar = 1f;

    // The angle for wandering
    protected float wanderAngle = 0f;

    // The maximum angle for wandering
    protected float maxWanderAngle = 360f;

    // Vector for stay-in-bounds force
    protected Vector3 boundsForce;

    // Scalar for bounds force
    [Min(1f)]
    protected float boundsScalar = 5f;

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

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ultimaForce = Vector3.zero;

        CalcSteeringForces();

        Vector3.ClampMagnitude(ultimaForce, maxForce);
        physicsObject.ApplyForce(ultimaForce);
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
            return Seek(Vector3.zero);
        }

        // Otherwise, return the zero vector -- the agent doesn't need to turn around
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
}
