using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    // List of objects affected by physics in the scene
    [SerializeField] private List<PhysicsObject> physicsObjects;

    // Update is called once per frame
    void Update()
    {
        // Check each physics object against each physics object
        for (int x = 0; x < physicsObjects.Count; x++)
        {
            for (int y = 0; y < physicsObjects.Count; y++)
            {
                // As long as the two objects being checked are not the same
                if (x != y)
                {
                    // Check if they're colliding
                    if (CollisionCheck(physicsObjects[x], physicsObjects[y]))
                    {
                        // If they are, set their collision flags to true
                        physicsObjects[x].IsColliding = true;
                        physicsObjects[y].IsColliding = true;
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
