using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// The game effect manager is used to solve any shoot or use of competence.The game must not go on while the solve function isn't over.
/// On notera que les effets sont appliqués dans leur ordre d'arrivée
/// </summary>
public class GameEffectManager : MonoBehaviour
{
    private List<Effect> toBeTreated = new List<Effect>();
    private List<BlockEffects> activeBlocks = new List<BlockEffects>(); // blocks comme bouclier qui bloque



    /// <summary>
    /// Résoud les effets à traiter
    /// </summary>
    public void Solve()
    {

        int k = 0;
        while (k < ToBeTreated.Count)
        {
            Effect element = ToBeTreated[k];


            bool canUse = true;
            int l = 0;
            while (l < ActiveBlocks.Count)
            {
                BlockEffects blockEffect = ActiveBlocks[l]; // si il n'y a pas de block, on applique l'effet



                if (blockEffect.ContientEffetType(element))

                {
                    blockEffect.NumberToBlock -= 1;
                    if (blockEffect.NumberToBlock > 0 || blockEffect.TourEffetActif > 0)
                    {
                        canUse = false;

                        break; // petite opti

                    }
                }
                l++;
            }
            if (canUse)
            {
                element.Use();



            }
            element.TourEffetActif--;
            k++;
        }
        int i = 0;
        while (i < ToBeTreated.Count) // la différence diminue de 1 a chaque tour de boucle
        {
            if (ToBeTreated[i].TourEffetActif <= 0)
            {
                ToBeTreated.RemoveAt(i);
            }
            else
            {
                i++;
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
            return activeBlocks;
        }

        set
        {
            activeBlocks = value;
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
