using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Représente un point visable et ses multiplicateur de dégats, ainsi que si il est atteignable.
/// </summary>
public class HitablePoint  {
    private Vector3 relativePosition;
    private float damageMultiplier;
    private bool shootable;

    public HitablePoint(Vector3 relativePosition,float damageMultiplier)
    {
        this.relativePosition = relativePosition;
        this.damageMultiplier = damageMultiplier;
    }

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

    public bool Shootable
    {
        get
        {
            return shootable;
        }

        set
        {
            shootable = value;
        }
    }
}
