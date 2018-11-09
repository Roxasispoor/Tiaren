using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Effect to push a bloc
/// </summary>
public class Push : EffectOnPlaceable
{
    private float damage;
    private float nbCases;
    protected Vector3 direction;
    private bool isDirectionFromPosition;
    private bool doesHeightCount;
    private const float pushSpeed= 1;
   
    
    public Push(Push other) : base(other)
    {
        this.damage = other.damage;
        this.nbCases = other.nbCases;
        this.direction = other.direction;
        this.isDirectionFromPosition = other.isDirectionFromPosition;
        this.doesHeightCount = other.doesHeightCount;
    }
    public Push( int nbCases, float damage,bool doesHeightCount) : this(nbCases, damage)
    {
        this.doesHeightCount =doesHeightCount;
        isDirectionFromPosition = true;
    }

    /// <summary>
    /// create a Push
    /// </summary>
    /// <param name="target">target of blow</param>
    /// <param name="launcher">launcher of blow</param>
    public Push(Placeable target, Placeable launcher, int nbCases,float damage) : base(target, launcher)
    {
        this.nbCases = nbCases;
        this.damage = damage;
        isDirectionFromPosition = true;
      }

    public Push(int nbCases, float damage,Vector3 direction) :this(nbCases,damage)
    {

        this.direction = direction;
        isDirectionFromPosition = false;
        doesHeightCount = true;
    }
    public Push( int nbCases, float damage)
    {
        this.nbCases = nbCases;
        this.damage = damage;
    }
    public override Effect Clone()
    {
        return new Push(this);
    }
    private Vector2Int CalculateDelta()
    {
        int dx = 0, dz = 0;
        if (direction.x > 0.38268343236) //Trace an hexakaidecan, to chose from, 360/16=22.5; sin(22.5)=0.38268343236
        {
            dx = 1;
        }
        else if (direction.x < -0.38268343236)
        {
            dx = -1;
        }
        if (direction.y > 0.38268343236)
        {
            dz = 1;
        }
        else if (direction.y < -0.38268343236)
        {
            dz = -1;
        }
        return new Vector2Int(dx, dz);
    }
    public virtual void GetDirection()
    {
        direction=Target.GetPosition() - Launcher.GetPosition();
    }
    /// <summary>
    /// rounds up the angle to 0,45 or 90, does not use bresenham due to complexity and non deterministic
    /// </summary>
    override
    public void Use()
    {
        if (isDirectionFromPosition)
        {
            if (Launcher==Target)
            {
                Debug.Log("The hell, push caster is push target");
            }

                GetDirection();
              
                direction.Normalize();
            if(doesHeightCount)
            {
                direction.z = 0;
            }
        }
        //float angle = Mathf.Atan2(direction.y, direction.x); // we didive by the length of interval and round up
        //‎int hexakaidecan = (int) (angle / (22.5 * Mathf.Deg2Rad));
        Vector2Int delta = CalculateDelta();
        int distance = (int)nbCases;
        if (delta.x*delta.y!=0)// si les deux sont à 1
        {
            distance = (int)(nbCases / 1.4142 + 0.5); //Round up the euclidian distance
        }
        Placeable directCollision=null ;
        List<Placeable> diagonalCollisions = new List<Placeable>();
        List<Vector3> path= CheckPath(distance, delta,out directCollision, out diagonalCollisions);
        //Make damage and chek dodge conditions, destructions.... to modify according gameplay decided
        if (directCollision != null && directCollision.IsLiving())
        {
            EffectManager.instance.UseEffect(new Damage((LivingPlaceable)directCollision,this.Launcher,damage));
        }
        foreach(Placeable diagcoll in diagonalCollisions)
        {
            if (diagcoll != null && diagcoll.IsLiving())
            {
                EffectManager.instance.UseEffect(new Damage((LivingPlaceable)diagcoll, this.Launcher, damage));
            }
        }
        Grid.instance.MoveBlock(Target, new Vector3Int((int)path[path.Count - 1].x, (int)path[path.Count - 1].y, (int)path[path.Count - 1].z));
        Player.MoveAlongBezier(path, Target, pushSpeed);

    }
    //Todo check que ça sort pas du terrain...
    public List<Vector3> CheckPath(int distance, Vector2Int delta,out Placeable directCollision, out List<Placeable> diagonalCollisions)
    {
        directCollision = null;
        List<Vector3> positions = new List<Vector3>();
        diagonalCollisions = new List<Placeable>();
        bool isDiagonal = delta.x * delta.y != 0;
        for (int i = 0; i < distance; i++)
        {
            if (Grid.instance.CheckNull(new Vector3Int(Target.GetPosition().x + delta.x *i,Target.GetPosition().y,
                Target.GetPosition().z + delta.y * i)))
               {
                directCollision=Grid.instance.GridMatrix[Target.GetPosition().x + delta.x * i, Target.GetPosition().y, Target.GetPosition().z + delta.y * i];
            }
            else
            {
                //check diagonal
                if (isDiagonal && Grid.instance.CheckNull(
                    new Vector3Int(Target.GetPosition().x + delta.x * (i-1), Target.GetPosition().y, Target.GetPosition().z + delta.y * i)))
                {
                    diagonalCollisions.Add(Grid.instance.GridMatrix[Target.GetPosition().x + delta.x * (i - 1),
                Target.GetPosition().y, Target.GetPosition().z + delta.y * i]);

                }
                if (isDiagonal && Grid.instance.CheckNull(
                    new Vector3Int(Target.GetPosition().x + delta.x * i, Target.GetPosition().y, Target.GetPosition().z + delta.y * (i-1))))
                {
                    diagonalCollisions.Add(Grid.instance.GridMatrix[Target.GetPosition().x + delta.x * i,
                Target.GetPosition().y, Target.GetPosition().z + delta.y * (i - 1)]);

                }


            }
            if (directCollision || diagonalCollisions.Count > 0)
            {
                break;
            }
            else
            {
                if(Grid.instance.CheckRange(
                    new Vector3Int(Target.GetPosition().x + delta.x * i, Target.GetPosition().y, Target.GetPosition().z + delta.y * i)))
                    { 
                positions.Add(new Vector3(Target.GetPosition().x + delta.x * i, Target.GetPosition().y, Target.GetPosition().z + delta.y * i));
                }
            }
        }
        return positions;
    }
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
