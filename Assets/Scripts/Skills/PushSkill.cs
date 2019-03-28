using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PushSkill : Skill
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private int nbCases;

    public PushSkill(string JSON) : base(JSON)
    {
        dynamic skills = JsonConvert.DeserializeObject(JSON);
        SkillInfo info = (SkillInfo)skills.PushSkill;
        base.Init(info);
        InitSpecific(skills);
    }

    protected void InitSpecific(dynamic skills)
    {
        damage = skills.PushSkill.damage;
        nbCases = skills.PushSkill.nbCases;
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        Effect effectToConsider = new Push((Placeable)target, caster, 2, damage);
        effectToConsider.Launcher = caster;
        target.DispatchEffect(effectToConsider);
    }

    protected override bool CheckSpecificCondition(LivingPlaceable caster, NetIdeable target)
    {
        if(target as Placeable == null)
        {
            Debug.LogError("target is not a placeable! Not good in push");
            return false;
        }
        return true;
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        throw new System.NotImplementedException();
    }
}
