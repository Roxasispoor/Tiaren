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
    /// <summary>
    /// Used for batching.
    /// </summary>
    public CombineInstance meshInCombined;
    [SerializeField]
    public bool isSpawnPoint;

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
        this.explored = false;
        this.grounded = false;
        this.onWalkEffects = new List<Effect>();
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

    /// <summary>
    /// To call when something is put above
    /// </summary>
    public virtual void SomethingPutAbove()
    {
        foreach (Transform obj in transform.Find("Inventory"))
        {
            obj.GetComponent<ObjectOnBloc>().SomethingPutAbove();
        }
        if (isSpawnPoint)
        {
            Placeable above = Grid.instance.GetPlaceableFromVector(GetPosition() + new Vector3Int(0, 1, 0));
            if (above != null && !above.IsLiving())
            {
                above.Destroy();

                Grid.instance.Gravity(above.GetPosition().x, above.GetPosition().y, above.GetPosition().z);
                //                GameManager.instance.ResetAllBatches();
            }

        }
    }

}
