using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class CreateTotemSkill : Skill
{
    [SerializeField]
    string cubeName;
    int power;


    GameObject cubeToCreate;

    CreateTotemEffect CreateEffect { get { return (CreateTotemEffect)effects[0]; } }

    public CreateTotemSkill(string JSON) : base(JSON)
    {
    }

    protected override void Init(JToken deserializedSkill)
    {
        base.Init(deserializedSkill);
        effects = new List<Effect>();
        cubeName = (string)deserializedSkill["cubeName"];
        power = (int)deserializedSkill["power"];
        foreach (GameObject cube in Grid.instance.prefabsList)
        {
            if (cube.name == cubeName)
            {
                cubeToCreate = cube;
                effects.Add(new CreateTotemEffect(cubeToCreate, new Vector3Int(0, 1, 0)));
                return;
            }
        }
        Debug.LogError("Could not find the prefab named " + cubeName);
    }

    public override void Preview(NetIdeable target)
    {
        CreateEffect.face = Vector3Int.FloorToInt(GameManager.instance.RaycastSelector.CurrentHovered.face);

        if (CheckConditions(GameManager.instance.PlayingPlaceable, target))
        {
            CreateEffect.Preview((Placeable)target);
        }
    }

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
        vect = PatternCreate(vect);
        LivingPlaceable caster = (LivingPlaceable)Grid.instance.GetPlaceableFromVector(position);
        for (int i = vect.Count - 1; i >= 0; i--)
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
        CreateEffect.face = Vector3Int.FloorToInt(GameManager.instance.currentSelection.face);
        target.DispatchEffect(CreateEffect);
    }
}
