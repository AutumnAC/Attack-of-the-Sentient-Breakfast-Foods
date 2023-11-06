using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waffle : Agent
{
    [SerializeField] private GameObject target;

    protected override void CalcSteeringForces()
    {
        if (physicsObject.IsColliding)
        {
            Destroy(this.gameObject);
        }

        physicsObject.ApplyForce(Flee(target));
    }

}
