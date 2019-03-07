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
            if (effect.TriggerOnApply)
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
            if(!effect.TriggerOnce || effect.TriggerOnce && effect.TurnActiveEffect==1)
            {
                UseEffect(effect);
            }
            else
            { effect.TurnActiveEffect--;
            }
         
        }
        else//deletes the DOT
        {
            effect.GetTarget().AttachedEffects.RemoveAll((x) => x.GetType() == typeof(Effect));
        }
    }
    public bool CalculateEffectBlocked(Effect effect)
    {
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
        effect.GetTarget().AttachedEffects.RemoveAll((x) => x.GetType() == typeof(BlockEffects) && ((BlockEffects)x).numberToBlock <= 0);
        return isblocked;
    }
    public void AttachEffect(Effect effect)
    {

        if (effect.TurnActiveEffect > 0)
        {
            effect.GetTarget().AttachedEffects.Add(effect);
        }
    }
    private void UseEffect(Effect effect)
    {
        effect.Use();
        effect.TurnActiveEffect--;
        if (effect.TurnActiveEffect <= 0)
        {
            effect.GetTarget().AttachedEffects.Remove(effect);
        }
        //GameManager.instance.CheckWinCondition();

    }

}
