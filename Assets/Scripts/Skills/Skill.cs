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
    [SerializeField]
    protected string skillName;
    public string SkillName { get { return skillName; } }

    [SerializeField]
    protected int cost;
    public int Cost { get { return cost; } }

    [SerializeField]
    public int cooldownTurnLeft = 0;

    [SerializeField]
    protected int cooldown;
    public int Cooldown { get { return cooldown; } }

    [SerializeField]
    private string spritePath;

    // Mettre dans le JSON le path du sprite ?
    protected Sprite abilitySprite;
    public Sprite AbilitySprite { get { return abilitySprite; } }

    [SerializeField]
    protected string description = "Use the object";
    public string Description { get { return description; } }
    
    [SerializeField]
    private TargetType targetType;

    protected bool throughblocks = false;
    protected bool squareShaped = false;

    public int MaxRange { get => maxRange;}
    public int MinRange { get => minRange;}
    public bool SquareShaped { get => squareShaped;}
    public bool Throughblocks { get => throughblocks;}


    [SerializeField]
    private int maxRange;
    [SerializeField]
    private int minRange;

    protected List<Effect> effects;

    /// <summary>
    /// Should the skill be automaticaly lauched when we click on the icon.
    /// </summary>
    protected bool oneClickUse = false;


    // ####### OLD #########
    
    //TO REMOVE

    [SerializeField]
    private SkillArea skillArea;
    [SerializeField]
    private SkillEffect skillEffect;


    public Skill(string JSON)
    {
    }
    
    protected virtual void Init(Newtonsoft.Json.Linq.JToken jObject)
    {
        skillName = (string)jObject["skillName"];
        cost = (int)jObject["cost"];
        cooldown = (int)jObject["cooldown"];
        spritePath = (string)jObject["spritePath"];
        abilitySprite = Resources.Load<Sprite>("UI_Images/Abilities/" + spritePath);
        if (abilitySprite == null)
            Debug.LogError("Could find the image for: " + skillName + " at: " + "UI_Images/Abilities/" + spritePath);
        description = (string)jObject["description"];
        maxRange = (int)jObject["maxRange"];
        minRange = (int)jObject["minRange"];
        targetType = (TargetType)(int)jObject["targetType"];
    }


    protected abstract void UseSpecific(LivingPlaceable caster, NetIdeable target);

    public bool Use(LivingPlaceable caster, NetIdeable target)
    {
        if (!CheckConditions(caster, target))
        {
            Debug.LogError("Condition not verified to launch the skill: " + this.SkillName);
            return false;
        }

        UseSpecific(caster, target);

        GameManager.instance.ActiveSkill = null;
        this.cooldownTurnLeft = this.cooldown;
        return true;

    }


    protected abstract bool CheckSpecificConditions(LivingPlaceable caster, NetIdeable target);

    public virtual bool CheckConditions(LivingPlaceable caster, NetIdeable target)
    {
        // Check PA se fait plus haut
        if (cooldownTurnLeft > 0)
        {
            return false;
        }
        return CheckSpecificConditions(caster, target);
    }

    public abstract void Preview(NetIdeable target);

    public virtual void UnPreview(NetIdeable target)
    {
        foreach (Effect effect in effects)
        {
            effect.ResetPreview((Placeable) target);
        }
    }

    protected abstract List<Placeable> PatterVision(Vector3 position, List<Placeable> vect);

    //protected abstract List<Placeable> (Placeable target);
    

    // TODO : rework les ALREADYTARGETED
    public void Activate()
    {

        if (GameManager.instance.ActiveSkill == this)
        {
            GameManager.instance.PlayingPlaceable.Player.cameraScript.BackToMovement();
            return;
        }

        if (oneClickUse)
        {
            Vector3 Playerpos = GameManager.instance.PlayingPlaceable.GetPosition();
            GameManager.instance.PlayingPlaceable.Player.OnUseSkill(Player.SkillToNumber(GameManager.instance.PlayingPlaceable, this), GameManager.instance.PlayingPlaceable.netId, new int[0], 0);

        } else
        {
            GameManager.instance.State = States.UseSkill;
            GameManager.instance.ActiveSkill = this;
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
        if (targetType == TargetType.BLOCK || targetType == TargetType.AREA)
        {
            vect = Grid.instance.FindTargetableBlocks(playingPlaceable.GetPosition(),this, false);
            List<Placeable> placeables = new List<Placeable>();
            foreach(Vector3Int pos in vect)
            {
                placeables.Add(Grid.instance.GridMatrix[pos.x, pos.y, pos.z]);
            }
            placeables = PatterVision(playingPlaceable.GetPosition(), placeables);
            playingPlaceable.TargetArea = placeables;
            GameManager.instance.ResetAllBatches();
            GameManager.instance.RaycastSelector.layerMask = LayerMask.GetMask("Placeable");
            if (skillArea == SkillArea.LINE || skillArea == SkillArea.MIXEDAREA)
            {
                GameManager.instance.RaycastSelector.Pattern = skillArea;
            }
        }
        else if (targetType == TargetType.LIVING)
        {
            vect = Grid.instance.FindTargetableBlocks(playingPlaceable.GetPosition(), this, true);
            List<StandardCube> range = new List<StandardCube>();
            foreach (Vector3Int pos in vect)
            {
                if (Grid.instance.GridMatrix[pos.x, pos.y + 1, pos.z] == null || Grid.instance.GridMatrix[pos.x, pos.y + 1, pos.z].IsLiving())
                {
                    range.Add((StandardCube)Grid.instance.GridMatrix[pos.x, pos.y, pos.z]);
                }
            }
            playingPlaceable.Range = range;


            targetUnits = Grid.instance.FindTargetableLiving(playingPlaceable.GetPosition(), this);
            List<Placeable> placeables = new List<Placeable>();
            foreach (LivingPlaceable livingPlaceable in targetUnits)
            {
                placeables.Add(livingPlaceable);
            }
            placeables = PatterVision(playingPlaceable.GetPosition(), placeables);
            targetUnits.Clear();
            foreach (LivingPlaceable livingPlaceable in placeables)
            {
                targetUnits.Add(livingPlaceable);
            }
            playingPlaceable.TargetableUnits = targetUnits;
            GameManager.instance.RaycastSelector.layerMask = LayerMask.GetMask("LivingPlaceable");
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
        Debug.LogError("Need to redo this");
        //throw new System.NotImplementedException;
        return "SAVE SKILL NOT REWORKED";
        string text = JsonUtility.ToJson(this);
        text = "\n" + text + "\n";
        /*
        foreach (Effect eff in effects)
        {
            text+=eff.Save();
            
        }*/
        text = text.Remove(text.Length - 1) + ";";
        return text;
        //File.WriteAllText(path, text);
    }

    /// <summary>
    /// No pattern
    /// </summary>
    /// <param name="position"></param>
    /// <param name="vect"></param>
    /// <returns></returns>
    protected static List<Placeable> NoPattern(Vector3 position, List<Placeable> vect)
    {
        return vect;
    }

    /// <summary>
    /// spinning pattern
    /// </summary>
    /// <param name="position"></param>
    /// <param name="vect"></param>
    /// <returns></returns>
    protected static List<Placeable> PatternAround(Vector3 position, List<Placeable> vect)
    {
        List<Placeable> targets = new List<Placeable>(vect);
        foreach (Placeable Character in vect)
        {
            Vector3 Pos = Character.transform.position;
            if (Pos != position)
                targets.Remove(Character);
        }
        return targets;
    }

    /// <summary>
    /// SwordRange pattern (8 blocks around and up)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="vect"></param>
    /// <returns></returns>
    protected static List<Placeable> PatternSwordRange(Vector3 position, List<Placeable> vect)
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
    protected static List<Placeable> PatternPush(Vector3 position, List<Placeable> vect)
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
    protected static List<Placeable> PatternCreate(Vector3 position, List<Placeable> vect)
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
    protected static List<Placeable> PatternDestroy(Vector3 position, List<Placeable> vect)
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
    protected static List<Placeable> PatternLine(Vector3 position, List<Placeable> vect)
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
    protected static List<Placeable> PatternTopBlock(Vector3 position, List<Placeable> vect)
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
    /*
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
    */
    public static List<Placeable> PatternUseSphere(Placeable target, int sizezone)
    {
        List<Placeable> targetableBlocks = new List<Placeable>();
        Vector3 Position = target.GetPosition();
        int sizeX = Grid.instance.sizeX;
        int sizeY = Grid.instance.sizeY;
        int sizeZ = Grid.instance.sizeZ;
        
        for (int x = Mathf.Max((int)Position.x - sizezone, 0);
                x < Mathf.Min((int)Position.x + sizezone + 1, sizeX);
                x++)
        {
            for (int y = Mathf.Max((int)Position.y - sizezone, 0);
                y < Mathf.Min((int)Position.y + sizezone + 1, sizeY);
                y++)
            {
                for (int z = Mathf.Max((int)Position.z - sizezone, 0);
                z < Mathf.Min((int)Position.z + sizezone + 1, sizeZ);
                z++)
                {
                    if (!Grid.instance.CheckNull(new Vector3Int(x,y,z))
                        && !Grid.instance.GridMatrix[x, y, z].IsLiving() 
                        && Mathf.Abs(x - Position.x) + Mathf.Abs(y - Position.y) + Mathf.Abs(z - Position.z) < sizezone)
                    {
                        targetableBlocks.Add(Grid.instance.GridMatrix[x, y, z]);
                    }
                }
            }
        }

        return targetableBlocks;
    }

    public static List<Placeable> PatternUseAround(Placeable target)
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
    /*
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

    /// <summary>
    /// Patter wich return a line centered around a the target
    /// </summary>
    /// <param name="target">The center of the line</param>
    /// <param name="size">size=1: only the target, size=2: the target and the two adjacent cubes</param>
    /// <returns></returns>
    static protected List<Placeable> PatternUseLine(Placeable target, int size = 2)
    {
        List<Placeable> targets = new List<Placeable>();
        Vector3 Position = target.GetPosition();
        int state = GameManager.instance.orientationState % 2;
        Vector3Int direction = new Vector3Int(state, 0, 1 - state);

        Placeable placeableTemp = null;

        targets.Add(target);
        Debug.Log("Try to use line");
        for (int i = 1; i < size; i++)
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
}
