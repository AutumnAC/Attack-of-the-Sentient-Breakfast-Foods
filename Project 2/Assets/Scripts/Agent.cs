using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class purpose: Abstract parent class for the different types of agents

public abstract class Agent : MonoBehaviour
{
    /* FIELDS AND PROPERTIES */

    // The physics object
    [SerializeField] protected PhysicsObject physicsObject;

    // Maximum force
    [SerializeField] protected float defaultMaxForce;
    protected float maxForce;

    // Sprite renderer
    protected SpriteRenderer spriteRenderer;

    // The radius for seeing other agents for seeking or fleeing
    [SerializeField] protected float visionRadius;

    // The sum total of all the forces, to be applied to the agent
    protected Vector3 ultimaForce;


    // STAYING IN BOUNDS

    // Vector for bounds force -- used for debugging purposes
    protected Vector3 boundsForce;

    // The scalar to increase the bounds force by
    [SerializeField] protected float boundsScalar;

    // The time to look ahead for bounds
    [SerializeField] protected float boundsTime;


    // SEPARATION

    // The scalar to increase the separation by
    [SerializeField] protected float separateScalar;

    // The range for separation
    [SerializeField] float separateRange;


    // OBSTACLE AVOIDANCE

    // List of found obstacles -- for debugging purposes only
    protected List<Vector3> foundObstacles = new List<Vector3>();

    // The scalar to increase the obstacle avoidance force by
    [SerializeField] protected float obstaclesScalar;
    
    // The time to look ahead for avoiding obstacles
    [SerializeField] protected float obstacleAvoidTime;


    // WANDER

    // Vector for wander force -- used for debugging purposes
    protected Vector3 wanderForce;

    // The scalar to increase the wander force by
    [SerializeField] protected float wanderScalar;

    // The angle for wandering
    [SerializeField] protected float wanderAngle;

    // The maximum angle for wandering
    [SerializeField] protected float maxWanderAngle;


    // FLEE

    // Vector for flee force -- used for debugging purposes
    protected Vector3 fleeForce;

    // The scalar to increase the flee force by
    [SerializeField] protected float fleeScalar;


    // SEEK

    // The scalar to increase the seek force by
    [SerializeField] protected float seekScalar;


    // Property for the physics object
    public PhysicsObject PhysicsObject
    {
        get { return physicsObject; }

        // Get-only property
    }


    /* METHODS */

