using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

class HighGround : Skill
{
    [SerializeField]
    private GameObject cubeToCreate;

    [SerializeField]
    private int height;

    CreateBlock CreateBlockEffect { get { return (CreateBlock)effects[0]; } }

    MoveEffect MoveEffect { get { return (MoveEffect)effects[1]; } }

    public HighGround(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a HighGround skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        Init(deserializedSkill["HighGround"]);
        oneClickUse = true;
    }

    protected override void Init(JToken deserializedSkill)
    {
        base.Init(deserializedSkill);

        height = (int)deserializedSkill["height"];
        string cubeName = (string)deserializedSkill["cubeName"];


        effects = new List<Effect>();

        foreach (GameObject cube in Grid.instance.prefabsList)
        {
            if (cube.name == cubeName)
            {
                cubeToCreate = cube;
                effects.Add(new CreateBlock(cubeToCreate, new Vector3Int(0, 1, 0), 1));
                break;
            }
        }

        effects.Add(new MoveEffect());
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        StandardCube blocUnder = Grid.instance.GetPlaceableFromVector(caster.GetPosition() + Vector3.down) as StandardCube;
        Placeable placeableAbove = Grid.instance.GetPlaceableFromVector(caster.GetPosition() + Vector3.up);

        if (blocUnder == null || !blocUnder.isConstructableOn)
        {
            Debug.LogError("Trying to " + skillName + " but the bloc under does not exist or is not constructable on");
            return false;
        }
        if (placeableAbove != null)
        {
            return false;
        }
        return true;
    }


    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        CreateBlockEffect.Launcher = caster;

        MoveEffect.Launcher = caster;
        MoveEffect.Target = caster;

        Vector3Int positionPlayerStart = target.GetPosition();
        int freeSpaceAbove = 0;

        for (int i = 1; i < height+1; i++)
        {
            if (Grid.instance.GetPlaceableFromVector(positionPlayerStart + Vector3Int.up * i) != null)
            {
                break;
            }
            freeSpaceAbove = i;
        }

        if (freeSpaceAbove > 0)
        {
            MoveEffect.Direction = Vector3Int.up * freeSpaceAbove;
        }

        caster.DispatchEffect(MoveEffect);

        CreateBlockEffect.face = Vector3Int.down;
        CreateBlockEffect.height = freeSpaceAbove;

        caster.DispatchEffect(CreateBlockEffect);

    }

    public override void Preview(NetIdeable target)
    {
        Debug.LogError(SkillName + ": no preview");
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return new List<Placeable>() { Grid.instance.GetPlaceableFromVector(position) };
    }
}

