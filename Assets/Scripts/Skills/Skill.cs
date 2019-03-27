using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
/// <summary>
/// Class representing a skill usable by player through a character of his team
/// </summary>
[Serializable]
public abstract class Skill
{
    // ####### NEW #########
    private string typeName;

    [SerializeField]
    private string skillName;
    public string SkillName { get { return skillName; } }

    [SerializeField]
    private int cost;
    public int Cost { get { return cost; } }

    [SerializeField]
    public int cooldownTurnLeft;

    [SerializeField]
    private int cooldown;
    public int Cooldown { get { return cooldown; } }

    // Mettre dans le JSON le path du sprite ?
    private Sprite abilitySprite;
    public Sprite AbilitySprite { get { return AbilitySprite; } }

    [SerializeField]
    private string description = "Use the object";
    public string Description { get { return Description; } }

    // ####### OLD #########


    [SerializeField]
    private int maxRange;
    [SerializeField]
    private int minRange;
    [SerializeField]
    private int sizeZone = 1;

    [SerializeField]
    public List<Effect> effects;
    
    [SerializeField]
    private TargetType skillType;
    [SerializeField]
    private SkillArea skillArea;
    [SerializeField]
    private SkillEffect skillEffect;
    [SerializeField]
    private int patternNumber;
    private delegate List<Placeable> PatternVision(Vector3 position, List<Placeable> vect);
    private PatternVision patternVision;

    [SerializeField]
    private PatternUseType patternUseType;
    public delegate List<Placeable> PatternUse(Placeable target);
    public PatternUse patternUse;

    [SerializeField]
    //private ConditionType conditionType;
    public delegate bool DelegateCondition();
    public DelegateCondition condition;

    public Skill(dynamic skills)
    {
        /*
        Cost = cost;
        Cooldown = cooldown;
        tourCooldownLeft = 0;
        this.effects = effects;
        SkillName = skillName;
        this.AbilitySprite = Resources.Load<Sprite>("UI_Images/Abilities/" + SkillName);
        TargetType = skillType;
        this.maxRange = rangeMax;
        this.minRange = rangeMin;
        EffectArea = effectarea;
        SkillArea = skillarea;
        SkillEffect = skilleffect;
        this.patternUseType = patternUse;
        InitPattern();
        InitPatternUse();
        */
    }
    
    protected virtual void Init()
    {
        // Lire le JSON et initialiser les viariables communes
    }

