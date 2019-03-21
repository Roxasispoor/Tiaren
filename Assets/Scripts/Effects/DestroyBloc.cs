using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// effect to destroy a bloc
/// </summary>
public class DestroyBloc : EffectOnPlaceableOnly
{
    public int depthExceed=0;

    private Queue<MeshRenderer> meshDeactivatedForThePreview = new Queue<MeshRenderer>();
    private Queue<GameObject> gameObjectDeactivatedForThePreview = new Queue<GameObject>();
    /// <summary>
    /// Used to stock the transparent cubes used for the preview.
    /// </summary>
    private Queue<GameObject> previewedCubes = new Queue<GameObject>();

    public DestroyBloc(DestroyBloc other) : base(other)
    {
        this.depthExceed = other.depthExceed;
    }
    public DestroyBloc(int depth = 0) : base()
    {
        depthExceed = depth;
    }
    public DestroyBloc(Placeable launcher,int depth) : base(launcher)
    {
        depthExceed = depth;
        Launcher = launcher;
    }

    public override Effect Clone()
    {
        return new DestroyBloc(this);
    }

    public override void Preview(Placeable target)
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
        //throw new System.NotImplementedException();
        for (int i = 0; i < depthExceed + 1; i++)
        {
            StandardCube bloc = Grid.instance.GetPlaceableFromVector(target.GetPosition() + Vector3Int.down * i) as StandardCube;
            if (bloc)
            {

                MeshRenderer meshRenderer = bloc.GetComponent<MeshRenderer>();

                meshRenderer.enabled = false;
                meshDeactivatedForThePreview.Enqueue(meshRenderer);

                GameObject quad = bloc.QuadUp.transform.parent.gameObject;
                quad.SetActive(false);
                gameObjectDeactivatedForThePreview.Enqueue(quad);

                GameObject cube = FactoryTransparentCube.Instance.getCube();
                cube.transform.position = target.GetPosition() + Vector3Int.down * i;
                cube.GetComponent<MeshRenderer>().material = GameManager.instance.materialPreviewDestroy;
                previewedCubes.Enqueue(cube);

                //bloc.Destroy();
                //oldPos.Add(bloc.GetPosition());
            }
        }
    }

    public override void ResetPreview(Placeable target)
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
            FactoryTransparentCube.Instance.putBack(previewedCubes.Dequeue());
        }
    }

    // Use this for initialization
    override
    public void Use()
    {
        //Vector3 pos = Target.GetPosition();
        //Target.Destroy();
        List<Vector3> oldPos = new List<Vector3>();
        for (int i = 0; i <depthExceed + 1;i++)
        {
            Placeable bloc = Grid.instance.GetPlaceableFromVector(Target.GetPosition() + Vector3Int.down * i);
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
