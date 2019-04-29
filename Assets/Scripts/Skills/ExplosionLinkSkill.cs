using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionLinkSkill : Skill
{

    public static ExplosionLinkSkill reference;

    DamageCalculated DamageEffect { get { return (DamageCalculated)effects[0]; } }

    private Totem Launcher;

    public ExplosionLinkSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating an explosive link");
        JObject deserializedSkill = JObject.Parse(JSON);
        Init(deserializedSkill["ExplosionLinkSkill"]);
        oneClickUse = true;
        throughblocks = true;
        linkskill = true;
    }

    protected override void Init(JToken deserializedSkill)
    {
        base.Init(deserializedSkill);
    }

    public ExplosionLinkSkill(Totem totem) : base()
    {
        oneClickUse = true;
        throughblocks = true;
        linkskill = true;
        Launcher = totem;
        MaxRange = Launcher.Range;
    }

    public static ExplosionLinkSkill CreateNewInstanceFromReferenceAndSetTarget(Totem totem)
    {
        ExplosionLinkSkill newExplosionLink = new ExplosionLinkSkill(totem);
        newExplosionLink.CopyMainVariables(ExplosionLinkSkill.reference);
        newExplosionLink.effects.Add(new DamageCalculated(15, DamageCalculated.DamageScale.BRUT, 0));
        return newExplosionLink;
    }

    public override void Preview(NetIdeable target)
    {
        GameManager.instance.PlayingPlaceable.TargetableHurtable = Grid.instance.FindTargetableLiving(Launcher.GetPosition(), this);
        foreach(Placeable hurtable in GameManager.instance.PlayingPlaceable.TargetableHurtable)
        {
            DamageEffect.Preview(hurtable);
        }
    }

    public override void UnPreview(NetIdeable target)
    {
        foreach (Placeable hurtable in GameManager.instance.PlayingPlaceable.TargetableHurtable)
        {
            DamageEffect.ResetPreview(hurtable);
        }
        GameManager.instance.PlayingPlaceable.TargetableHurtable = new List<Placeable>();
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
        List<Placeable> targets = Grid.instance.FindTargetableLiving(Launcher.GetPosition(), this);
        for(int i = targets.Count - 1; i >= 0; i--)
        {
            targets[i].DispatchEffect(DamageEffect);
        }
        Skill skillToSynchronise = GameManager.instance.PlayingPlaceable.Skills.Find(o => o.SkillName == "Totem Linking");
        if(null != skillToSynchronise)
        {
            skillToSynchronise.cooldownTurnLeft = 1;
        }
    }
}
