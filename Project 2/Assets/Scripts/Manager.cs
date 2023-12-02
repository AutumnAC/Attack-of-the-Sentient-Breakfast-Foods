using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Prefabs for agent
    [SerializeField] private Waffle wafflePrefab;
    [SerializeField] private Pancake pancakePrefab;

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
                new Vector3(Random.Range(mainCamera.orthographicSize * mainCamera.aspect, -mainCamera.orthographicSize * mainCamera.aspect),
                    Random.Range(mainCamera.orthographicSize, -mainCamera.orthographicSize),
                    0),
                Quaternion.identity));
        }

        // Spawn in the pancakes
        for (int i = 0; i < pancakeNumber; i++)
        {
            // Instantiate the pancake with a random x and y location within the bounds of the screen, and add it to the list
            pancakes.Add(Instantiate(pancakePrefab,
                new Vector3(Random.Range(mainCamera.orthographicSize * mainCamera.aspect, -mainCamera.orthographicSize * mainCamera.aspect),
                    Random.Range(mainCamera.orthographicSize, -mainCamera.orthographicSize),
                    0),
                Quaternion.identity));
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Experimental code for flocking
        centerPoint = GetCenterPoint();
        sharedDirection = GetSharedDirection();

        // Check each physics object against each physics object
        for (int x = 0; x < waffles.Count; x++)
        {
            for (int y = 0; y < pancakes.Count; y++)
            {
                //Debug.Log("x, y: " + x + ", " + y);

                // Check if they're colliding
                if (waffles.Count > 0 && CollisionCheck(waffles[x].PhysicsObject, pancakes[y].PhysicsObject))
                {
                    Waffle waffle = waffles[x];
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
                    pancakes[y].PhysicsObject.IsColliding = true;
                }
            }
        }
    }

    /// <summary>
    /// Checks for collision using bounding circles.
    /// </summary>
    /// <param name="sA">The first sprite to be checked for collision.</param>
    /// <param name="sB">The second sprite to be checked for collision.</param>
    /// <returns></returns>
    private bool CollisionCheck(PhysicsObject a, PhysicsObject b)
    {
        // Check if the combined lengths of the radii are less than the distance between them
        return Mathf.Pow(a.Radius + b.Radius, 2) >
            Mathf.Pow(b.transform.position.x - a.transform.position.x, 2)
            + Mathf.Pow(b.transform.position.y - a.transform.position.y, 2);
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
