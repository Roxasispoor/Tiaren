using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingTotemLinkSkill : Skill
{

    public static HealingTotemLinkSkill reference;

    HealingEffect HealingEffect { get { return (HealingEffect)effects[0]; } }

    public HealingTotemLinkSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating Link heal");
        JObject deserializedSkill = JObject.Parse(JSON);
        Init(deserializedSkill["HealingTotemLinkSkill"]);
        oneClickUse = true;
        linkskill = true;
    }

    protected override void Init(JToken deserializedSkill)
    {
        base.Init(deserializedSkill);
    }

    public HealingTotemLinkSkill() : base()
    {
        oneClickUse = true;
        linkskill = true;
    }

    public static HealingTotemLinkSkill CreateNewInstanceFromReferenceAndSetTarget(Totem totem)
    {
        HealingTotemLinkSkill newHealLink = new HealingTotemLinkSkill();
        newHealLink.CopyMainVariables(HealingTotemLinkSkill.reference);
        newHealLink.effects.Add(new HealingEffect(1));
        newHealLink.HealingEffect.Target = totem;
        return newHealLink;
    }

    public override void Preview(NetIdeable target)
    {
        //TODO
    }

    public override void UnPreview(NetIdeable target)
    {
        //todo
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        return true;
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return vect;
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        HealingEffect.Launcher = caster;
        HealingEffect.Target.DispatchEffect(HealingEffect);

        Skill skillToSynchronise = GameManager.instance.PlayingPlaceable.Skills.Find(o => o.SkillName == "Totem Linking");
        if (null != skillToSynchronise)
        {
            skillToSynchronise.cooldownTurnLeft = 1;
        }
    }
}
