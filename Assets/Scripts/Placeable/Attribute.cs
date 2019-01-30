using System;
using UnityEngine;

[Serializable]
public struct Attribute
{
    [SerializeField]
    private float baseValue;
    private float flatModif ;
    private float percentModif;

    public float Value
    {
        get
        {
            return (BaseValue + flatModif) * (1 + percentModif);
        }
    }

    public float BaseValue
    {
        get
        {
            return baseValue;
        }

        set
        {
            baseValue = value;
        }
    }

    public float FlatModif
    {
        get
        {
            return flatModif;
        }

        set
        {
            flatModif = value;
        }
    }

    public float PercentModif
    {
        get
        {
            return percentModif;
        }

        set
        {
            percentModif = value;
        }
    }
}
