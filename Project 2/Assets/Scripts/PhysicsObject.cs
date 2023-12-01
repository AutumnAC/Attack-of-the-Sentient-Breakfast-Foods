using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class purpose: Handles physics for agents.

public class PhysicsObject : MonoBehaviour
{
    /* FIELDS */

    // Pposition, direction, velocity
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 velocity;

    // Sum of all forces in a frame
    [SerializeField] private Vector3 acceleration = Vector3.zero;

    // Mass of object
    [SerializeField] private float mass = 1;

    // Friction
    [SerializeField] private bool useFriction;
    [SerializeField] private float frictionCoeffecient;

    // Maximum speed
    [SerializeField] private float maxSpeed = 10;

    // Window bounds
    private Vector3 screenMax = Vector3.zero;

    // Camera
    [SerializeField] private Camera mainCamera;

    // For collisions
    [SerializeField] private float radius;
    private bool isColliding;


    /* PROPERTIES */

    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    public Vector3 Velocity
    {
        get { return velocity; }
    }

    public float MaxSpeed
    {
        get { return maxSpeed; }
    }

    public Vector3 ScreenMax
    {
        get { return screenMax; }
    }

    public float Radius
    {
        get { return radius; }
    }

    public bool IsColliding
    {
        get { return isColliding; }
        set { isColliding = value; }
    }

    public Camera MainCamera
    {
        get { return mainCamera; }
        set { mainCamera = value; }
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
        // Calculate friction, if the object uses friction
        if (useFriction)
        {
            ApplyFriction(frictionCoeffecient);
        }

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
    /// Applies friction to the velocity.
    /// </summary>
    /// <param name="coeff">The friction coeffecient.</param>
    private void ApplyFriction(float coeff)
    {
        // Make the friction the reverse of the velocity
        Vector3 friction = velocity * -1;

        // Normalize it
        friction.Normalize();

        // Multiply it by the friction coeffecient
        friction = friction * coeff;

        // Apply it
        ApplyForce(friction);
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
