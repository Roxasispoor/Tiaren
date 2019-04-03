using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZipLine : ObjectOnBloc {
    public ZipLine linkedTo;
    public int netIdLinkedTo;
    public LineRenderer rope;
    private bool outlined;
    protected override void Awake()
    {
        base.Awake();
        isPickable = false;
        isPicked = false;
        dropOnDeathPicker = false;
        dropOnDeathBlocUnder = false;
        Debug.LogError("No skill created Yet !");
        //TODO: create the skill
        //Skill newSkill = new Skill(0, 0, new List<Effect>() { new ZipLineTeleport(this) }, TargetType.ALREADYTARGETED, "Zip", 0, 0);
        //newSkill.Description = "Use the Zipline to go down";
        //GivenSkills.Add(newSkill);
        //Resources.Load<Sprite>("UI_Images/Abilities" + newSkill.SkillName);
    }

    public void Outline()
    {
        if (!outlined)
        {
            gameObject.GetComponentInChildren<MeshRenderer>().material.shader = GameManager.instance.PlayingPlaceable.outlineShader;
            linkedTo.GetComponentInChildren<MeshRenderer>().material.shader = GameManager.instance.PlayingPlaceable.outlineShader;
            outlined = true;
        }
        else
        {
            gameObject.GetComponentInChildren<MeshRenderer>().material.shader = GameManager.instance.PlayingPlaceable.originalShader;
            linkedTo.GetComponentInChildren<MeshRenderer>().material.shader = GameManager.instance.PlayingPlaceable.originalShader;
            outlined = false;
        }
    }

    public override void Destroy()
    {
        GivenSkills.Clear();
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
            GameManager.instance.PlayingPlaceable.Player.GetComponent<UIManager>().updateSpecialAbilities(GameManager.instance.PlayingPlaceable,
                GameManager.instance.PlayingPlaceable.GetPosition());
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
        rope = GetComponentInChildren<ZiplineFX>().ConnectZipline(linkedTo.GetComponentInChildren<ZiplineFX>(), false);
    }
}
