using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBlock : EffectOnPlaceableOnly {

    public int prefabListNumber;
    public GameObject prefab;
    /// <summary>
    /// Correspond to the offset compare ton the target.
    /// </summary>
    [SerializeField]
    public Vector3Int face;
    /// <summary>
    /// How many cube we want to create.
    /// </summary>
    [SerializeField]
    public int height=0;
    /// <summary>
    /// Used to stock the transparent cubes sused for the preview.
    /// </summary>
    private Queue<GameObject> previewedCubes = new Queue<GameObject>();

    public CreateBlock()
    {
    }
    public override void Initialize(LivingPlaceable livingPlaceable)
    {
        base.Initialize(livingPlaceable);
        prefab = Grid.instance.prefabsList[prefabListNumber];
    }

    public CreateBlock(CreateBlock other) : base(other)
    {
        this.prefab = other.prefab;
        this.face = other.face;
        this.height = other.height;
    }
    
    public override void Preview(Placeable target)
    {
        for (int i = 1; i < height + 1 ; i++)
        {
            //new CreateBlockRelativeEffect(Target, prefab, new Vector3Int(0, 1, 0), new Vector3Int(0, 1 + i, 0)).Use();
            if (!Grid.instance.CheckNull(target.GetPosition() + face * i))
            {
                break;
            }
            GameObject cube = FactoryTransparentCube.Instance.getCube();
            cube.transform.position = target.GetPosition() + face * i;
            previewedCubes.Enqueue(cube);
        }
    }

    public override void ResetPreview(Placeable target)
    {
        while (previewedCubes.Count > 0)
        {
            FactoryTransparentCube.Instance.putBack(previewedCubes.Dequeue());
        }
    }

    public CreateBlock(GameObject prefab, Vector3Int face,int height=0):base()
    {
        this.prefab = prefab;
        this.face = face;
        this.height = height;
    }
    public CreateBlock(Placeable launcher,GameObject prefab, Vector3Int face, int height = 0) : base(launcher)
    {
        this.prefab = prefab;
        this.face = face;
        this.height = height;
    }
    public CreateBlock(int prefabNumber, Vector3Int face,int height= 0)
    {
        prefabListNumber = prefabNumber;
        prefab = Grid.instance.prefabsList[prefabListNumber];
        this.face = face;
        this.height = height;
    }

    public override Effect Clone()
    {
        return new CreateBlock(this);
    }

    public override void Use()
    {
        for (int i = 1; i < height + 1; i++)
        {
            //new CreateBlockRelativeEffect(Target, prefab, new Vector3Int(0, 1, 0), new Vector3Int(0, 1 + i, 0)).Use();
            if (!Grid.instance.CheckNull(Target.GetPosition() + face * i))
            {
                break;
            }
            Grid.instance.InstantiateCube(prefab, Target.GetPosition() + face * i);
        }
    }
}
