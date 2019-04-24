using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTotem : Skill
{

    DestroyBloc DestroyEffect { get { return (DestroyBloc)effects[0]; } }

    public DestroyTotem(string JSON) : base(JSON)
    {
        Debug.LogError("Creating a destroyTotem skill");
        JObject deserializedSkill = JObject.Parse(JSON);
        Init(deserializedSkill["DestroyTotem"]);
    }

    protected override void Init(JToken deserializedSkll)
    {
        base.Init(deserializedSkll);
        effects = new List<Effect>();
        effects.Add(new DestroyBloc(affectTotem : true));
    }

    public override void Preview(NetIdeable target)
    {
        if (CheckConditions(GameManager.instance.PlayingPlaceable, target))
        {
            DestroyEffect.Preview((Placeable)target);
        }
    }

    protected override bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target)
    {
        if(caster.LinkedObjects.Contains(target))
        {
            return true;
        }
        Debug.LogError("non totem can't be destroyed this way");
        return false;
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
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
        DestroyEffect.Launcher = caster;
        target.DispatchEffect(DestroyEffect);
    }
}
