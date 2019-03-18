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

    private bool isInAreaOfMovement = false;
    private bool isTarget = false;

    /// <summary>
    /// An array of all the quad of the cube (up; down; left; right; front; back)
    /// </summary>
    private GameObject[] quads;

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



    private GameObject QuadUp
    {
        get
        {
            return quads[0];
        }
    }

    private GameObject QuadDown
    {
        get
        {
            return quads[1];
        }
    }

    private GameObject QuadLeft
    {
        get
        {
            return quads[2];
        }
    }

    private GameObject QuadRight
    {
        get
        {
            return quads[3];
        }
    }
    

    private GameObject QuadFront
    {
        get
        {
            return quads[4];
        }
    }

    private GameObject QuadBack
    {
        get
        {
            return quads[5];
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

        quads = new GameObject[6];

        quads[0] = transform.Find("Quads").Find("QuadUp").gameObject;
        quads[1] = transform.Find("Quads").Find("QuadDown").gameObject;
        quads[2] = transform.Find("Quads").Find("QuadLeft").gameObject;
        quads[3] = transform.Find("Quads").Find("QuadRight").gameObject;
        quads[4] = transform.Find("Quads").Find("QuadFront").gameObject;
        quads[5] = transform.Find("Quads").Find("QuadBack").gameObject;
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
    /// Reset the quads of the cube to their default value.
    /// </summary>
    private void ResetQuads()
    {
        for (int i = 0; i < quads.Length; i++)
        {
            quads[i].transform.localScale = new Vector3(quads[i].transform.localScale.x, sizeQuad, 1);
            if (i == 0)  // Only for the top one
            {
                quads[i].transform.localPosition = new Vector3(quads[i].transform.localPosition.x, sizeQuad/2, quads[i].transform.localPosition.z);
            } else if (i == 1)  // Only for the down one
            {
                quads[i].transform.localPosition = new Vector3(quads[i].transform.localPosition.x, -sizeQuad/2, quads[i].transform.localPosition.z);
            } else // For the others
            {
                quads[i].transform.localPosition = new Vector3(quads[i].transform.localPosition.x, 0, quads[i].transform.localPosition.z);
            }
        }
    }

    /// <summary>
    /// Set the scale and the position of the quads to look like a tile (used for mov for example).
    /// </summary>
    private void SetQuadsAsTile()
    {
        float heightSize = 0.2f;

        QuadUp.transform.localScale = new Vector3(QuadUp.transform.localScale.x, sizeQuad, 1);
        QuadUp.transform.localPosition = new Vector3(QuadUp.transform.localPosition.x, sizeQuad / 2, QuadUp.transform.localPosition.z);
        QuadUp.SetActive(true);

        QuadDown.transform.localScale = new Vector3(QuadDown.transform.localScale.x, sizeQuad, 1);
        QuadDown.transform.localPosition = new Vector3(QuadDown.transform.localPosition.x, 0.5f - heightSize + 0.01f, QuadDown.transform.localPosition.z);
        QuadDown.SetActive(true);

        for (int i = 2; i < quads.Length; i++) // For the rest of the quads
        {
            quads[i].transform.localScale = new Vector3(quads[i].transform.localScale.x, heightSize + 0.01f, 1); 
            quads[i].transform.localPosition = new Vector3(quads[i].transform.localPosition.x, 0.5f - heightSize / 2 + 0.01f, quads[i].transform.localPosition.z);
        }
    }

    /// <summary>
    /// Highlight when the element is Hovered.
    /// </summary>
    public override void Highlight()
    {
        if (QuadUp.activeInHierarchy && QuadUp.GetComponent<MeshRenderer>().material == GameManager.instance.highlightingMaterial)
        {
            return;
        }

        foreach (GameObject quad in quads)
        {
            quad.SetActive(true);
            quad.GetComponent<MeshRenderer>().material = GameManager.instance.highlightingMaterial;
        }
    }

    /// <summary>
    /// Unhighlight when the element is unhovered.
    /// </summary>
    public override void UnHighlight()
    {
        //If we are in move mode doesn't belong to path we desactivate it
        if (isInAreaOfMovement)

        {
            foreach (GameObject quad in quads)
            {
                quad.GetComponent<MeshRenderer>().material = GameManager.instance.pathFindingMaterial;
            }
        } else if (isTarget)
        {
            foreach (GameObject quad in quads)
            {
                quad.GetComponent<MeshRenderer>().material = GameManager.instance.targetMaterial;
            }
        }
        else
        {
            foreach (GameObject quad in quads)
            {
                quad.SetActive(false);
            }
        }

    }

    /// <summary>
    /// Set the quads as a tile, apply the pathfinding material to them and acivate them.
    /// </summary>
    public void HighlightForMovement()
    {
        isInAreaOfMovement = true;
        SetQuadsAsTile();
        foreach (GameObject quad in quads)
        {
            quad.gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.pathFindingMaterial;
            quad.SetActive(true);
        }
    }

    /// <summary>
    /// Reset the quads and deactivate them.
    /// </summary>
    public void UnhighlightForMovement()
    {
        isInAreaOfMovement = false;
        ResetQuads();
        foreach (GameObject quad in quads)
        {
            quad.SetActive(false);
        }
    }

    /// <summary>
    /// Apply the target material to all the quads and activate them.
    /// </summary>
    public void HighlightForSkill()
    {
        isTarget = true;
        foreach (GameObject quad in quads)
        {
            quad.gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.targetMaterial;
            quad.SetActive(true);
        }
    }

    /// <summary>
    /// Apply the target material to all the quads and activate some of them.
    /// </summary>
    /// <param name="up">Activate quad up</param>
    /// <param name="down">Activate quad down</param>
    /// <param name="left">Activate quad left</param>
    /// <param name="right">Activate quad right</param>
    /// <param name="front">Activate quad front</param>
    /// <param name="back">Activate quad back</param>
    public void HighlightForSkill(bool up, bool down, bool left, bool right, bool front, bool back)
    {
        isTarget = true;
        QuadUp.SetActive(up);
        QuadDown.SetActive(down);
        QuadLeft.SetActive(left);
        QuadRight.SetActive(right);
        QuadFront.SetActive(front);
        QuadBack.SetActive(back);
        foreach (GameObject quad in quads)
        {
            quad.gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.targetMaterial;
        }
    }

    /// <summary>
    /// Reset the quads and deactivate them (+ deactivate the isTarget bool)
    /// </summary>
    // TODO : May be possible to merge some highlight target
    public void UnhighlightForSkill()
    {
        isTarget = false;
        ResetQuads();
        foreach (GameObject quad in quads)
        {
            quad.SetActive(false);
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
