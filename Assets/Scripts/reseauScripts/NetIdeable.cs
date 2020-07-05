using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetIdeable : MonoBehaviour {
    public int serializeNumber;
    public int netId;
    protected List<Effect> attachedEffects;
    public bool shouldBatch=true;
    public bool isMoving=false;
    public Vector3Int logicPosition = Vector3Int.down;
    public static int currentMaxId = 0;

    [SerializeField]
    private Transform visualTransorm;

    public Transform VisualTransform
    {
        get
        {
            if (visualTransorm == null)
            {
                return transform;
            }

            return visualTransorm;
        }
    }


    public Vector3Int GetPosition()
    {
        if(logicPosition == Vector3Int.down)
        { 
        return new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        }
        else
        {
            return logicPosition;

        }
    }
    public List<Effect> AttachedEffects
    {
        get
        {
            if (attachedEffects == null)
            {
                attachedEffects = new List<Effect>();
            }
            return attachedEffects;
        }

        set
        {
            attachedEffects = value;
        }
    }

    public void ResetVisualTransform()
    {
        VisualTransform.position = logicPosition;
    }

    public abstract void DispatchEffect(Effect effect);
    public virtual bool IsLiving()
    {
        return false;
    }
}
