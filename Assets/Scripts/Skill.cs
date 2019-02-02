using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// Class representing a skill usable by player through a character of his team
/// </summary>
[Serializable]
public class Skill
{
    [SerializeField]
    private string skillName;
    [SerializeField]
    private int cost;
    [SerializeField]
    private int tourCooldownLeft;
    [SerializeField]
    private int cooldown;
    [SerializeField]
    private int maxRange;
    [SerializeField]
    private int minRange;
    [SerializeField]
    private int effectarea = 0;
    [SerializeField]
    public List<Effect> effects;
    [SerializeField]
    public delegate bool DelegateCondition();
    [SerializeField]
    public DelegateCondition condition;
    [SerializeField]
    private string description = "Ca tue";
    private Sprite abilitySprite;
    [SerializeField]
    private SkillType skillType;
    [SerializeField]
    private SkillArea skillarea;
    [SerializeField]
    private SkillEffect skilleffect;


    public SkillType SkillType
    {
        get
        {
            return skillType;
        }

        set
        {
            skillType = value;
        }
    }

    public SkillArea SkillArea
    {
        get
        {
            return skillarea;
        }

        set
        {
            skillarea = value;
        }
    }

    public SkillEffect SkillEffect
    {
        get
        {
            return skilleffect;
        }

        set
        {
            skilleffect = value;
        }
    }

    public int TourCooldownLeft
    {
        get
        {
            return tourCooldownLeft;
        }

        set
        {
            tourCooldownLeft = value;
        }
    }

    public int Cooldown
    {
        get
        {
            return cooldown;
        }

        set
        {
            cooldown = value;
        }
    }

    public int EffectArea
    {
        get
        {
            return effectarea;
        }

        set
        {
            effectarea = value;
        }
    }

    public int Maxrange
    {
        get
        {
            return maxRange;
        }

        set
        {
            maxRange = value;
        }
    }

    public int Minrange
    {
        get
        {
            return minRange;
        }

        set
        {
            minRange = value;
        }
    }

    public int Cost
    {
        get
        {
            return cost;
        }

        set
        {
            cost = value;
        }
    }

    public string SkillName
    {
        get
        {
            return skillName;
        }

        set
        {
            skillName = value;
        }
    }

    public Sprite AbilitySprite
    {
        get
        {
            return abilitySprite;
        }

        set
        {
            abilitySprite = value;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }

        set
        {
            description = value;
        }
    }

    public Skill(int cost, int cooldown, List<Effect> effects, SkillType skillType, string skillName, int rangeMin,int rangeMax, SkillEffect skilleffect = SkillEffect.NONE, SkillArea skillarea = SkillArea.NONE, int effectarea = 0)
    {
        Cost = cost;
        Cooldown = cooldown;
        tourCooldownLeft = 0;
        this.effects = effects;
        SkillName = skillName;
        this.AbilitySprite = Resources.Load<Sprite>("UI_Images/Abilities/" + SkillName);
        SkillType = skillType;
        this.maxRange = rangeMax;
        this.minRange = rangeMin;
        EffectArea = effectarea;
        SkillArea = skillarea;
        SkillEffect = skilleffect;
    }

    public Skill(string jsonFilePath)
    {

    }

    public void Activate()
    {
        GameManager.instance.State = States.UseSkill;
        GameManager.instance.activeSkill = this;
        GameManager.instance.playingPlaceable.Player.ShowSkillEffectTarget(GameManager.instance.playingPlaceable, this);
    }
    public bool UseTargeted(Skill skill)
    {
        if (this.tourCooldownLeft > 0)
        {
            return false;
        }
        if (condition != null && !condition.Invoke())
        {
            return false;
        }
        this.tourCooldownLeft = this.cooldown;
        if (skill.SkillType == SkillType.ALREADYTARGETED) //Simply use them
        {
            foreach (Effect eff in skill.effects)
            {
                Effect effectToConsider = eff.Clone();
                effectToConsider.Launcher = GameManager.instance.playingPlaceable;
                effectToConsider.Use();
            }
        }
        return true;
    }

    // set SkillAnimationToPlay in AnimationHandler
    public void SendAnimationInfo(List<Animator> animTargets, List<Placeable> placeableTargets, List<Vector3> positionTargets)
    {
        AnimationHandler.Instance.SkillAnimationToPlay = this.skillName;
        AnimationHandler.Instance.animLauncher = GameManager.instance.playingPlaceable.GetComponent<Animator>();
        AnimationHandler.Instance.animTargets = animTargets;
        AnimationHandler.Instance.placeableTargets = placeableTargets;
        AnimationHandler.Instance.positionTargets = positionTargets;
    }

    // ask AnimationHandler to play it
    public void PlayAnimation()
    {
        AnimationHandler.Instance.PlayAnimation();
    }

    ///TODO makes the copy and return if succeeded launching the skill
    public bool Use(LivingPlaceable caster, List<NetIdeable> targets)
    {
        if (this.tourCooldownLeft > 0)
        {
            return false;
        }
        if (condition != null && !condition.Invoke())
        {
            return false;
        }
        
        List<Animator> animTargets = new List<Animator>();
        List<Placeable> placeableTargets = new List<Placeable>();
        List<Vector3> positionTargets = new List<Vector3>();
        foreach (Placeable target in targets)
        {
            placeableTargets.Add(target);
            positionTargets.Add(target.GetPosition());
            if (target.GetComponent<Animator>() != null)
            {
                animTargets.Add(target.GetComponent<Animator>());
            }
        }
        SendAnimationInfo(animTargets, placeableTargets, positionTargets);
        PlayAnimation();
        this.tourCooldownLeft = this.cooldown;//On pourrait avoir de la cdr dans les effets afterall
        foreach (Placeable target in targets)
        { 
            foreach (Effect effect in effects)
            {
                //makes the deep copy, send it to effect manager and zoo
                Effect effectToConsider = effect.Clone();
                effectToConsider.Launcher = caster;
                //Double dispatch
                target.DispatchEffect(effectToConsider);

            }
        }
        GameManager.instance.activeSkill = null;
        return true;
    }
    public string Save()
    {
        string text = JsonUtility.ToJson(this);
        text = "\n" + text + "\n";
        foreach (Effect eff in effects)
        {
            text+=eff.Save();
            
        }
        text = text.Remove(text.Length - 1) + ";";
        return text;
        //File.WriteAllText(path, text);
    }

    

}
