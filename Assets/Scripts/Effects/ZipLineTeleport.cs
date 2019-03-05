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
       // Debug.DrawRay(Target.GetPosition() + new Vector3(0, 1.5f, 0), zip - Target.GetPosition(),Color.green,10f);
        if (zip.y<Target.GetPosition().y && Grid.instance.CheckNull(zip + new Vector3Int(0,1,0))
            && !Physics.Raycast(Target.GetPosition()+new Vector3(0,1.5f,0),  zip- Target.GetPosition(),
            (Target.GetPosition() - zip).magnitude,LayerMask.GetMask("Placeable"))
            )
        {
            Grid.instance.MoveBlock(Launcher, zip + new Vector3Int(0, 1, 0), GameManager.instance.isServer);
            
            if (GameManager.instance.isClient)
            {

                List<Vector3> path = new List<Vector3>() { Launcher.GetPosition() + Vector3Int.down, zip };// + new Vector3Int(0, 1, 0) };
                GameManager.instance.playingPlaceable.Player.StartMoveAlongBezier(path, Launcher, 4f, true);
               
            }
            if(Launcher.Player.isLocalPlayer)
            {
                GameManager.instance.playingPlaceable.Player.GetComponent<UIManager>().UpdateAbilities(
                   (LivingPlaceable)Launcher, zip + new Vector3Int(0, 1, 0));
            }

        }
    }
}
