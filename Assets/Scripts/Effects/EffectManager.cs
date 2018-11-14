using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager:MonoBehaviour {
    public static EffectManager instance;
    void Awake()
    {
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

    }

    public void UseEffect(Effect effect)
    {
        bool isblocked = false;
        Placeable target = effect.GetTarget();
        foreach(Effect eff in target.AttachedEffects)
        {
            if(eff.GetType()==typeof(BlockEffects))
            {
                BlockEffects block= (BlockEffects)eff;
                if (block.listEffectsToBlock.Exists((x) => x.GetType() == effect.GetType()) && block.numberToBlock>0)
                {
                    block.numberToBlock--;
                    isblocked = true;
                }

            }
          
        }
        if(!isblocked)
        {
            effect.Use();
        }
        //Deletes all nulls blocks
        effect.GetTarget().AttachedEffects.RemoveAll((x) => x.GetType() == typeof(BlockEffects) && ((BlockEffects)x).numberToBlock <= 0);

    }
}
