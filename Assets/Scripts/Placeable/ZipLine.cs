using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZipLine : ObjectOnBloc {
    public ZipLine linkedTo;
    public int netIdLinkedTo;
    private void Awake()
    {
        base.Awake();
        isPickable = false;
        isPicked = false;
        dropOnDeathPicker = false;
        dropOnDeathBlocUnder = false;
        GivenSkills.Add(new Skill(0, 0, new List<Effect>() { new ZipLineTeleport(this) }, SkillType.ALREADYTARGETED, "Zip", 0, 0));
    }
    public override void Destroy()
    {

        Destroy(linkedTo.gameObject);
        Destroy(gameObject);

    }
    public override string Save()
    {
        return base.Save() + ";"+ linkedTo.netId;
    }
    public override void SomethingPutAbove()
    {
        base.SomethingPutAbove();
        if(!Grid.instance.GetPlaceableFromVector(GetPosition()+new Vector3Int(0,1,0)).IsLiving())
        {
            GameManager.instance.playingPlaceable.Player.GetComponent<UIManager>().UpdateAbilities(
                GameManager.instance.playingPlaceable, GameManager.instance.playingPlaceable.GetPosition());

           Destroy();
            
        }
       
    }
    public override void Load(string[] objectInfo)
    {
        netIdLinkedTo = Int32.Parse(objectInfo[3]);
        base.Load(objectInfo);
    }
    public override void Initialize()
    {
        base.Initialize();
        linkedTo = GameManager.instance.FindLocalIdeable(netIdLinkedTo).GetComponent<ZipLine>();
        GetComponentInChildren<ZiplineFX>().ConnectZipline(linkedTo.GetComponentInChildren<ZiplineFX>());
    }
}
