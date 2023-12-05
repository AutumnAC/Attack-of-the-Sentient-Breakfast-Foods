using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Class purpose: Manager for handing agent interactions

public class Manager : Singleton<Manager>
{
    /* FIELDS */

    // List of agents in the scene
    [SerializeField] private List<Waffle> waffles;
    [SerializeField] private List<Pancake> pancakes;

    // List of obstacles
    [SerializeField] private List<Obstacle> obstacles = new List<Obstacle>();

    // Number of each type of agent to spawn in the scene
    [SerializeField] private int waffleNumber;
    [SerializeField] private int pancakeNumber;

    // Prefabs for agents and obstacle
    [SerializeField] private Waffle wafflePrefab;
    [SerializeField] private Pancake pancakePrefab;
    [SerializeField] private Obstacle obstaclePrefab;

    // The camera
    [SerializeField] private Camera mainCamera;

    // Experimental code for flocking
    private Vector3 centerPoint = Vector3.zero;

    private Vector3 sharedDirection = Vector3.zero;

    /* PROPERTIES */

    public Vector3 CenterPoint
    {
        get { return centerPoint; }
    }

    public Vector3 SharedDirection
    {
        get { return sharedDirection; }
    }

    public List<Waffle> Waffles
    {
        get { return waffles; }
    }

    public List<Pancake> Pancakes
    {
        get { return pancakes; }
    }

    public List<Obstacle> Obstacles
    {
        get { return obstacles; }
    }

    public Camera MainCamera
    {
        get { return mainCamera; }
    }


    /* METHODS */

    // (Optional) Prevent non-singleton constructor use.
    protected Manager() { }


    private void Start()
    {
        // Spawn in the waffles
        for (int i = 0; i < waffleNumber; i++)
        {
            // Instantiate the waffle with a random x and y location within the bounds of the screen, and add it to the list
            waffles.Add(Instantiate(wafflePrefab,
                new Vector3(UnityEngine.Random.Range(mainCamera.orthographicSize * mainCamera.aspect, -mainCamera.orthographicSize * mainCamera.aspect),
                    UnityEngine.Random.Range(mainCamera.orthographicSize, -mainCamera.orthographicSize),
                    0),
                Quaternion.identity));
        }

        // Spawn in the pancakes
        for (int i = 0; i < pancakeNumber; i++)
        {
            // Instantiate the pancake with a random x and y location within the bounds of the screen, and add it to the list
            pancakes.Add(Instantiate(pancakePrefab,
                new Vector3(UnityEngine.Random.Range(mainCamera.orthographicSize * mainCamera.aspect, -mainCamera.orthographicSize * mainCamera.aspect),
                    UnityEngine.Random.Range(mainCamera.orthographicSize, -mainCamera.orthographicSize),
                    0),
                Quaternion.identity));
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Get the center point and average direction for flocking
        centerPoint = GetCenterPoint();
        sharedDirection = GetSharedDirection();

        // Check each waffle
        for (int x = 0; x < waffles.Count; x++)
        {
            Waffle waffle = waffles[x];

            // Reset the obstacle collision flags every frame
            waffle.PhysicsObject.IsCollidingWithObstacle = false;


            // Check each waffle against each pancake
            for (int y = 0; y < pancakes.Count; y++)
            {
                //Debug.Log("x, y: " + x + ", " + y);

                // Check if they're colliding
                if (waffles.Count > 0 && CollisionCheck(waffles[x].PhysicsObject, pancakes[y].PhysicsObject))
                {
                    //Debug.Log("x, y: " + x + ", " + y + " are colliding");

                    // If they are, let the waffle know it's collided and remove the waffle from the list
                    //waffles[x].PhysicsObject.IsColliding = true;
                    waffles.RemoveAt(x);

                    // Destroy the waffle
                    Destroy(waffle.gameObject);

                    // Step backwards in the list
                    if (x > 0)
                    {
                        x--;
                    }

                    // Set the pancake's collision flag to true -- currently does nothing
                    // pancakes[y].PhysicsObject.IsColliding = true;
                }
            }

            // Check each waffle against each obstacle
            for (int y = 0; y < obstacles.Count; y++)
            {
                // If there are waffles and obstacles in the scene, and one of the obstacles and waffles is colliding
                if (waffles.Count > 0
                    && obstacles.Count > 0
                    && CollisionCheck(waffle.PhysicsObject.Radius, obstacles[y].Radius,
                        waffle.transform.position, obstacles[y].transform.position))
                {
                    // Turn on the waffle's collision flag
                    waffle.PhysicsObject.IsCollidingWithObstacle = true;
                }
            }
        }

        // Check each pancake against each obstacle
        for (int x = 0; x < pancakes.Count; x++)
        {
            // Get a reference to the current pancake
            Pancake pancake = pancakes[x];

            for (int y = 0; y < obstacles.Count; y++)
            {
                // If there are obstacles in the scene, and one of the obstacles and pancakes is colliding
                if (obstacles.Count > 0
                    && CollisionCheck(pancake.PhysicsObject.Radius, obstacles[y].Radius,
                        pancake.transform.position, obstacles[y].transform.position))
                {
                    // Turn on the pancake's collision flag
                    pancake.PhysicsObject.IsCollidingWithObstacle = true;
                }
            }

        }
    }

    /// <summary>
    /// Adds a pool of syrup wherever the player clicks.
    /// </summary>
    /// <param name="context"></param>
    public void OnFire(InputAction.CallbackContext context)
    {
        // If the player has just clicked
        if (context.started)
        {
            // Get the mouse position
            Vector3 mousePosition = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0);
            mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
            mousePosition.z = 0;

            // Instantiate the syrup
            Obstacle newObstacle = Instantiate(
                obstaclePrefab,
                mousePosition,
                Quaternion.identity);

            // Add it to the list of obstacles
            obstacles.Add(newObstacle);
        }
    }

