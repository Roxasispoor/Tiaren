﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing damage
/// </summary>
public class Damage : EffectOnLiving
{
    private float damageValue;

    public Damage()
    {
        damageValue = 0;
    }
    public Damage(float value)
    {
        damageValue = value;
    }

    public Damage(Damage other) : base(other)
    {
        this.damageValue = other.damageValue;
    }


    /// <summary>
    /// create a damage object
    /// </summary>
    /// <param name="target">target of the action</param>
    /// <param name="launcher">launcher of the action</param>
    /// <param name="damageValue">quanitity of damage given</param>
    public Damage(LivingPlaceable target, Placeable launcher, float damageValue) : base(target, launcher)
    {
        this.DamageValue = damageValue;
    }


    public float DamageValue
    {
        get
        {
            return damageValue;
        }

        set
        {
            damageValue = value;
        }
    }

    public override Effect Clone()
    {
        return new Damage(this);
    }

    override
        public void Use()
    {

        Debug.Log("Touché!" + damageValue );
        Target.CurrentHP -= DamageValue;
        if (Target.CurrentHP <= 0)
        {

            Target.Destroy();
        }


        
    }
}
