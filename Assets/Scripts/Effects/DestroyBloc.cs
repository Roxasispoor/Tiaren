using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// effect to destroy a bloc
/// </summary>
public class DestroyBloc : EffectOnPlaceable
{
    public DestroyBloc(DestroyBloc other) : base(other)
    {
    }

    public override Effect Clone()
    {
        return new DestroyBloc(this);
    }

    // Use this for initialization
    override
    public void Use()
    {
        if (Target.GetType() != typeof(LivingPlaceable))
        {
            Target.DestroyLivingPlaceable();
        }
    }
}
