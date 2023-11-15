using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : Singleton<CollisionManager>
{
    // List of agents in the scene
    [SerializeField] private List<Waffle> waffles;
    [SerializeField] private List<Pancake> pancakes;

    // Number of each type of agent to spawn in the scene
    [SerializeField] private int waffleNumber = 10;
    [SerializeField] private int pancakeNumber = 5;

    // Prefabs for agent
    [SerializeField] private Waffle wafflePrefab;
    [SerializeField] private Pancake pancakePrefab;

    // The camera
    [SerializeField] private Camera mainCamera;

    // (Optional) Prevent non-singleton constructor use.
    protected CollisionManager() { }

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

    private void Start()
    {
        // Spawn in the waffles
        for (int i = 0; i < waffleNumber; i++)
        {
            waffles.Add(Instantiate(wafflePrefab, Vector3.zero, Quaternion.identity));
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
                        // If they are, set their collision flags to true
                        waffles[x].PhysicsObject.IsColliding = true;
                        waffles[y].PhysicsObject.IsColliding = true;
                    }
                }
            }
        }
    }

    // Spawn methods
    // Look at spawn manager from project 1

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
