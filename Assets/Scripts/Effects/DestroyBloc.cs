using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBloc : Effect {

	// Use this for initialization
    override
	public void Use()
    {
        if(Cible.GetType()!=typeof(LivingPlaceable))
        {
            Cible.Detruire();
        }
    }
}
