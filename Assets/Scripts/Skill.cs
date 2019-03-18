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
    private string description = "Use the object";
    private Sprite abilitySprite;
    [SerializeField]
    private SkillType skillType;
    [SerializeField]
    private SkillArea skillarea;
    [SerializeField]
    private SkillEffect skilleffect;
    [SerializeField]
    private int patternNumber;
    private delegate List<Placeable> Pattern(Vector3 position, List<Placeable> vect);
    private Pattern pattern;


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
        GameManager.instance.ActiveSkill = this;

        if (SkillType == SkillType.ALREADYTARGETED)
        {
            Vector3 Playerpos = GameManager.instance.PlayingPlaceable.GetPosition();
            if (SkillEffect == SkillEffect.UP)
            {
                if (Playerpos.y + 1 < Grid.instance.sizeY && Grid.instance.GridMatrix[(int)Playerpos.x, (int)Playerpos.y + 1, (int)Playerpos.z] == null)
                {
                    GameManager.instance.PlayingPlaceable.Player.OnUseSkill(Player.SkillToNumber(GameManager.instance.PlayingPlaceable, this), GameManager.instance.PlayingPlaceable.netId, new int[0], 0);
                }
            }
            else
            {
                GameManager.instance.PlayingPlaceable.Player.OnUseSkill(Player.SkillToNumber(GameManager.instance.PlayingPlaceable, this), GameManager.instance.PlayingPlaceable.netId, new int[0], 0);
            }
            return;
        }

        ShowSkillEffectTarget(GameManager.instance.PlayingPlaceable);
    }

    public void ShowSkillEffectTarget(LivingPlaceable playingPlaceable)
    {
        GameManager.instance.RaycastSelector.Pattern = SkillArea.NONE;
        GameManager.instance.RaycastSelector.EffectArea = 0;
        playingPlaceable.ResetHighlightSkill();
        playingPlaceable.ResetAreaOfMovement();
        playingPlaceable.ResetTargets();
        List<Vector3Int> vect = new List<Vector3Int>();
        List<LivingPlaceable> targetUnits = new List<LivingPlaceable>();
        if (skillType == SkillType.BLOCK || skillType == SkillType.AREA)
        {
            vect = Grid.instance.HighlightTargetableBlocks(playingPlaceable.GetPosition(), Minrange, Maxrange, SkillArea == SkillArea.THROUGHBLOCKS, Minrange > 0);
            List<Placeable> placeables = new List<Placeable>();
            foreach(Vector3Int pos in vect)
            {
                placeables.Add(Grid.instance.GridMatrix[pos.x, pos.y, pos.z]);
            }
            placeables = pattern(playingPlaceable.GetPosition(), placeables);

            playingPlaceable.TargetArea = placeables;
            playingPlaceable.ChangeMaterialAreaOfTarget(GameManager.instance.targetMaterial);
            GameManager.instance.RaycastSelector.layerMask = LayerMask.GetMask("Placeable");
            GameManager.instance.RaycastSelector.EffectArea = EffectArea;
            if (SkillArea == SkillArea.LINE || SkillArea == SkillArea.MIXEDAREA)
            {
                GameManager.instance.RaycastSelector.Pattern = SkillArea;
            }
        }
        else if (skillType == SkillType.LIVING)
        {
            targetUnits = Grid.instance.HighlightTargetableLiving(playingPlaceable.GetPosition(), Minrange, Maxrange, SkillArea == SkillArea.THROUGHBLOCKS, Minrange > 0);
            List<Placeable> placeables = new List<Placeable>();
            foreach (LivingPlaceable livingPlaceable in targetUnits)
            {
                placeables.Add(livingPlaceable);
            }
            placeables = pattern(playingPlaceable.GetPosition(), placeables);
            targetUnits.Clear();
            foreach (LivingPlaceable livingPlaceable in placeables)
            {
                targetUnits.Add(livingPlaceable);
            }
            playingPlaceable.TargetableUnits = targetUnits;
            GameManager.instance.RaycastSelector.layerMask = LayerMask.GetMask("LivingPlaceable");
        }
        else if (skillType == SkillType.PLACEABLE)
        {
            vect = Grid.instance.HighlightTargetableBlocks(playingPlaceable.GetPosition(), Minrange, Maxrange, SkillArea == SkillArea.THROUGHBLOCKS, Minrange > 0);
            targetUnits = Grid.instance.HighlightTargetableLiving(playingPlaceable.GetPosition(), Minrange, Maxrange, SkillArea == SkillArea.THROUGHBLOCKS, Minrange > 0);
            List<Placeable> placeables = new List<Placeable>();
            foreach (LivingPlaceable livingPlaceable in targetUnits)
            {
                placeables.Add(livingPlaceable);
            }
            foreach (Vector3Int pos in vect)
            {
                placeables.Add(Grid.instance.GridMatrix[pos.x, pos.y, pos.z]);
            }
            placeables = pattern(playingPlaceable.GetPosition(), placeables);
            Debug.LogError("not finished option : placeable");

        }
        else
        {
            Debug.LogError("Skill Type not handled");
        }
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
        GameManager.instance.PlayingPlaceable.CurrentPA -= this.Cost;
        this.tourCooldownLeft = this.cooldown;
        if (skill.SkillType == SkillType.ALREADYTARGETED) //Simply use them
        {
            foreach (Effect eff in skill.effects)
            {
                Effect effectToConsider = eff.Clone();
                effectToConsider.Launcher = GameManager.instance.PlayingPlaceable;
                effectToConsider.Use();
            }
        }
        return true;
    }

    // set SkillAnimationToPlay in AnimationHandler
    public void SendAnimationInfo(List<Animator> animTargets, List<Placeable> placeableTargets, List<Vector3> positionTargets)
    {
        AnimationHandler.Instance.SkillAnimationToPlay = this.skillName;
        AnimationHandler.Instance.animLauncher = GameManager.instance.PlayingPlaceable.GetComponent<Animator>();
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
        this.tourCooldownLeft = this.cooldown;
        Debug.Log(this.Cost);
        caster.CurrentPA -= this.Cost;
        Debug.Log(caster.CurrentPA);
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

        GameManager.instance.ActiveSkill = null;
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

    public void InitPattern()
    {
        switch (patternNumber)
        {
            case 0:
                pattern = new Pattern(Pattern0);
                break;
            case 1:
                pattern = new Pattern(Pattern1);
                break;
            case 2:
                pattern = new Pattern(Pattern2);
                break;
            case 3:
                pattern = new Pattern(Pattern3);
                break;
            case 4:
                pattern = new Pattern(Pattern4);
                break;
            case 5:
                pattern = new Pattern(Pattern5);
                break;
            case 6:
                pattern = new Pattern(Pattern6);
                break;
            case 7:
                pattern = new Pattern(Pattern7);
                break;
        }
    }

    /// <summary>
    /// No pattern
    /// </summary>
    /// <param name="position"></param>
    /// <param name="vect"></param>
    /// <returns></returns>
    private List<Placeable> Pattern0(Vector3 position, List<Placeable> vect)
    {
        return vect;
    }

    /// <summary>
    /// spinning pattern
    /// </summary>
    /// <param name="position"></param>
    /// <param name="vect"></param>
    /// <returns></returns>
    private List<Placeable> Pattern7(Vector3 position, List<Placeable> vect)
    {
        List<Placeable> targets = new List<Placeable>(vect);
        foreach (Placeable Character in vect)
        {
            Vector3 Pos = Character.transform.position;
            if (Pos.y - position.y != 0 || Math.Abs(Pos.x - position.x) > 1 || Math.Abs(Pos.z - position.z) > 1 || Pos == position)
                targets.Remove(Character);
        }
        return targets;
    }

    /// <summary>
    /// SwordRange pattern
    /// </summary>
    /// <param name="position"></param>
    /// <param name="vect"></param>
    /// <returns></returns>
    private List<Placeable> Pattern6(Vector3 position, List<Placeable> vect)
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

    /// <summary>
    /// Push pattern
    /// </summary>
    /// <param name="position"></param>
    /// <param name="vect"></param>
    /// <returns></returns>
    private List<Placeable> Pattern5(Vector3 position, List<Placeable> vect)
    {
        List<Placeable> targetableblock = new List<Placeable>(vect);
        foreach (Placeable placeable in vect)
        {
            if (!placeable.movable || placeable.GetPosition().y != position.y)
                targetableblock.Remove(placeable);
            else
            {
                if (Math.Abs((int)position.x - placeable.GetPosition().x) - Math.Abs((int)position.z - placeable.GetPosition().z) > 0)
                {
                    int direction = (placeable.GetPosition().x - (int)position.x) / Math.Abs((int)position.x - placeable.GetPosition().x);
                    if (placeable.GetPosition().x + direction < 0 || placeable.GetPosition().x + direction >= Grid.instance.sizeX || 
                        Grid.instance.GridMatrix[placeable.GetPosition().x + direction, placeable.GetPosition().y, placeable.GetPosition().z] != null)
                        targetableblock.Remove(placeable);
                }
                else if (Math.Abs((int)position.x - placeable.GetPosition().x) - Math.Abs((int)position.z - placeable.GetPosition().z) < 0)
                {
                    int direction = (placeable.GetPosition().z - (int)position.z) / Math.Abs((int)position.z - placeable.GetPosition().z);
                    if (placeable.GetPosition().z + direction < 0 || placeable.GetPosition().z + direction >= Grid.instance.sizeZ ||
                        Grid.instance.GridMatrix[placeable.GetPosition().x, placeable.GetPosition().y, placeable.GetPosition().z + direction] != null)
                        targetableblock.Remove(placeable);
                }
                else
                {
                    targetableblock.Remove(placeable);
                }
            }

        }
        return targetableblock;
    }

    /// <summary>
    /// Create pattern (top block but doesn't allow living/under living
    /// and spawn)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="vect"></param>
    /// <returns></returns>
    private List<Placeable> Pattern4(Vector3 position, List<Placeable> vect)
    {
        List<Placeable> targetableblock = new List<Placeable>(vect);
        foreach (Placeable plac in vect)
        {
            if ((!plac.IsLiving() && (((StandardCube)plac).GetType() == typeof(Goal) || ((StandardCube)plac).isSpawnPoint))
                || plac.GetPosition().y == Grid.instance.sizeY - 1
                || Grid.instance.GridMatrix[plac.GetPosition().x, plac.GetPosition().y + 1, plac.GetPosition().z] != null)
                targetableblock.Remove(plac);
        }
        return targetableblock;
    }

    /// <summary>
    /// Destroy pattern
    /// </summary>
    /// <param name="position"></param>
    /// <param name="vect"></param>
    /// <returns></returns>
    private List<Placeable> Pattern3(Vector3 position, List<Placeable> vect)
    {
        List<Placeable> targetableblock = new List<Placeable>(vect);
        foreach (Placeable plac in vect)
        {
            if (!plac.IsLiving() && !((StandardCube)plac).Destroyable)
                targetableblock.Remove(plac);
        }
        return targetableblock;
    }

    /// <summary>
    /// Staight Line skill Pattern
    /// </summary>
    /// <param name="position"></param>
    /// <param name="vect"></param>
    /// <returns></returns>
    private List<Placeable> Pattern2(Vector3 position, List<Placeable> vect)
    {
        List<Placeable> targetableblock = new List<Placeable>(vect);
        foreach (Placeable plac in vect)
        {
            if (plac.GetPosition().y - position.y != 0 || (plac.GetPosition().x - position.x != 0 && plac.GetPosition().z - position.z != 0))
                targetableblock.Remove(plac);
        }
        return targetableblock;
    }

    /// <summary>
    /// Top block pattern
    /// </summary>
    /// <param name="position"></param>
    /// <param name="vect"></param>
    /// <returns></returns>
    public List<Placeable> Pattern1(Vector3 position, List<Placeable> vect)
    {
        List<Placeable> targetableblock = new List<Placeable>(vect);
        foreach (Placeable plac in vect)
        {
            if (plac.GetPosition().y != Grid.instance.sizeY - 1 && 
                (Grid.instance.GridMatrix[plac.GetPosition().x, plac.GetPosition().y + 1, plac.GetPosition().z] != null &&
                !Grid.instance.GridMatrix[plac.GetPosition().x, plac.GetPosition().y + 1, plac.GetPosition().z].IsLiving()))
                targetableblock.Remove(plac);
        }
        return targetableblock;
    }
}
