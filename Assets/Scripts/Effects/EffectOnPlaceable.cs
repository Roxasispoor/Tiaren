using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EffectOnPlaceable : Effect {

    private Placeable target;
    private Placeable launcher;

    public EffectOnPlaceable(EffectOnPlaceable other)
    {
        this.target = other.target;
        this.launcher = other.launcher;
    }

    protected EffectOnPlaceable()
    {
    }

    protected EffectOnPlaceable(Placeable target, Placeable launcher)
    {
        this.target = target;
        this.launcher = launcher;
    }
    public virtual Placeable Target
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

    public virtual Placeable Launcher
    {
        get
        {
            return launcher;
        }

        set
        {
            launcher = value;
        }
    }
    public override Placeable GetTarget()
    {
        return Target;
    }
    public override void TargetAndInvokeEffectManager(LivingPlaceable placeable)
    {
        TargetAndInvokeEffectManager(placeable);
    }

    public override void TargetAndInvokeEffectManager(Placeable placeable)
    {
        this.target = placeable;
        EffectManager.instance.UseEffect(this);
    }
}
