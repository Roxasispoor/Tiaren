using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBlock : EffectOnPlaceableOnly {

    public GameObject prefab;
    public Vector3Int face;

    public CreateBlock()
    {
    }

    public CreateBlock(CreateBlock other) : base(other)
    {
        this.prefab = other.prefab;
        this.face = other.face;
    } 
    public CreateBlock(GameObject prefab,Vector3Int face)
    {
        this.prefab = prefab;
        this.face = face;
    }
    public override Effect Clone()
    {
        return new CreateBlock(this);
    }

    public override void Use()
    {

        Grid.instance.InstantiateCube(prefab, Target.GetPosition() + face);
       
    }
    
}
