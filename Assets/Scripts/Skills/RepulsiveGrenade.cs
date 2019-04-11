using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class RepulsiveGrenade : Skill
{
    [SerializeField]
    private int nbCasePush;
    [SerializeField]
    private int damage;
    [SerializeField]
    private int damagePush;


    Damage DamageEffect { get { return (Damage)effects[0]; } }
    Push PushEffect { get { return (Push)effects[1]; } }


    public RepulsiveGrenade(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a EarthBending skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        base.Init(deserializedSkill["RepulsiveGrenade"]);
        InitSpecific(deserializedSkill["RepulsiveGrenade"]);
    }

    protected void InitSpecific(JToken deserializedSkill)
    {
        damage = (int)deserializedSkill["damage"];
        nbCasePush = (int)deserializedSkill["nbCasePush"];
        damagePush = (int)deserializedSkill["damagePush"];
        effects = new List<Effect>();
        effects.Add(new Damage(damage));
        effects.Add(new Push(nbCasePush, damage));
        PushEffect.pushSpeed = 3f;
    }

    protected override void UseSpecific(LivingPlaceable caster, NetIdeable target)
    {
        DamageEffect.Launcher = caster;
        PushEffect.Launcher = caster;

        Vector3Int positionAboveTarget = target.GetPosition() + Vector3Int.up;

        PushInDirection(positionAboveTarget, Vector3.forward);
        PushInDirection(positionAboveTarget, Vector3.right);
        PushInDirection(positionAboveTarget, Vector3.back);
        PushInDirection(positionAboveTarget, Vector3.left);
    }

    protected void PushInDirection(Vector3 center, Vector3 direction)
    {
        LivingPlaceable possibleTarget = Grid.instance.GetPlaceableFromVector(center + direction) as LivingPlaceable;
        if (possibleTarget != null)
        {
            PushEffect.direction = direction;
            possibleTarget.DispatchEffect(PushEffect);
        }
    }

    public override void Preview(NetIdeable target)
    {
        Debug.LogError(SkillName + ": preview not implemented");
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        return PatternTopBlock(position, vect);
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        StandardCube above = Grid.instance.GetPlaceableFromVector(target.GetPosition() + Vector3Int.up) as StandardCube;
        if (null == above)
        {
            return false;
        }
        return true;
    }
}
