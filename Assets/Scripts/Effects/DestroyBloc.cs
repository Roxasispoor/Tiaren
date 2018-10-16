using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// effect to destroy a bloc
/// </summary>
public class DestroyBloc : Effect
{

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
