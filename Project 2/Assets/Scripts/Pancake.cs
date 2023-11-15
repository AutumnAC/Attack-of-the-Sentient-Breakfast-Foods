using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pancake : Agent
{
    [SerializeField] private PhysicsObject target;

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

    protected override void Init()
    {
        physicsObject.MainCamera = CollisionManager.Instance.MainCamera;
    }

}