    // TODO : rework les ALREADYTARGETED
    public void Activate()
    {

        if (GameManager.instance.ActiveSkill == this)
        {
            GameManager.instance.PlayingPlaceable.Player.cameraScript.BackToMovement();
            return;
        }
        GameManager.instance.State = States.UseSkill;
        GameManager.instance.ActiveSkill = this;

        if (skillType == TargetType.ALREADYTARGETED)
        {
            Vector3 Playerpos = GameManager.instance.PlayingPlaceable.GetPosition();

            
            if (patternUse(GameManager.instance.PlayingPlaceable) != null)
            {
                GameManager.instance.PlayingPlaceable.Player.OnUseSkill(Player.SkillToNumber(GameManager.instance.PlayingPlaceable, this), GameManager.instance.PlayingPlaceable.netId, new int[0], 0);
            } else
            {
                return;
            }
            
        } else
        {
            ShowSkillEffectTarget(GameManager.instance.PlayingPlaceable);
        }
        
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
        if (skillType == TargetType.BLOCK || skillType == TargetType.AREA)
        {
            vect = Grid.instance.HighlightTargetableBlocks(playingPlaceable.GetPosition(), minRange, maxRange, skillArea == SkillArea.THROUGHBLOCKS, minRange > 0, false, new Vector3(0,0,0));
            List<Placeable> placeables = new List<Placeable>();
            foreach(Vector3Int pos in vect)
            {
                placeables.Add(Grid.instance.GridMatrix[pos.x, pos.y, pos.z]);
            }
            placeables = patternVision(playingPlaceable.GetPosition(), placeables);
            playingPlaceable.TargetArea = placeables;
            GameManager.instance.ResetAllBatches();
            GameManager.instance.RaycastSelector.layerMask = LayerMask.GetMask("Placeable");
            GameManager.instance.RaycastSelector.EffectArea = sizeZone;
            if (skillArea == SkillArea.LINE || skillArea == SkillArea.MIXEDAREA)
            {
                GameManager.instance.RaycastSelector.Pattern = skillArea;
            }
        }
        else if (skillType == TargetType.LIVING)
        {
            
            vect = Grid.instance.HighlightTargetableBlocks(playingPlaceable.GetPosition(), minRange, maxRange, skillArea == SkillArea.THROUGHBLOCKS, minRange > 0, true, new Vector3(0, 1, 0));
            List<StandardCube> range = new List<StandardCube>();
            foreach (Vector3Int pos in vect)
            {
                if (Grid.instance.GridMatrix[pos.x, pos.y + 1, pos.z] == null || Grid.instance.GridMatrix[pos.x, pos.y + 1, pos.z].IsLiving())
                {
                    range.Add((StandardCube)Grid.instance.GridMatrix[pos.x, pos.y, pos.z]);
                }
            }
            playingPlaceable.Range = range;


            targetUnits = Grid.instance.HighlightTargetableLiving(playingPlaceable.GetPosition(), minRange, maxRange, skillArea == SkillArea.THROUGHBLOCKS, minRange > 0);
            List<Placeable> placeables = new List<Placeable>();
            foreach (LivingPlaceable livingPlaceable in targetUnits)
            {
                placeables.Add(livingPlaceable);
            }
            placeables = patternVision(playingPlaceable.GetPosition(), placeables);
            targetUnits.Clear();
            foreach (LivingPlaceable livingPlaceable in placeables)
            {
                targetUnits.Add(livingPlaceable);
            }
            playingPlaceable.TargetableUnits = targetUnits;
            GameManager.instance.RaycastSelector.layerMask = LayerMask.GetMask("LivingPlaceable");
        }
        else if (skillType == TargetType.PLACEABLE)
        {
            vect = Grid.instance.HighlightTargetableBlocks(playingPlaceable.GetPosition(), minRange, maxRange, skillArea == SkillArea.THROUGHBLOCKS, minRange > 0, false, new Vector3(0, 0, 0));
            targetUnits = Grid.instance.HighlightTargetableLiving(playingPlaceable.GetPosition(), minRange, maxRange, skillArea == SkillArea.THROUGHBLOCKS, minRange > 0);
            List<Placeable> placeables = new List<Placeable>();
            foreach (LivingPlaceable livingPlaceable in targetUnits)
            {
                placeables.Add(livingPlaceable);
            }
            foreach (Vector3Int pos in vect)
            {
                placeables.Add(Grid.instance.GridMatrix[pos.x, pos.y, pos.z]);
            }
            placeables = patternVision(playingPlaceable.GetPosition(), placeables);
            Debug.LogError("not finished option : placeable");

        }
        else
        {
            Debug.LogError("Skill Type not handled");
        }
    }

    /*
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
        GameManager.instance.PlayingPlaceable.CurrentPA -= this.cost;
        this.tourCooldownLeft = this.cooldown;
        if (skill.skillType == TargetType.ALREADYTARGETED) //Simply use them
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
    */

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

    protected virtual bool Condition(/**/)
    {
        // Check PA
        Debug.LogError("PAS DE CONDITION IMPLEMENTER");
        return false;
    }

    public abstract bool Use(LivingPlaceable caster, List<NetIdeable> targets);
    /*
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
    }*/

