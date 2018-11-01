using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing damage
/// </summary>
public class Damage : Effect
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
    public Damage(Placeable target, Placeable launcher, float damageValue) : base(target, launcher)
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
        if (this.Target is LivingPlaceable)
        {
            LivingPlaceable Target = (LivingPlaceable)(this.Target);
            Target.CurrentHP -= DamageValue;
            if (Target.CurrentHP <= 0)
            {

                Target.DestroyLivingPlaceable();
            }


        }
    }
}
