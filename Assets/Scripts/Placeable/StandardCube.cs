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


    /// <summary>
    /// Size of the child quads.
    /// </summary>
    private const float sizeQuad = 1.02f;

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
        this.walkable = true;
        
        this.movable = true;
        this.Destroyable = true;

        this.traversableType = TraversableType.NOTHROUGH;
        this.traversableBullet = TraversableType.NOTHROUGH;

        this.gravityType = GravityType.RELATED_GRAVITY;
       
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

    public override void Highlight()
    {
        if (GameManager.instance.activeSkill != null && GameManager.instance.activeSkill.SkillType == SkillType.BLOCK)
        {
            GameObject quadUp = transform.Find("Quads").Find("QuadUp").gameObject;
            GameObject quadRight = transform.Find("Quads").Find("QuadRight").gameObject;
            GameObject quadLeft = transform.Find("Quads").Find("QuadLeft").gameObject;
            GameObject quadFront = transform.Find("Quads").Find("QuadFront").gameObject;
            GameObject quadBack = transform.Find("Quads").Find("QuadBack").gameObject;

            quadUp.SetActive(true);

            quadRight.SetActive(true);
            quadRight.transform.localScale = new Vector3(quadRight.transform.localScale.x, sizeQuad, 1);
            quadRight.transform.localPosition = new Vector3(quadRight.transform.localPosition.x, 0, quadRight.transform.localPosition.z);

            quadLeft.SetActive(true);
            quadLeft.transform.localScale = new Vector3(quadLeft.transform.localScale.x, sizeQuad, 1);
            quadLeft.transform.localPosition = new Vector3(quadLeft.transform.localPosition.x, 0, quadLeft.transform.localPosition.z);

            quadFront.SetActive(true);
            quadFront.transform.localScale = new Vector3(quadFront.transform.localScale.x, sizeQuad, 1);
            quadFront.transform.localPosition = new Vector3(quadFront.transform.localPosition.x, 0, quadFront.transform.localPosition.z);

            quadBack.SetActive(true);
            quadBack.transform.localScale = new Vector3(quadBack.transform.localScale.x, sizeQuad, 1);
            quadBack.transform.localPosition = new Vector3(quadBack.transform.localPosition.x, 0, quadBack.transform.localPosition.z);

        }
        foreach (Transform fils in transform.Find("Quads"))
        {

            fils.gameObject.SetActive(true);
            fils.gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.highlightingMaterial;
        }
    }

    public override void UnHighlight()
    {

        //Put back the default material
        foreach (Transform fils in transform.Find("Quads"))
        {
            fils.gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.pathFindingMaterial;
        }
        //If we are in move mode doesn't belong to path we desactivate it
        if (GameManager.instance.State != States.Move || GameManager.instance.PlayingPlaceable != null && GameManager.instance.PlayingPlaceable.AreaOfMouvement != null &&
            !GameManager.instance.PlayingPlaceable.AreaOfMouvement.Exists(new NodePath(GetPosition().x, GetPosition().y, GetPosition().z, 0, null).Equals))

        {


            foreach (Transform fils in transform.Find("Quads"))
            {

                fils.gameObject.SetActive(false);

            }
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
