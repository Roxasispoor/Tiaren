using System;
using System.Collections.Generic;
using UnityEngine;

class SwordAttack: Skill
{
    [SerializeField]
    private int power;

    protected override void Init()
    {
        // Lire le JSON
        base.Init();
    }


    public override bool Use(LivingPlaceable caster, NetIdeable target)
    {
        
        if (target is LivingPlaceable)
        {
            target
        }
    }

    protected override List<Placeable> PatterVision(Vector3 position, List<Placeable> vect)
    {
        List<Placeable> targetableunits = new List<Placeable>(vect);
        foreach (LivingPlaceable Character in vect)
        {
            Vector3 Pos = Character.transform.position;
            if (Math.Abs(Pos.y - position.y) > 1 || Math.Abs(Pos.x - position.x) > 1 || Math.Abs(Pos.z - position.z) > 1)
                targetableunits.Remove(Character);
        }
        return targetableunits;
    }
}

