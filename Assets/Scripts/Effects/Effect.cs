using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// effects must be used by gameEffectManager which will resolve them
/// </summary>
public abstract class Effect
{
  
    private int turnActiveEffect; //-1 = unactive 0=stop. we use int.MaxValue/2 when it's independent
    protected Effect()
    {

    }
   
    public abstract Effect Clone();
    public abstract void TargetAndInvokeEffectManager(LivingPlaceable placeable);
    public abstract void TargetAndInvokeEffectManager(Placeable placeable);

    // Use this for initialization
    public abstract void Use();

}
