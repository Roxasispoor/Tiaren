using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectOnPlaceableOnly : EffectOnPlaceable {
    public EffectOnPlaceableOnly(EffectOnPlaceable other) : base(other)
    {
    }

    protected EffectOnPlaceableOnly()
    {
    }

    protected EffectOnPlaceableOnly(Placeable target, Placeable launcher) : base(target, launcher)
    {
    }

    //Do nothing if living
    public override void TargetAndInvokeEffectManager(LivingPlaceable placeable)
    {
      
    }
    

}
