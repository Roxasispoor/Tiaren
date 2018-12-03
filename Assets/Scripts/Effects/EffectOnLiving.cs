using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EffectOnLiving : Effect
{
    private Animation OnPlaceable;
    private LivingPlaceable target;
  

    public EffectOnLiving(EffectOnLiving other)
    {
        this.target = other.target;
        this.launcher = other.launcher;
    }

    protected EffectOnLiving()
    {
    }

    protected EffectOnLiving(LivingPlaceable target, Placeable launcher)
    {
        this.target = target;
        this.launcher = launcher;
    }
    
    public virtual LivingPlaceable Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }

  
    public override Placeable GetTarget()
    {
        return Target;
    }

    public override void TargetAndInvokeEffectManager(LivingPlaceable placeable)
    {
        this.target = placeable;
        EffectManager.instance.UseEffect(this);
        
    }

    public override void TargetAndInvokeEffectManager(Placeable placeable)
    {
       
    }
}


