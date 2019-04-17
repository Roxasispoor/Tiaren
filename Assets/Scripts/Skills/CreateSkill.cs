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

    CreateBlock CreateEffect { get { return (CreateBlock)effects[0]; } }

    public CreateSkill(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a creation skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["CreateSkill"]);
        InitSpecific(deserializedSkill["CreateSkill"]);
    }

    private void InitSpecific(JToken deserializedSkill)
    {
        effects = new List<Effect>();
        cubeName = (string)deserializedSkill["cubeName"];
        foreach(GameObject cube in Grid.instance.prefabsList)
        {
            if(cube.name == cubeName)
            {
                cubeToCreate = cube;
                effects.Add(new CreateBlock(cubeToCreate, new Vector3Int(0, 1, 0)));
                return;
            }
        }
        Debug.LogError("Could not find the prefab named " + cubeName);
    }


    // TODO : Maybe make to check, one for use and one for the vision
    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        StandardCube targetedCube = target as StandardCube;
        if (target == null)
        {
            //Debug.Log("Cannot use CreateSkill because the tager is null or not a cube.");
            return false;
        }

        /* Check if the face is free
        Vector3 potentialPositionToCreate = target.GetPosition() + GameManager.instance.RaycastSelector.CurrentHovered.face;
        return Grid.instance.CheckNull(potentialPositionToCreate); ;
        */
        return true;
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        vect =  PatternCreateTop(vect);
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
        CreateEffect.Launcher = caster;
        CreateEffect.face = CollectSelectedFace(false);
        target.DispatchEffect(CreateEffect);
    }

    public override void Preview(NetIdeable target)
    {
        CreateEffect.face = CollectSelectedFace(true);

        if (CheckConditions(GameManager.instance.PlayingPlaceable, target))
        {
            CreateEffect.Preview((Placeable)target);
        }
    }
}
