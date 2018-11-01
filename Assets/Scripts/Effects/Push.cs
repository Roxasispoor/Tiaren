using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Effect to push a bloc
/// </summary>
public class Push : Effect
{
    private float damage;
    private int nbCases;
    private GameManager gameManager;
    /// <summary>
    /// super push destroy bloc and increase damage
    /// </summary>
    private bool isSuper;

    public Push(Effect other) : base(other)
    {
    }

    /// <summary>
    /// create a damage object
    /// </summary>
    /// <param name="target">target of blow</param>
    /// <param name="launcher">launcher of blow</param>
    public Push(Placeable target, Placeable launcher, int nbCases, bool isSuper) : base(target, launcher)
    {
        this.nbCases = nbCases;
        this.isSuper = isSuper;
    }

    public override Effect Clone()
    {
        throw new System.NotImplementedException();
    }

    override
    public void Use() { }

    //TODO: a revoir, vieux et mal fait
    /*
    override
        public void Use()
    {
        if (Cible != Lanceur)
        {
            Vector3Int vectDiff = Cible.Position - Lanceur.Position;
            if (vectDiff.magnitude == 1)
            {
                for (int i = 0; i < nbCases; i++)
                {
                    //Si il y a de la place derrière la target on la déplace
                    Vector3Int posConsidered = Cible.Position + vectDiff;
                    if (gameManager.GrilleJeu.Grid[posConsidered.x, posConsidered.y, posConsidered.z] == null)
                    {
                        gameManager.GrilleJeu.DeplaceBloc(Cible, posConsidered);

                    }
                    //si derrière il y avait un placeable il prend des dégats
                    else if (gameManager.GrilleJeu.Grid[posConsidered.x, posConsidered.y, posConsidered.z].GetType() == typeof(LivingPlaceable))
                    {
                        if (isSuper)
                        {
                            if (Cible.GetType() == typeof(LivingPlaceable))
                            {
                                gameManager.GameEffectManager.ToBeTreated.Add(new Damage(Cible, Lanceur, nbCases * 35));
                            }
                            gameManager.GameEffectManager.ToBeTreated.Add(new Damage(gameManager.GrilleJeu.Grid[posConsidered.x, posConsidered.y, posConsidered.z], Cible, nbCases * 35));
                            break;
                        }
                        else
                        {
                            if (Cible.GetType() == typeof(LivingPlaceable))
                            {
                                gameManager.GameEffectManager.ToBeTreated.Add(new Damage(Cible, Lanceur, nbCases * 20));
                            }
                            gameManager.GameEffectManager.ToBeTreated.Add(new Damage(gameManager.GrilleJeu.Grid[posConsidered.x, posConsidered.y, posConsidered.z], Cible, nbCases * 20));
                            break;
                        }
                    }
                    else if (gameManager.GrilleJeu.Grid[posConsidered.x, posConsidered.y, posConsidered.z].GetType() == typeof(Placeable))
                    {

                        if (isSuper)
                        {
                            //on deplace le placeable
                            gameManager.GrilleJeu.DeplaceBloc(Cible, posConsidered);
                            if (Cible.GetType() == typeof(LivingPlaceable))
                            {
                                gameManager.GameEffectManager.ToBeTreated.Add(new Damage(Cible, Lanceur, nbCases * 35));
                            }
                        }
                        else //on applique des dommages réduit si il y a lieu
                        {
                            if (Cible.GetType() == typeof(LivingPlaceable))
                            {
                                gameManager.GameEffectManager.ToBeTreated.Add(new Damage(Cible, Lanceur, nbCases * 20));
                            }

                            break;
                        }
                    }

                }




            }
        }
    }
     */

}
