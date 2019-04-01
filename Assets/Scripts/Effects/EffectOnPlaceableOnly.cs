using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EffectOnPlaceableOnly : EffectOnPlaceable {
    public EffectOnPlaceableOnly(EffectOnPlaceable other) : base(other)
    {
    }

    protected EffectOnPlaceableOnly()
    {
    }
    protected EffectOnPlaceableOnly( Placeable launcher) : base(launcher)
    {
    }

    protected EffectOnPlaceableOnly(Placeable target, Placeable launcher) : base(target, launcher)
    {
    }

    //Do nothing if living
    public override void TargetAndInvokeEffectManager(LivingPlaceable placeable)
    {
        Debug.LogError("Try to use an effect on placeable on a LivingPlaceable");
    }
    

}
