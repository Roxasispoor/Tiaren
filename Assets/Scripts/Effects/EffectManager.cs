using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    private void Awake()
    {
        if (instance == null)
        {

            //if not, set instance to this
            instance = this;
        }

        //If instance already exists and it's not this:
        else if (instance != this)
        {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// Launched when attack is used
    /// </summary>
    /// <param name="effect"></param>
    public void DirectAttack(Effect effect)
    {
        NetIdeable target = effect.GetTarget();
        bool isBlocked = CalculateEffectBlocked(effect);
        if (!isBlocked)
        {
            if (effect.activationType == ActivationType.INSTANT)
            {
                UseEffect(effect);
            }
            else
            {
                AttachEffect(effect);//if it is a dot only, add it to attached
            }
        }
    }
    public void StartTurnUseEffect(Effect effect)
    {

        NetIdeable target = effect.GetTarget();
        bool isBlocked = CalculateEffectBlocked(effect);
        if (!isBlocked)
        {
            if(!effect.TriggerOnce || effect.TriggerOnce && effect.turnActiveEffect==1)
            {
                UseEffect(effect);
            }
            else
            { effect.turnActiveEffect--;
            }
         
        }
        else//deletes the DOT
        {
            //effect.GetTarget().AttachedEffects.RemoveAll((x) => x.GetType() == typeof(Effect));
            effect.GetTarget().AttachedEffects.Remove(effect);
        }
    }

    // TODO: rework this function to take more precise element as condition (ex: only POISON)
    public bool CalculateEffectBlocked(Effect effect)
    {
        return false;
        /*
        NetIdeable target = effect.GetTarget();
        bool isblocked = false;
        foreach (Effect eff in target.AttachedEffects)
        {
            if (eff.GetType() == typeof(BlockEffects))
            {
                BlockEffects block = (BlockEffects)eff;
                if (block.listEffectsToBlock.Exists((x) => x.GetType() == effect.GetType()) && block.numberToBlock > 0)
                {
                    block.numberToBlock--;
                    isblocked = true;
                }

            }

        }
        //effect.GetTarget().AttachedEffects.RemoveAll((x) => x.GetType() == typeof(BlockEffects) && ((BlockEffects)x).numberToBlock <= 0);
        return isblocked;*/
    }

    public void EffectsOnBlock(StandardCube cube, Placeable target)
    {
        for(int i = cube.ObjectOns.Count-1; i >= 0; i--)
        {
            cube.ObjectOns[i].TriggerOnWalk(target);
        }
        foreach(Effect effect in cube.OnWalkEffects)
        {
            target.DispatchEffect(effect);
        }
        
    }


    public void AttachEffect(Effect effect)
    {

        if (effect.turnActiveEffect > 0)
        {
            effect.GetTarget().AttachedEffects.Add(effect);
        } else
        {
            Debug.LogError("Try to attach an effect with less than 1 turn activation");
        }
    }
    private void UseEffect(Effect effect)
    {
        effect.Use();
        effect.turnActiveEffect--;
        if (effect.turnActiveEffect <= 0)
        {
            effect.GetTarget().AttachedEffects.Remove(effect);
        }
        //GameManager.instance.CheckWinCondition();

    }

}
