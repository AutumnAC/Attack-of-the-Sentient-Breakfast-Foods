using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
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
    [SerializeField] Camera mainCamera;

    // For collisions
    [SerializeField] private float radius;
    private bool isColliding;

    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    public Vector3 Velocity
    {
        get { return velocity; }
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


    // Start is called before the first frame update
    void Start()
    {
        // Set the position variable equal to the transform position
        position = transform.position;

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

        // Calculate the velocity for this frame - New
        velocity += acceleration * Time.deltaTime;

        // Bounce the object if it hits the edges of the screen
        Bounce();

        // Clamp the velocity to a maximum speed
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Set the position based on the velocity
        position += velocity * Time.deltaTime;

        // Grab current direction from velocity  - New
        direction = velocity.normalized;

        // Set the transform position to the position variable
        transform.position = position;

        // Zero out acceleration - New
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
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction = friction * coeff;
        ApplyForce(friction);
    }

    /// <summary>
    /// Bounces the object when it hits the edge of the screen.
    /// </summary>
    private void Bounce()
    {
        // If the object goes beyond the edge of the screen, move it to within the edge of the screen and reverse its velocity

        if (position.x > screenMax.x)
        {
            velocity.x *= -1;
            position.x = screenMax.x;
        }

        else if (position.x < -screenMax.x)
        {
            velocity.x *= -1;
            position.x = -screenMax.x;
        }


        if (position.y > screenMax.y)
        {
            velocity.y *= -1;
            position.y = screenMax.y;
        }

        else if (position.y < -screenMax.y)
        {
            velocity.y *= -1;
            position.y = -screenMax.y;
        }
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
