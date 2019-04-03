using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

class CreateZiplineSkill : Skill
{

    [SerializeField]
    private GameObject ziplinePrefab;

    CreateZipLine CreateZiplineEffect { get { return (CreateZipLine)effects[0]; } }

    public CreateZiplineSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a Zipline skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        Init(deserializedSkill["CreateZiplineSkill"]);
    }

    protected override void Init(JToken deserializedSkill)
    {
        base.Init(deserializedSkill);

        string prefabZiplineName = (string)deserializedSkill["prefabZiplineName"];

        effects = new List<Effect>();

        foreach (GameObject prefab in Grid.instance.prefabsList)
        {
            if (prefab.name == prefabZiplineName)
            {
                ziplinePrefab = prefab;
                effects.Add(new CreateZipLine(ziplinePrefab, Vector3Int.up));
                break;
            }
        }
        if (ziplinePrefab == null)
        {
            Debug.LogError(skillName + ": could not find the prefab");
        }
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        Placeable placeableAboveTarget = Grid.instance.GetPlaceableFromVector(target.GetPosition() + Vector3.up);

        if (placeableAboveTarget != null)
        {
            return false;
        }
        return true;
    }


    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        CreateZiplineEffect.Launcher = caster;
        CreateZiplineEffect.Target = caster;

        target.DispatchEffect(CreateZiplineEffect);

    }

    public override void Preview(NetIdeable target)
    {
        Debug.LogError(SkillName + ": no preview");
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return PatternCreate(position, vect);
    }
}

