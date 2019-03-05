using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Cube placeable standard
/// </summary>
[Serializable]
public class StandardCube : Placeable
{

    [NonSerialized]
    public Batch batch;
    protected List<Effect> onWalkEffects;
    protected bool destroyable;

    public List<Effect> OnWalkEffects
    {
        get
        {
            return onWalkEffects;
        }

        set
        {
            onWalkEffects = value;
        }
    }

    public bool Destroyable
    {
        get
        {
            return destroyable;
        }

        set
        {
            destroyable = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        this.Walkable = true;
        
        this.Movable = true;
        this.Destroyable = true;

        this.TraversableChar = TraversableType.NOTHROUGH;
        this.TraversableBullet = TraversableType.NOTHROUGH;

        this.GravityType = GravityType.RELATED_GRAVITY;
       
        this.crushable = CrushType.CRUSHSTAY;
        this.Explored = false;
        this.Grounded = false;
        this.onWalkEffects = new List<Effect>();
        this.OnDestroyEffects = new List<Effect>();
        this.OnStartTurn = new List<Effect>();
        this.OnEndTurn = new List<Effect>();
        this.AttachedEffects = new List<Effect>();
    }

    /// <summary>
    /// method to call for destroying object
    /// </summary>
    override
    public void Destroy()
    {
        if (this.Destroyable)
        {
            Grid.instance.GridMatrix[GetPosition().x, GetPosition().y, GetPosition().z] = null;
            foreach (Effect effect in this.OnDestroyEffects)
            {
                EffectManager.instance.DirectAttack(effect);
            }
            foreach (Transform obj in transform.Find("Inventory"))
            {
                obj.GetComponent<ObjectOnBloc>().Destroy();
            }
            if (!IsLiving())
            {
                GameManager.instance.RemoveBlockFromBatch((StandardCube)this);
            }
            gameObject.SetActive(false);
            this.UnHighlight();
            Destroy(this.gameObject);
        }

    }

}
