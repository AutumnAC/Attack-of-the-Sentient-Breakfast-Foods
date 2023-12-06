using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class purpose: To store information related to obstacles

public class Obstacle : MonoBehaviour
{
    // Radius
    [SerializeField] private float radius;

    public float Radius
    {
        get { return radius; }
    }

    // Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
