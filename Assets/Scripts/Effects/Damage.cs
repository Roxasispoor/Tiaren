using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing damage
/// </summary>
public class Damage : Effect
{
    private float damageValue;
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

    override
        public void Use()
    {
        if (this.Target is LivingPlaceable)
        {
            LivingPlaceable Target = (LivingPlaceable)(this.Target);
            Target.currentPV -= DamageValue;
            if (Target.currentPV <= 0)
            {

                Target.DestroyLivingPlaceable();
            }


        }
    }
}