    /// <summary>
    /// Checks for collision using the radii of bounding circles.
    /// </summary>
    /// <param name="a">The radius of the first object to be checked for collision.</param>
    /// <param name="b">The radius of the second object to be checked for collision.</param>
    /// <returns></returns>
    private bool CollisionCheck(PhysicsObject a, PhysicsObject b)
    {
        // Call the other CollisionCheck
        return CollisionCheck(a.Radius, b.Radius, a.transform.position, b.transform.position);
    }

    /// <summary>
    /// Overload of CollisionCheck for when checking an object that isn't a PhysicsObject.
    /// </summary>
    /// <param name="aRadius">The radius of the first object.</param>
    /// <param name="bRadius">The radius of the second object.</param>
    /// <param name="aPosition"></param>
    /// <param name="bPosition"></param>
    /// <returns></returns>
    private bool CollisionCheck(float aRadius, float bRadius, Vector3 aPosition, Vector3 bPosition)
    {
        // Check if the combined lengths of the radii are less than the distance between them
        return Mathf.Pow(aRadius + bRadius, 2) >
            Mathf.Pow(bPosition.x - aPosition.x, 2)
            + Mathf.Pow(bPosition.y - aPosition.y, 2);

    }


    /// <summary>
    /// Gets the center point of all the pancakes.
    /// </summary>
    /// <returns>A steering vector pointng towards the center point.</returns>
    private Vector3 GetCenterPoint()
    {
        Vector3 totalVector = new Vector3();

        // Loop through all the pancakes
        foreach (Pancake pancake in pancakes)
        {
            // Add each pancake's position to the total vector
            totalVector += pancake.transform.position;
        }

        // Return the averaged position
        return totalVector / pancakes.Count;
    }

    /// <summary>
    /// Gets the average direction of all the pancakes.
    /// </summary>
    /// <returns>The average direction of all the pancakes.</returns>
    private Vector3 GetSharedDirection()
    {
        Vector3 totalVector = Vector3.zero;

        // Loop through all the pancakes
        foreach (Pancake pancake in pancakes)
        {
            // Add each pancake's direction to the total vector
            totalVector += pancake.PhysicsObject.Velocity.normalized;
        }

        // Return the averaged direction
        return totalVector.normalized;
    }

}
