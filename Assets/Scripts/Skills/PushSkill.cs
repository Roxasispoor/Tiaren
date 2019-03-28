using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PushSkill : Skill
{
    public PushSkill(string JSON) : base(JSON)
    {
        dynamic skills = JsonConvert.DeserializeObject(JSON);
        SkillInfo info = (SkillInfo)skills.PushSkill;
        base.Init(info);
        Init(skills);
    }

    /// <summary>
    /// Initialised properties specific to this skill and adds effects
    /// </summary>
    /// <param name="JSON"></param>
    protected override void Init(dynamic JSON)
    {
        effects.Add(new Push(null, null, 2, 20));
    }

    public override bool Use(LivingPlaceable caster, List<NetIdeable> targets)
    {
        Debug.LogError("no use yet");
        return false;
    }
}
