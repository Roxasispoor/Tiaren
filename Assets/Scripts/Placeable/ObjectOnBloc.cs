using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class Containing that may contain a pickable object
/// </summary>
public class ObjectOnBloc : NetIdeable {
    public bool isPickable;
    public bool isPicked;
    public override void DispatchEffect(Effect effect)
    {
      
        effect.TargetAndInvokeEffectManager(this);
    }
    //Does nothing if you try to pick An object on block that isn't blockable
    public virtual void GetPicked(Placeable Luncher)
    {
    }

    private List<Effect> onWalk;


    private List<Skill> givenSkills;
    public List<Effect> OnWalk
    {
        get
        {
            return onWalk;
        }

        set
        {
            onWalk = value;
        }
    }

    public List<Skill> GivenSkills
    {
        get
        {
            return givenSkills;
        }

        set
        {
            givenSkills = value;
        }
    }
    private void Awake()
    {
        givenSkills = new List<Skill>();
        if(isPickable)
        {
            PickObjectOnBlockEffect eff = new PickObjectOnBlockEffect(this);
            Skill taker = new Skill(0, 0, new List<Effect>() { eff }, SkillType.ALREADYTARGETED, "Pick Object", 0, 1);//No cooldown because skills from these objects aren't resetted
            givenSkills.Add(taker);
        }
    }
}
