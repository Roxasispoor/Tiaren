using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBlockRelativeEffect : CreateBlock
{
    [SerializeField]
    private Vector3Int relativePosition;
    /// <summary>
    /// Warning not used!
    /// </summary>
    private bool inTargetDirection = false;

    public override void Initialize(LivingPlaceable livingPlaceable)
    {
        base.Initialize(livingPlaceable);
    }

    public CreateBlockRelativeEffect(CreateBlockRelativeEffect other) : base(other)
    {
        this.relativePosition = other.relativePosition;
        this.inTargetDirection = other.inTargetDirection;
    }

    public CreateBlockRelativeEffect(GameObject prefab, Vector3Int face, Vector3Int relativePosition, bool inTargetDirection = false) : base(prefab, face)
    {
        this.relativePosition = relativePosition;
        this.inTargetDirection = inTargetDirection;
    }
    public override Effect Clone()
    {
        return new CreateBlockRelativeEffect(this);
    }

    public override void Use()
    {
        Target = Grid.instance.GetPlaceableFromVector(Launcher.GetPosition() + relativePosition);
        if(Target!=null && relativePosition.y < 0 && relativePosition.x == 0 && relativePosition.z == 0
            && Grid.instance.CheckNull(Launcher.GetPosition() + new Vector3Int(0, 1, 0)))//if under us, check if we can go higher
        {
            base.Use();
        }
    }
}
