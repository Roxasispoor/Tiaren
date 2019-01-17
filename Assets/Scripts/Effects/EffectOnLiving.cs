using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EffectOnLiving : Effect
{
    private Animation OnPlaceable;
    private LivingPlaceable target;
    public int netIdTarget=-1;
  

    public EffectOnLiving(EffectOnLiving other)
    {
        Target = other.target;
        Launcher = other.Launcher;
    }

    protected EffectOnLiving()
    {
    }

    protected EffectOnLiving(LivingPlaceable target, Placeable launcher)
    {
        Target = target;
        Launcher = launcher;
    }
    /// <summary>
    /// Also sets netId of launcher
    /// </summary>
    public virtual LivingPlaceable Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
            if (target != null)
            {
                netIdTarget = target.netId;
            }
        }
    }

  
    public override Placeable GetTarget()
    {
        return Target;
    }

    public override void TargetAndInvokeEffectManager(LivingPlaceable placeable)
    {
        Target = placeable;
        EffectManager.instance.UseEffect(this);
        
    }

    public override void TargetAndInvokeEffectManager(Placeable placeable)
    {
       
    }
}


