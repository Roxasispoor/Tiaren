using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// The game effect manager is used to solve any shoot or use of competence.The game must not go on while the solve function isn't over.
/// On notera que les effets sont appliqués dans leur ordre d'arrivée
/// </summary>
public class GameEffectManager:MonoBehaviour
{
    private List<Effect> toBeTreated;
    private List<BlockEffects> activeBlocks;

   

    /// <summary>
    /// Résoud les effets à traiter
    /// </summary>
    public void Solve()
    {
        foreach(Effect element in ToBeTreated)
        {
            bool canUse = true;
            foreach(BlockEffects blockEffect in ActiveBlocks) // si il n'y a pas de block, on applique l'effet
            {
              

                if (blockEffect.ContientEffetType(element)) 
                //attention a bien override contains
                {
                    blockEffect.NumberToBlock -= 1;
                    if(blockEffect.NumberToBlock>0 || blockEffect.TourEffetActif>0)
                    {
                        canUse = false;
                        break; // petite opti

                    }
                }
            }
            if(canUse)
            {
                element.Use();
            }
        }
        ActiveBlocks.RemoveAll(x => x.NumberToBlock <= 0 && x.TourEffetActif < 0);// si il n'y a plus de coup a bloquer ni de tour effet actif on le supprime de la liste des blocs actifs
    }

    /// <summary>
    /// Liste des blocs actif
    /// </summary>
    public List<BlockEffects> ActiveBlocks
    {
        get
        {
            return ActiveBlocks;
        }

        set
        {
            ActiveBlocks = value;
        }
    }

    public List<Effect> ToBeTreated
    {
        get
        {
            return toBeTreated;
        }

        set
        {
            toBeTreated = value;
        }
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
