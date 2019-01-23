using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EffectOnPlaceable : Effect {

    private Placeable target;
    public int netIdTarget = -1;

    public EffectOnPlaceable(EffectOnPlaceable other)
    {
        Target = other.target;
        Launcher = other.Launcher;
    }

    protected EffectOnPlaceable()
    {
    }

    protected EffectOnPlaceable(Placeable target, Placeable launcher)
    {
        Target = target;
        Launcher = launcher;
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
            if (target != null)
            {
                netIdTarget = target.netId;
            }
        }
    }

    public override NetIdeable GetTarget()
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
        Target = placeable;
        EffectManager.instance.UseEffect(this);
    }
    public override void TargetAndInvokeEffectManager(ObjectOnBloc placeable)
    {

    }
}
