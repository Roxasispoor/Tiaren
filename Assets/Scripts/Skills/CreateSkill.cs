using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class CreateSkill : Skill
{
    [SerializeField]
    string cubeName;
    GameObject cubeToCreate;

    public CreateSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a creation skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["CreateSkill"]);
        InitSpecific(deserializedSkill["CreateSkill"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        cubeName = (string)deserializedSkill["cubeName"];
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
