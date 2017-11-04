using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitablePoint : MonoBehaviour {
    private Vector3 relativePosition;
    private float damageMultiplier;

    public Vector3 RelativePosition
    {
        get
        {
            return relativePosition;
        }

        set
        {
            relativePosition = value;
        }
    }

    public float DamageMultiplier
    {
        get
        {
            return damageMultiplier;
        }

        set
        {
            damageMultiplier = value;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
