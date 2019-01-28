﻿public struct Attribute
{
    private float baseValue;
    private float flatModif ;
    private float percentModif;

    public float Value
    {
        get
        {
            if (BaseValue == 0 && flatModif == 0 && percentModif == 0)
            {
                percentModif = 1;
            }
            return (BaseValue + flatModif) * percentModif;
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
            if (BaseValue == 0 && flatModif == 0 && percentModif == 0)
            {
                percentModif = 1;
            }
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
