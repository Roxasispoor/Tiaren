using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZipLineTeleport : EffectOnObjectBloc
{

    public ZipLineTeleport(ZipLine target):base(target)
    {

    }
    public ZipLineTeleport(ZipLineTeleport other) : base(other)
    {

    }
    public override Effect Clone()
    {
        return new ZipLineTeleport(this);
    }
    public override void Use()
    {
        Vector3Int zip = ((ZipLine)Target).linkedTo.GetPosition();
        if (zip.y<Target.GetPosition().y && Grid.instance.CheckNull(zip + new Vector3Int(0,1,0))
            && !Physics.Raycast(Target.GetPosition()+new Vector3(0,0.5f,0), Target.GetPosition()- zip,
            (Target.GetPosition() - zip).magnitude*0.95f,LayerMask.GetMask("Placeable"))
            )
        {
            Grid.instance.MoveBlock(Launcher, zip + new Vector3Int(0, 1, 0),false);
            
            if (GameManager.instance.isClient)
            {

                List<Vector3> path = new List<Vector3>() { Launcher.GetPosition(), zip + new Vector3Int(0, 1, 0) };
                GetLauncherAnimation();
                AnimationHandler.Instance.StartCoroutine(AnimationHandler.Instance.WaitAndPushBlock(Launcher, path, 4f, GetTimeOfLauncherAnimation()));
            }

        }
    }
}
