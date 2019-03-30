using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

class Wall : Skill
{
    [SerializeField]
    private GameObject cubeToCreate;

    [SerializeField]
    private int sizeZone;

    CreateBlock CreateBlockEffect { get { return (CreateBlock) effects[0]; } }

    public Wall(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a wall skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        Init(deserializedSkill["Wall"]);
    }

    protected override void Init(JToken deserializedSkill)
    {
        base.Init(deserializedSkill);

        sizeZone = (int)deserializedSkill["sizeZone"];
        string cubeName = (string)deserializedSkill["cubeName"];
        

        effects = new List<Effect>();

        foreach (GameObject cube in Grid.instance.prefabsList)
        {
            if (cube.name == cubeName)
            {
                cubeToCreate = cube;
                effects.Add(new CreateBlock(cubeToCreate, new Vector3Int(0, 1, 0), 2));
                return;
            }
        }
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        return true;
    }


    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        CreateBlockEffect.Launcher = caster;

        List<Placeable> affectedPlaceable = Skill.PatternUseLine((Placeable)target, sizeZone);


        foreach (Placeable placeable in affectedPlaceable)
        {
            StandardCube cube = placeable as StandardCube;
            if (cube)
            {
                cube.DispatchEffect(CreateBlockEffect);
            }
        }
        
        target.DispatchEffect(CreateBlockEffect);
    }

    public override void Preview(NetIdeable target)
    {
        List<Placeable> affectedPlaceable = Skill.PatternUseLine((Placeable)target, sizeZone);


        foreach (Placeable placeable in affectedPlaceable)
        {
            StandardCube cube = placeable as StandardCube;
            if (cube)
            {
                CreateBlockEffect.Preview(cube);
            }
        }
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return PatternCreate(position, vect);
    }
}

