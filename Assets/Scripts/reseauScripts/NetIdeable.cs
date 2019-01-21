using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetIdeable : MonoBehaviour {

    [NonSerialized]
    public int netId;
    protected List<Effect> attachedEffects;
    public Vector3Int GetPosition()
    {
        return new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

    }
    public List<Effect> AttachedEffects
    {
        get
        {
            return attachedEffects;
        }

        set
        {
            attachedEffects = value;
        }
    }
    public abstract void DispatchEffect(Effect effect);
}
