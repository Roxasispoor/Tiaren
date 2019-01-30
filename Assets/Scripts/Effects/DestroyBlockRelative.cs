using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBlockRelative : DestroyBloc {

    [SerializeField]
    private Vector3Int relativePosition;
    private bool inTargetDirection = false;
    public override void Initialize(LivingPlaceable livingPlaceable)
    {
        base.Initialize(livingPlaceable);
    }
    public DestroyBlockRelative(DestroyBlockRelative other) : base(other)
    {
        this.relativePosition = other.relativePosition;
        this.inTargetDirection = other.inTargetDirection;
    }

    public DestroyBlockRelative( Placeable Launcher,Vector3Int relativePosition,int depth=0, bool inTargetDirection = false) : base(Launcher,depth)
    {
        this.relativePosition = relativePosition;
        this.inTargetDirection = inTargetDirection;
    }
    public override Effect Clone()
    {
        return new DestroyBlockRelative(this);
    }

    public override void Use()
    {
        Target = Grid.instance.GetPlaceableFromVector(Launcher.GetPosition() + relativePosition);
        if (Target != null)
        {
            base.Use();
        }
    }
}

