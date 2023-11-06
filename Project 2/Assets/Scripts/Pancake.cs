using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pancake : Agent
{
    [SerializeField] private GameObject target;

    protected override void CalcSteeringForces()
    {
        if (target != null)
        {
            physicsObject.ApplyForce(Seek(target));
        }
        else
        {
            physicsObject.ApplyForce(Seek(Vector3.zero));
        }
    }

}
