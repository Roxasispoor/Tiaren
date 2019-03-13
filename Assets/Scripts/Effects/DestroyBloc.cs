using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// effect to destroy a bloc
/// </summary>
public class DestroyBloc : EffectOnPlaceableOnly
{
    public int depthExceed=0;
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

    public override void preview()
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    override
    public void Use()
    {
        Vector3 pos = Target.GetPosition();
        Target.Destroy();

        for (int i = 0; i <depthExceed;i++)
        {
             new DestroyBlockRelative(Target, new Vector3Int(0, -i-1, 0)).Use();
        }

        Grid.instance.Gravity((int)pos.x, (int)pos.y, (int)pos.z);
    }
}
