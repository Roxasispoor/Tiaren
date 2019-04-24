using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// effect to destroy a bloc
/// </summary>
public class DestroyBloc : EffectOnPlaceableOnly
{
    public int depth=1;

    private bool affectTotem = false;

    /// <summary>
    /// The direcion of to dig.
    /// </summary>
    public Vector3Int direction = Vector3Int.down;

    private Queue<MeshRenderer> meshDeactivatedForThePreview = new Queue<MeshRenderer>();
    private Queue<GameObject> gameObjectDeactivatedForThePreview = new Queue<GameObject>();
    /// <summary>
    /// Used to stock the transparent cubes used for the preview.
    /// </summary>
    private Queue<GameObject> previewedCubes = new Queue<GameObject>();

    public DestroyBloc(DestroyBloc other) : base(other)
    {
        this.depth = other.depth;
        this.affectTotem = other.affectTotem;
    }
    public DestroyBloc(int depth = 1, bool affectTotem = false) : base()
    {
        this.depth = depth;
    }
    public DestroyBloc(Placeable launcher,int depth, bool affectTotem = false) : base(launcher)
    {
        this.depth = depth;
        this.affectTotem = affectTotem;
        Launcher = launcher;
    }

    public override Effect Clone()
    {
        return new DestroyBloc(this);
    }

    public override void Preview(NetIdeable target)
    {
        if (meshDeactivatedForThePreview == null)
        {
            meshDeactivatedForThePreview = new Queue<MeshRenderer>();
        }
        if (gameObjectDeactivatedForThePreview == null)
        {
            gameObjectDeactivatedForThePreview = new Queue<GameObject>();
        }
        if (previewedCubes == null)
        {
            previewedCubes = new Queue<GameObject>();
        }
        for (int i = 0; i < depth; i++)
        {
            StandardCube block = Grid.instance.GetPlaceableFromVector(target.GetPosition() + direction * i) as StandardCube;
            bool hurtingTotem = affectTotem == true && null != block as IHurtable || null == block as IHurtable;
            if (block != null && block.Destroyable == true && hurtingTotem)
            {
                MeshRenderer meshRenderer = block.GetComponent<MeshRenderer>();
                GameManager.instance.RemoveBlockFromBatch(block);

                meshRenderer.enabled = false;
                meshDeactivatedForThePreview.Enqueue(meshRenderer);

                GameObject quad = block.QuadUp.transform.parent.gameObject;
                quad.SetActive(false);
                gameObjectDeactivatedForThePreview.Enqueue(quad);

                GameObject cube = FXManager.instance.getCube();
                cube.transform.position = target.GetPosition() + direction * i;
                cube.GetComponent<MeshRenderer>().material = FXManager.instance.materialPreviewDestroy;
                previewedCubes.Enqueue(cube);

                //bloc.Destroy();
                //oldPos.Add(bloc.GetPosition());
            }
        }
    }

    public override void ResetPreview(NetIdeable target)
    {
        //throw new System.NotImplementedException();
        while (meshDeactivatedForThePreview != null && meshDeactivatedForThePreview.Count > 0)
        {
            meshDeactivatedForThePreview.Dequeue().enabled = true;
        }
        while (gameObjectDeactivatedForThePreview != null && gameObjectDeactivatedForThePreview.Count > 0)
        {
            gameObjectDeactivatedForThePreview.Dequeue().SetActive(true);
        }
        while (previewedCubes != null && previewedCubes.Count > 0)
        {
            FXManager.instance.putBack(previewedCubes.Dequeue());
        }
    }

    // Use this for initialization
    override
    public void Use()
    {
        //Vector3 pos = Target.GetPosition();
        //Target.Destroy();
        List<Vector3> oldPos = new List<Vector3>();
        for (int i = 0; i < depth; i++)
        {
            Placeable bloc = Grid.instance.GetPlaceableFromVector(Target.GetPosition() + direction * i);
            if (bloc && !bloc.IsLiving())
            {
                bloc.Destroy();
                oldPos.Add(bloc.GetPosition());
            }
        }

        foreach (Vector3 pos in oldPos)
        {
            Grid.instance.Gravity((int)pos.x, (int)pos.y, (int)pos.z);
        }
            
    }
}
