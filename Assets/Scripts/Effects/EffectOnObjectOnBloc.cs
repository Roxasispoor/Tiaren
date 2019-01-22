﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectOnObjectBloc : Effect {
    public ObjectOnBloc target;
    public int netIdTarget = -1;

    public override NetIdeable GetTarget()
    {
        return Target;
    }
    public virtual ObjectOnBloc Target
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
    public override void TargetAndInvokeEffectManager(LivingPlaceable placeable)
    {
     }

    public override void TargetAndInvokeEffectManager(Placeable placeable)
    {
       }

    public override void TargetAndInvokeEffectManager(ObjectOnBloc placeable)//TODO, doesn't work
    {
        Target = placeable;
        EffectManager.instance.UseEffect(this);
    }

    public EffectOnObjectBloc(EffectOnObjectBloc other)
    {
        Target = other.target;
        Launcher = other.Launcher;
    }
    public EffectOnObjectBloc(ObjectOnBloc target)
    {
        Target = target;
    }
    public EffectOnObjectBloc():base()
    {
        }
}
