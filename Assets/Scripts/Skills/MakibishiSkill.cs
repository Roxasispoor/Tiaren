using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakibishiSkill : Skill
{
    private GameObject prefabMakibishi;

    private int power;

    CreateMakibishi CreateMakibishiEffect { get { return (CreateMakibishi)effects[0]; } }

    public MakibishiSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a Makibishi skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        Init(deserializedSkill["MakibishiSkill"]);
    }

    protected override void Init(JToken deserializedSkill)
    {
        base.Init(deserializedSkill);

        string prefabName = (string)deserializedSkill["prefabName"];
        power = (int)deserializedSkill["power"];

        effects = new List<Effect>();

        foreach (GameObject prefab in Grid.instance.prefabsList)
        {
            if (prefab.name == prefabName)
            {
                prefabMakibishi = prefab;
                effects.Add(new CreateMakibishi(prefabMakibishi, Vector3Int.up, power));
                break;
            }
        }
    }

    public override void Preview(NetIdeable target)
    {
        CreateMakibishiEffect.Preview(target);
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        if(null == target as StandardCube)
        {
            return false;
        }
        return true;
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        vect = PatternCreateTop(position, vect);
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

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        CreateMakibishiEffect.Launcher = caster;

        target.DispatchEffect(CreateMakibishiEffect);
    }
}
