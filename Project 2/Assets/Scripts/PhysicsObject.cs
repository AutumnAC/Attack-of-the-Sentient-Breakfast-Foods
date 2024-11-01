using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class purpose: Handles physics for agents.

public class PhysicsObject : MonoBehaviour
{
    /* FIELDS */

    // Position, direction, velocity
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 velocity;

    // Sum of all forces in a frame
    [SerializeField] private Vector3 acceleration = Vector3.zero;

    // Mass of object
    [SerializeField] private float mass;

    // Maximum speed
    private float maxSpeed;
    [SerializeField] private float defaultMaxSpeed;

    // Window bounds
    private Vector3 screenMax = Vector3.zero;

    // Camera
    private Camera mainCamera;

    // Radius
    [SerializeField] private float radius;

    // Collision flags
    private bool isCollidingWithAgent;
    private bool isCollidingWithObstacle;


    /* PROPERTIES */

    // All properties are for the field of the same name

    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    public Vector3 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    public float MaxSpeed
    {
        get { return maxSpeed; }
        set { maxSpeed = value; }
    }

    public float DefaultMaxSpeed
    {
        get { return defaultMaxSpeed; }

        // Get-only property
    }

    public Vector3 ScreenMax
    {
        get { return screenMax; }

        // Get-only property
    }

    public Camera MainCamera
    {
        get { return mainCamera; }
        set { mainCamera = value; }
    }

    public float Radius
    {
        get { return radius; }

        // Get-only property
    }

    public bool IsCollidingWithAgent
    {
        get { return isCollidingWithAgent; }
        set { isCollidingWithAgent = value; }
    }

    public bool IsCollidingWithObstacle
    {
        get { return isCollidingWithObstacle; }
        set { isCollidingWithObstacle = value; }
    }


    /* METHODS */

    // Start is called before the first frame update
    void Start()
    {
        // Set the position variable equal to the transform position
        position = transform.position;

        // Set the screen max to the camera size
        screenMax.x = mainCamera.orthographicSize * mainCamera.aspect;
        screenMax.y = mainCamera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the velocity for this frame
        velocity += acceleration * Time.deltaTime;

        // Clamp the velocity to a maximum speed
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Set the position based on the velocity
        position += velocity * Time.deltaTime;

        // Grab current direction from velocity
        direction = velocity.normalized;

        // Set the transform position to the position variable
        transform.position = position;

        // Zero out acceleration
        acceleration = Vector3.zero;

        // Rotate the object towards the direction it's facing
        transform.rotation = Quaternion.LookRotation(Vector3.back, direction);
    }

    /// <summary>
    /// Applies a force to the acceleration.
    /// </summary>
    /// <param name="force">The force.</param>
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    /// <summary>
    /// Draws a circle gizmo around the bounding boxes of the monsters.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw a sphere based on the bounding circle collision
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
