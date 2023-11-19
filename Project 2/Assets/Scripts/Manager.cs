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

    // Number of each type of agent to spawn in the scene
    [SerializeField] private int waffleNumber;
    [SerializeField] private int pancakeNumber;

    // Prefabs for agent
    [SerializeField] private Waffle wafflePrefab;
    [SerializeField] private Pancake pancakePrefab;

    // The camera
    [SerializeField] private Camera mainCamera;

    // (Optional) Prevent non-singleton constructor use.
    protected Manager() { }


    /* PROPERTIES */

    public List<Waffle> Waffles
    {
        get { return waffles; }
    }

    public List<Pancake> Pancakes
    {
        get { return pancakes; }
    }

    public Camera MainCamera
    {
        get { return mainCamera; }
    }


    /* METHODS */

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
        // Check each physics object against each physics object
        for (int x = 0; x < waffles.Count; x++)
        {
            for (int y = 0; y < waffles.Count; y++)
            {
                // As long as the two objects being checked are not the same
                if (x != y)
                {
                    // Check if they're colliding
                    if (CollisionCheck(waffles[x].PhysicsObject, waffles[y].PhysicsObject))
                    {
                        // If they are, set their collision flags to true -- currently does nothing
                        waffles[x].PhysicsObject.IsColliding = true;
                        waffles[y].PhysicsObject.IsColliding = true;
                    }
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
}
