using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class PushSkill : Skill
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private int nbCases;

    public PushSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a pushing skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["PushSkill"]);
        InitSpecific(deserializedSkill["PushSkill"]);
    }

    protected void InitSpecific(JToken deserializedSkill)
    {
        damage = (int)deserializedSkill["damage"];
        nbCases = (int)deserializedSkill["nbCases"];
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        Effect effectToConsider = new Push((Placeable)target, caster, 2, damage);
        effectToConsider.Launcher = caster;
        target.DispatchEffect(effectToConsider);
    }

    public override void Preview(NetIdeable target)
    {
        Debug.LogError("Preview not implemented");
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return PatternPush(position, vect);
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        if (target as Placeable == null)
        {
            Debug.LogError("target is not a placeable! Not good in push");
            return false;
        }
        return true;
    }
}
