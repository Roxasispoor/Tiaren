using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// effect to destroy a bloc
/// </summary>
public class DestroyBloc : EffectOnPlaceableOnly
{
    public DestroyBloc(DestroyBloc other) : base(other)
    {
    }
    public DestroyBloc() : base()
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
        
        animLauncher.Play("destroyBlock");
        
        AnimationHandler.Instance.StartCoroutine(AnimationHandler.Instance.WaitAndDestroyBlock(Target, GetTimeOfLauncherAnimation()));
        //

    }
}
