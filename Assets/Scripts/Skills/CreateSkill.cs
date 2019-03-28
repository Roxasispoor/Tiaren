using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class CreateSkill : Skill
{
    [SerializeField]
    string cubeName;
    GameObject cubeToCreate;

    public CreateSkill(string JSON) : base(JSON)
    {
        dynamic skills = JsonConvert.DeserializeObject(JSON);
        SkillInfo info = (SkillInfo)skills.PushSkill;
        base.Init(info);
        InitSpecific(skills);
    }

    private void InitSpecific(dynamic skills)
    {
        cubeName = skills.cubeName;
        foreach(GameObject cube in Grid.instance.prefabsList)
        {
            if(cube.name == cubeName)
            {
                cubeToCreate = cube;
                return;
            }
        }
        Debug.LogError("Could not find the prefab named " + cubeName);
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        return true;
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return PatternCreate(position, vect);
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        Effect effectToConsider = new CreateBlock(cubeToCreate, new Vector3Int(0, 1, 0));
        effectToConsider.Launcher = caster;
        target.DispatchEffect(effectToConsider);
    }
}
