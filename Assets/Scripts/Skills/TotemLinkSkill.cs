using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemLinkSkill : Skill
{

    public TotemLinkSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a totem link skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["TotemLinkSkill"]);
        Init(deserializedSkill["TotemLinkSkill"]);
    }

    protected override void Init(JToken deserializedSkill)
    {
        base.Init(deserializedSkill);

    }

    public override void Preview(NetIdeable target)
    {
        Debug.Log("No preview for : " + skillName);
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        return true;
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        LivingPlaceable caster = (LivingPlaceable)Grid.instance.GetPlaceableFromVector(position);
        vect.Clear();
        foreach(NetIdeable obj in caster.LinkedObjects)
        {
            if(null != obj.GetComponent<Totem>())
            {
                vect.Add((StandardCube)obj);
            }
        }
        return vect;
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        if (caster.LinkedObjects.Contains(target))
        {
            caster.ActiveLink = target;
            GameManager.instance.State = States.Link;
            Debug.Log("connected to " + target.ToString());
        }
        else
        {
            Debug.LogError("target is not in linked objects, can't connect");
            return;
        }

        if (GameManager.instance.isClient && GameManager.instance.GetLocalPlayer() == caster.Player)
        {
            Player player = GameManager.instance.GetLocalPlayer();
            player.cameraScript.SetTarget(target.transform);
            if(null != target.GetComponent<Totem>()) {
                player.GetComponent<UIManager>().ActivateLinkUI(target.GetComponent<Totem>());
            }
        }
    }
}
