using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class DestroySkill : Skill
{
    public DestroySkill(string JSON) : base(JSON)
    {
        dynamic skills = JsonConvert.DeserializeObject(JSON);
        SkillDataFromJSON info = (SkillDataFromJSON)skills.PushSkill;
        base.Init(info);
        InitSpecific(skills);
    }

    private void InitSpecific(dynamic skills)
    {
        
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        throw new System.NotImplementedException();
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return PatternDestroy(position, vect);
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        throw new System.NotImplementedException();
    }
}
