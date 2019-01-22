using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBlock : EffectOnPlaceableOnly {

    public int prefabListNumber;
    public GameObject prefab;
    [SerializeField]
    public Vector3Int face;

    public CreateBlock()
    {
    }
    public override void Initialize()
    {
        base.Initialize();
        prefab = Grid.instance.prefabsList[prefabListNumber];
    }

    public CreateBlock(CreateBlock other) : base(other)
    {
        this.prefab = other.prefab;
        this.face = other.face;
    }

    public CreateBlock(GameObject prefab, Vector3Int face)
    {
        this.prefab = prefab;
        this.face = face;
    }

    public CreateBlock(int prefabNumber, Vector3Int face)
    {
        prefab = Grid.instance.prefabsList[prefabListNumber];
        this.face = face;
    }

    public override Effect Clone()
    {
        return new CreateBlock(this);
    }

    public override void Use()
    {
        animLauncher.Play("createBlock");
        AnimationHandler.Instance.StartCoroutine(AnimationHandler.Instance.WaitAndCreateBlock(prefab, Target.GetPosition() + face, GetTimeOfLauncherAnimation()));

    }
    
}
