using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that is used for effect that must work on mother class but not child class
/// </summary>
/// <typeparam name="L"></typeparam>
/// <typeparam name="T"></typeparam>
public abstract class EffectOnTargetTypeOnly<L,T> : EffectV2<L,T> where L:NetIdeable where T:NetIdeable
{
    public override void TargetAndInvokeEffectManager(T target)
    {
        if(target.GetType().IsSubclassOf(typeof(T)))
        {

        }
        else
        { 
        Target = target;
        }
    }
}
