using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// blocs have simple effects. When they are used, they are add to the list of blocked active effect of GameEffectsManager
/// <summary>
/// Class for effect blocking other effects
/// </summary>
public class BlockEffects : EffectOnPlaceable
{
    [SerializeField]
    public List<Effect> listEffectsToBlock;
    /// <summary>
    /// -1 = undependent
    /// </summary>
    [SerializeField]
    public int numberToBlock;


    public BlockEffects(Placeable target, Placeable launcher) : base(target, launcher)
    {
            }

    public BlockEffects(BlockEffects other) : base(other)
    {
        this.listEffectsToBlock = other.listEffectsToBlock.ConvertAll(effect => effect.Clone());//deep copy the list
        this.numberToBlock = other.numberToBlock;
        //this.listEffectsToBlock(other.listEffectsToBlock);
        
    }

    public override void preview()
    {
        throw new System.NotImplementedException();
    }

    public List<Effect> ListEffectsToBlock
    {
        get
        {
            return listEffectsToBlock;
        }

        set
        {
            listEffectsToBlock = value;
        }
    }

    public bool ContainsEffectType(Effect effect) // check effects list contains instance of type object
    {
        foreach (Effect iteEffect in listEffectsToBlock)
        {
            if (effect.GetType() == iteEffect.GetType())
            {
                return true;
            }
        }
        return false;
    }


    public int NumberToBlock
    {
        get
        {
            return numberToBlock;
        }

        set
        {
            numberToBlock = value;
        }
    }

    override
    public void Use()
    {
        
    }

    public override Effect Clone()
    {
        throw new System.NotImplementedException();
    }
}
