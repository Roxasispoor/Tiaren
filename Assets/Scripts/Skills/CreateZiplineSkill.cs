using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

class CreateZiplineSkill : Skill
{

    [SerializeField]
    private GameObject ziplinePrefab;

    private Vector3 topPosition;

    CreateZipLine CreateZiplineEffect { get { return (CreateZipLine)effects[0]; } }

    public CreateZiplineSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a Zipline skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        Init(deserializedSkill["CreateZiplineSkill"]);

        if (UseZipline.reference == null)
        {
            UseZipline.reference = new UseZipline(JSON);
        }
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
        else
        {
            //Position du point en haut du piquet de la tyro
            topPosition = new Vector3(0, 1.05f, 0);
            Debug.Log(topPosition);
        }
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        Placeable placeableAboveTarget = Grid.instance.GetPlaceableFromVector(target.GetPosition() + Vector3.up);

        if (placeableAboveTarget != null)
        {
            return false;
        }
        Vector3 start = caster.GetPosition() - new Vector3Int(0, 1, 0) + topPosition;
        Vector3 arrival = target.GetPosition() + topPosition;
        Vector3 direction = arrival - start;
        if (Physics.Raycast(start, direction, (direction).magnitude, LayerMask.GetMask("Cube")))
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
        CreateZiplineEffect.Preview(target);
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        vect = PatternCreate(position, vect);
        LivingPlaceable caster = (LivingPlaceable)Grid.instance.GetPlaceableFromVector(position);
        for (int i = 0; i < vect.Count; i++)
        {
            if (caster != null && !CheckSpecificConditions(caster, vect[i]))
            {
                vect.Remove(vect[i]);
            }
        }
        return vect;
    }
}