    private void Awake()
    {
        // Set up the main camera
        physicsObject.MainCamera = Manager.Instance.MainCamera;

        // Initialize a random angle for the wander angle
        wanderAngle = UnityEngine.Random.Range(-maxWanderAngle, maxWanderAngle);

        // Set up the sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize maxForce and maxSpeed
        maxForce = defaultMaxForce;
        physicsObject.MaxSpeed = physicsObject.DefaultMaxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // Start with the ultimate force at zero
        ultimaForce = Vector3.zero;

        // If the agent's obstacle collision was turned on and the agent wasn't already slowed, slow the agent down
        if (physicsObject.IsCollidingWithObstacle && physicsObject.MaxSpeed == physicsObject.DefaultMaxSpeed)
        {
            // Slow the agent down
            physicsObject.MaxSpeed = physicsObject.MaxSpeed / 10;

            // Decrease max force to decrease jitter
            maxForce = maxForce / 5;
        }

        // Otherwise
        else
        {
            // Set the max speed and max force back to the default
            physicsObject.MaxSpeed = physicsObject.DefaultMaxSpeed;
            maxForce = defaultMaxForce;

            // Give the agent a boost in the direction of their current velocity so that the agent can speed up again
            physicsObject.ApplyForce(physicsObject.Velocity);
        }

        // Calculate the steering forces (handled in the child classes)
        CalcSteeringForces();

        // Clamp the magnitude to the maximum force
        Vector3.ClampMagnitude(ultimaForce, maxForce);

        // Apply the ultimate force to the agent
        physicsObject.ApplyForce(ultimaForce);
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
        desiredVelocity = desiredVelocity.normalized * physicsObject.MaxSpeed;

        // Calculate seek steering force
        Vector3 seekingForce = desiredVelocity - physicsObject.Velocity;

        // Return seek steering force
        return seekingForce;
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
        desiredVelocity = desiredVelocity.normalized * physicsObject.MaxSpeed;

        // Calculate seek steering force
        Vector3 seekingForce = desiredVelocity - physicsObject.Velocity;

        // Return seek steering force
        return seekingForce;
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
        currentWanderAngle += UnityEngine.Random.Range(-wanderRange, wanderRange);

        // Stay inside a given max range
        if (currentWanderAngle > maxWanderAngle)
        {
            currentWanderAngle = maxWanderAngle;
        }
        else if (currentWanderAngle < -maxWanderAngle)
        {
            currentWanderAngle = -maxWanderAngle;
        }

        // Set the target offset to the future position initially
        Vector3 targetOffset = futurePosOffset;

        // Adjust the target position by a few degrees
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
    /// Separates the agent from pancakes.
    /// </summary>
    /// <returns>The steering force.</returns>
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
    /// <returns>The separation force between the two agents.</returns>
    private Vector3 Separate(Agent a)
    {
        Vector3 separateForce = Vector3.zero;

        // Get the square of the distance between the two agents
        float dist = CalcSquaredDistance(transform.position, a.transform.position);

        // As long as the agents aren't on top of each other
        if (Mathf.Epsilon < dist)
        {
            // Flee from the agent proportional to its distance
            separateForce = Flee(a.transform.position) * separateRange / dist;
        }

        return separateForce;
    }

    /// <summary>
    /// Gets the pancakes in range and flees from them, weighted proportional to distance.
    /// </summary>
    /// <param name="radius">The range being fled from.</param>
    /// <returns>A steering force fleeing from the pancakes.</returns>
    protected Vector3 FleeFromClosePancakes(float radius)
    {
        Vector3 fleeForce = FleeFromClosePancakes(radius, FindPancakesInRange(radius));

        return fleeForce;
    }

    protected Vector3 FleeFromClosePancakes(float radius, List<Pancake> pancakesInRange)
    {
        // ideas: make the pancakes in range a field

        Vector3 fleeForce = Vector3.zero;

        // Loop through all the pancakes in the list
        foreach (Pancake pancake in pancakesInRange)
        {
            // Get the square of the distance between the two agents
            float dist = CalcSquaredDistance(transform.position, pancake.transform.position);

            // As long as the distance is less than the given radius
            if (dist < Math.Pow(radius, 2))
            {
                // Flee from the agent proportional to its distance
                fleeForce += Flee(pancake.transform.position) * radius / dist;
            }
        }

        return fleeForce;
    }

    /// <summary>
    /// Steers the agent away from obstacles.
    /// </summary>
    /// <returns>The steering force away from the obstacles.</returns>
    protected Vector3 AvoidObstacles()
    {
        // Get the total force to avoid
        Vector3 totalAvoidForce = Vector3.zero;

        // Clear the list of found obstacles
        foundObstacles.Clear();

        // Loop through all the obstacles
        foreach (Obstacle obstacle in Manager.Instance.Obstacles)
        {
            // Get the vector from the obstacle to the agent
            Vector3 agentToObstacle = obstacle.transform.position - transform.position;

            // Declare the right dot product to be used later
            float rightDot = 0;

            // Get the forward dot product
            float forwardDot = Vector3.Dot(physicsObject.Velocity.normalized, agentToObstacle);

            // If the obstacle is in front of the agent
            if (forwardDot >= -obstacle.Radius)
            {
                // Get the future position
                Vector3 futurePos = CalcFuturePosition(obstacleAvoidTime);

                // Get the distance between the agent and its future position
                float dist = Vector3.Distance(transform.position, futurePos) + physicsObject.Radius;

                // Get a steering force
                Vector3 steeringForce = transform.right * (1 - forwardDot / dist) * physicsObject.MaxSpeed /* commenting out the maxSpeed made everything worse somehow */;

                // If the object is within the distance between the agent and its future position
                if (forwardDot <= dist + obstacle.Radius)
                {
                    // Get the right dot product by projecting onto the right axis
                    rightDot = Vector3.Dot(transform.right, agentToObstacle);

                    // If the obstacle is within the safe box width
                    if (Mathf.Abs(rightDot) <= physicsObject.Radius + obstacle.Radius)
                    {
                        // If left, steer right
                        if (rightDot < 0)
                        {
                            totalAvoidForce += steeringForce;
                        }

                        // If right, steer left
                        else
                        {
                            totalAvoidForce -= steeringForce;
                        }

                        // Add the obstacle to the list of found obstacles
                        foundObstacles.Add(obstacle.transform.position);
                    }

                }
                // Otherwise, it's too far away

            }
            // Otherwise, it's behind the agent

        }

        return totalAvoidForce;
    }

    /// <summary>
    /// Gets the waffle closest to the agent.
    /// </summary>
    /// <returns>The nearest waffle.</returns>
    protected Waffle FindClosestWaffle(float radius)
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
            if (dist < minDist && dist < Math.Pow(radius, 2))
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
    /// Gets all the pancakes within a certain radius of the agent.
    /// </summary>
    /// <returns>A list of nearby pancakes.</returns>
    protected List<Pancake> FindPancakesInRange(float radius)
    {
        // Create the list of the pancakes in range
        List<Pancake> pancakesInRange = new List<Pancake>();

        // Loop through each pancake
        foreach (Pancake pancake in Manager.Instance.Pancakes)
        {
            // Calculate the squared distance between the agent and the pancake
            float dist = CalcSquaredDistance(transform.position, pancake.transform.position);

            // If that distance is less than the radius squared
            if (dist < Math.Pow(radius, 2))
            {
                // Add it to the list of pancakes in range
                pancakesInRange.Add(pancake);
            }

        }

        // Return the list
        return pancakesInRange;
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
    /// Helper method that calculates the distance squared between two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns></returns>
    private float CalcSquaredDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2);
    }

    /// <summary>
    /// Draws gizmos for visualizing useful info.
    /// </summary>
    private void OnDrawGizmos()
    {
        //
        //  Draw safe space box
        //
        Vector3 futurePos = CalcFuturePosition(obstacleAvoidTime);

        float dist = Vector3.Distance(transform.position, futurePos) + physicsObject.Radius;

        Vector3 boxSize = new Vector3(physicsObject.Radius * 2f,
            dist,
            physicsObject.Radius * 2f);

        Vector3 boxCenter = Vector3.zero;
        boxCenter.y += dist / 2f;

        Gizmos.color = Color.green;

        // Change perspective of stuff being drawn to match the position and rotation of transform
        Gizmos.matrix = transform.localToWorldMatrix; // we're telling the gizmo to draw itself shifted to define the world based on the transform. and the transform says to scale it by 1.5!
        Gizmos.DrawWireCube(boxCenter, boxSize);
        Gizmos.matrix = Matrix4x4.identity;


        //
        //  Draw lines to found obstacles
        //
        Gizmos.color = Color.yellow;

        foreach (Vector3 pos in foundObstacles)
        {
            Gizmos.DrawLine(transform.position, pos);
        }
    }    
}
