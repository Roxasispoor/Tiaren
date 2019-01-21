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
        if (GameManager.instance.isClient)
        {
            GameManager.instance.RemoveBlockFromBatch(Target);
        }
        
        animLauncher.Play("destroyBlock");
        Vector3 pos = Target.transform.position;
        AnimationHandler.Instance.StartCoroutine(AnimationHandler.Instance.WaitAndDestroyBlock(Target, GetTimeOfLauncherAnimation()));
        Grid.instance.ConnexeFall((int)pos.x, (int)pos.y, (int)pos.z);

    }
}
