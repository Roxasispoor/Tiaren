using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Représente a peut pres n'importe quoi pouvant occuper un bloc dans le grille
/// </summary>
public abstract class Placeable : MonoBehaviour
{
    private GameObject prefab;
    private Vector3Int position;
    private bool accessible;
    private bool walkable;
    private List<Effect> onWalkEffects;
    private bool movable;
    private bool destroyable;
    private TraversableType traversableChar;
    private TraversableType traversableBullet;

    private GravityType gravityType;
    private bool pickable;
    private EcraseType ecrasable;
    private bool explored;
    private List<Effect> onDestroyEffects;
    private List<HitablePoint> hitablePoints;
    private List<Effect> onDebutTour;
    private List<Effect> onFinTour;
    /// <summary>
    /// Le joueur a qui appartient le placeable. Les joueurs, équipe neutre(monstres neutres) et null(blocs indépendants)
    /// </summary>
    public Joueur joueur;



    /// <summary>
    /// 
    /// </summary>
    /// <param name="walkable">Indique si on peut marcher sur le bloc</param>
    /// <param name="movable">Indique siu le bloc est déplassable</param>
    /// <param name="traversableType"></param>
    /// <param name="gravityType"></param>
    /// <param name="pickable"></param>
    /// <param name="ecrasable"></param>
    public Placeable(Vector3Int position, bool walkable, List<Effect> onWalkEffects, bool movable, bool destroyable, TraversableType traversableChar, TraversableType traversableBullet,
        GravityType gravityType, bool pickable, EcraseType ecrasable, List<Effect> onDestroyEffects
        , List<HitablePoint> hitablePoints, List<Effect> onDebutTour, List<Effect> onFinTour, Joueur joueur)
    {


        this.position = position;
        this.accessible = false;
        this.walkable = walkable;
        this.OnWalkEffects = onWalkEffects;
        this.movable = movable;
        this.Destroyable = destroyable;
        this.TraversableChar = traversableChar;
        this.TraversableBullet = traversableBullet;
        this.GravityType = gravityType;
        this.pickable = pickable;
        this.Ecrasable = ecrasable;
        this.OnDestroyEffects = onDestroyEffects;
        this.hitablePoints = hitablePoints;
        this.onDebutTour = onDebutTour;
        this.OnFinTour = onFinTour;
        this.Joueur = joueur;
    }

    public bool Walkable
    {
        get
        {
            return walkable;
        }

        set
        {
            walkable = value;
        }
    }

    public bool Movable
    {
        get
        {
            return movable;
        }

        set
        {
            movable = value;
        }
    }

    public bool Pickable
    {
        get
        {
            return pickable;
        }

        set
        {
            pickable = value;
        }
    }


    /// <summary>
    /// Indique si on a déja fait nos test de gravité sur ce placeable
    /// </summary>
    public bool Explored
    {
        get
        {
            return explored;
        }

        set
        {
            explored = value;
        }
    }





    public List<Effect> OnDebutTour
    {
        get
        {
            return onDebutTour;
        }

        set
        {
            onDebutTour = value;
        }
    }

    public List<Effect> OnFinTour
    {
        get
        {
            return onFinTour;
        }

        set
        {
            onFinTour = value;
        }
    }

    public List<Effect> OnDestroyEffects
    {
        get
        {
            return onDestroyEffects;
        }

        set
        {
            onDestroyEffects = value;
        }
    }

    public List<Effect> OnWalkEffects
    {
        get
        {
            return onWalkEffects;
        }

        set
        {
            onWalkEffects = value;
        }
    }

    public List<HitablePoint> HitablePoints
    {
        get
        {
            return hitablePoints;
        }

        set
        {
            hitablePoints = value;
        }
    }

    public TraversableType TraversableChar
    {
        get
        {
            return traversableChar;
        }

        set
        {
            traversableChar = value;
        }
    }

    public TraversableType TraversableBullet
    {
        get
        {
            return traversableBullet;
        }

        set
        {
            traversableBullet = value;
        }
    }

    public GravityType GravityType
    {
        get
        {
            return gravityType;
        }

        set
        {
            gravityType = value;
        }
    }

    public EcraseType Ecrasable
    {
        get
        {
            return ecrasable;
        }

        set
        {
            ecrasable = value;
        }
    }

    public Joueur Joueur
    {
        get
        {
            return joueur;
        }

        set
        {
            joueur = value;
        }
    }

    public Vector3Int Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
        }
    }

    public bool Destroyable
    {
        get
        {
            return destroyable;
        }

        set
        {
            destroyable = value;
        }
    }


    /// <summary>
    /// Copie l'objet
    /// </summary>
    /// <returns>Retourne une copie de l'objet</returns>
    public virtual Placeable Cloner()
    {
        var copy = (Placeable)this.MemberwiseClone();
        return copy;
    }

    /// <summary>
    /// Méthode a appeler lors de la destruction de l'objet
    /// </summary>
    public virtual void Detruire()
    {
        if (this.Destroyable)
        {
            foreach (var effet in this.OnDestroyEffects)
            {
                effet.Use();
            }
        }
        Destroy(this);
        Destroy(this.gameObject);
    }
    public void OnMouseOver()
    {
        if (accessible) //on met en vert
        {
            if (Input.GetMouseButton(0))//on lance l'ordre a grille
            {

            }
        }
    }
}
