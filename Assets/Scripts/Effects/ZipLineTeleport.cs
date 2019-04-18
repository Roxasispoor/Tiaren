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

    public override void Preview(NetIdeable target)
    {
        Debug.Log("No preview for ZipLineTeleport");
    }

    public override void ResetPreview(NetIdeable target)
    {
         Debug.Log("No preview for ZipLineTeleport");
    }

    public override void Use()
    {
        ZipLine arrival = ((ZipLine)Target).linkedTo;
        ZipLine start = (ZipLine)Target;
        Vector3 direction = arrival.gameObject.GetComponentInChildren<ZiplineFX>().TopPole.position - start.gameObject.GetComponentInChildren<ZiplineFX>().TopPole.position;
       // Debug.DrawRay(Target.GetPosition() + new Vector3(0, 1.5f, 0), arrival - Target.GetPosition(),Color.green,10f);
        if (arrival.GetPosition().y<Target.GetPosition().y && Grid.instance.CheckNull(arrival.GetPosition() + new Vector3Int(0,1,0))
            && !Physics.Raycast(start.gameObject.GetComponentInChildren<ZiplineFX>().TopPole.position, direction,
            (direction).magnitude,LayerMask.GetMask("Totems") | LayerMask.GetMask("Cube")))
        {
            Grid.instance.MovePlaceable((Placeable)Launcher, arrival.GetPosition() + new Vector3Int(0, 1, 0), GameManager.instance.isServer);
            
            if (GameManager.instance.isClient)
            {

                List<Vector3> path = new List<Vector3>() { start.GetPosition() + Vector3Int.up, arrival.GetPosition() + Vector3Int.up };
                GameManager.instance.PlayingPlaceable.Player.FollowPathAnimation(path, (Placeable)Launcher, null, 4f, false);
               
            }
            if(((Placeable)Launcher).Player.isLocalPlayer)
            {
                GameManager.instance.PlayingPlaceable.Player.GetComponent<UIManager>().updateSpecialAbilities(
                   (LivingPlaceable)Launcher, arrival.GetPosition() + new Vector3Int(0, 1, 0));
            }
        }
    }
}
