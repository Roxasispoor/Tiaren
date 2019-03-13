using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBlock : EffectOnPlaceableOnly {

    public int prefabListNumber;
    public GameObject prefab;
    [SerializeField]
    public Vector3Int face;
    [SerializeField]
    public int height=0;

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
    
    public override void preview()
    {
        throw new System.NotImplementedException();
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
        Grid.instance.InstantiateCube(prefab, Target.GetPosition() + face);
        for (int i = 0; i < height; i++)
        {
           new CreateBlockRelativeEffect(Target,prefab, new Vector3Int(0, 1, 0),new Vector3Int(0,1+i,0)).Use();
        }
    }
    
}
