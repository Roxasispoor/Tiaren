using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOnBloc : NetIdeable {
    public override void DispatchEffect(Effect effect)
    {
      
        effect.TargetAndInvokeEffectManager(this);
}

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