    //TODO: rework
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
/*
    public void InitPattern()
    {
        switch (patternNumber)
        {
            case 0:
                patternVision = new PatternVision(Pattern0);
                break;
            case 1:
                patternVision = new PatternVision(Pattern1);
                break;
            case 2:
                patternVision = new PatternVision(Pattern2);
                break;
            case 3:
                patternVision = new PatternVision(Pattern3);
                break;
            case 4:
                patternVision = new PatternVision(Pattern4);
                break;
            case 5:
                patternVision = new PatternVision(Pattern5);
                break;
            case 6:
                patternVision = new PatternVision(Pattern6);
                break;
            case 7:
                patternVision = new PatternVision(Pattern7);
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

    public void InitPatternUse()
    {
        switch (patternUseType)
        {
            case PatternUseType.NONE:
                patternUse = new PatternUse(PatternUseNone);
                break;

            case PatternUseType.LINE:
                patternUse = new PatternUse(PatternUseLine);
                break;

            case PatternUseType.AROUNDTARGET:
                patternUse = new PatternUse(PatternUseAround);
                break;

            case PatternUseType.CHECKUP:
                patternUse = new PatternUse(PatternUseCheckUp);
                break;
            default:
                patternUse = new PatternUse(PatternUseNone);
                break;
        }
    }

    public List<Placeable> PatternUseNone(Placeable target)
    {
        List<Placeable> targetableBlocks = new List<Placeable>();
        Vector3 Position = target.GetPosition();
        int sizeX = Grid.instance.sizeX;
        int sizeY = Grid.instance.sizeY;
        int sizeZ = Grid.instance.sizeZ;
        
        for (int x = Mathf.Max((int)Position.x - effectarea, 0);
                x < Mathf.Min((int)Position.x + effectarea + 1, sizeX);
                x++)
        {
            for (int y = Mathf.Max((int)Position.y - effectarea, 0);
                y < Mathf.Min((int)Position.y + effectarea + 1, sizeY);
                y++)
            {
                for (int z = Mathf.Max((int)Position.z - effectarea, 0);
                z < Mathf.Min((int)Position.z + effectarea + 1, sizeZ);
                z++)
                {
                    if (!Grid.instance.CheckNull(new Vector3Int(x,y,z))
                        && !Grid.instance.GridMatrix[x, y, z].IsLiving() 
                        && Mathf.Abs(x - Position.x) + Mathf.Abs(y - Position.y) + Mathf.Abs(z - Position.z) < effectarea)
                    {
                        targetableBlocks.Add(Grid.instance.GridMatrix[x, y, z]);
                    }
                }
            }
        }

        return targetableBlocks;
    }

    public List<Placeable> PatternUseLine(Placeable target)
    {
        List<Placeable> targets = new List<Placeable>();
        Vector3 Position = target.GetPosition();
        int state = GameManager.instance.RaycastSelector.State % 2;
        Vector3Int direction = new Vector3Int(state, 0, 1 - state);

        Placeable placeableTemp = null;

        targets.Add(target);
        Debug.Log("Try to use line");
        for (int i=1; i < effectarea; i++)
        {
            placeableTemp = Grid.instance.GetPlaceableFromVector(target.GetPosition() + direction * i);
            if (placeableTemp)
                targets.Add(placeableTemp);
            placeableTemp = Grid.instance.GetPlaceableFromVector(target.GetPosition() - direction * i);
            if (placeableTemp)
                targets.Add(placeableTemp);
        }

        return targets;
    }

    public List<Placeable> PatternUseAround(Placeable target)
    {
        List<Placeable> targets = new List<Placeable>();
        Vector3 position = target.GetPosition();

        Placeable current = null;
        for (int x = -1; x < 2; x++)
        {
            for (int z = -1; z < 2; z++)
            {
                current = Grid.instance.GetPlaceableFromVector(position + new Vector3(x, 0, z));

                if (current != null && !(x == 0 && z == 0))
                {
                    targets.Add(current);
                }

            }
        }

        return targets;
    }

    public List<Placeable> PatternUseCheckUp(Placeable target)
    {
        if (Grid.instance.GetPlaceableFromVector(target.GetPosition() + Vector3Int.up) == null
             && target.GetPosition().y < Grid.instance.sizeY - 1)
        {
            return new List<Placeable>() { target };
        }
        else
        {
            return null;
        }
    }
    */

}
