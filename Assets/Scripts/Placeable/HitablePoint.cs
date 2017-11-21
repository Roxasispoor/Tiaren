using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitablePoint  {
    private Vector3 relativePosition;
    private float damageMultiplier;

    public Vector3 RelativePosition
    {
        get
        {
            return relativePosition;
        }

        set
        {
            relativePosition = value;
        }
    }

    public float DamageMultiplier
    {
        get
        {
            return damageMultiplier;
        }

        set
        {
            damageMultiplier = value;
        }
    }

   
}
