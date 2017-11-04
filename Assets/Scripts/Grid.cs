using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe représentant une grille de jeux pour une partie
/// </summary>
public class Grid : MonoBehaviour {

    //  50 x 50 x 5 = 12 500 blocs
    public static int sizeX = 50;
    public static int sizeY = 5;
    public static int sizeZ = 50;
    

    /// <summary>
    /// Représente la grille de jeux
    /// </summary>
    private Placeable[,,] grid;
    /// <summary>
    /// Pourquoi est-il fixé ?
    /// </summary>
    public int randomParameter = 90;
    /// <summary>
    /// on mettra tous les floaties dedans pour ne pas avoir a les chercher lors de la gravite connexe
    /// </summary>
    public List<Placeable> floaties;

    /// <summary>
    /// Crée une grille aléatoire a partir de randomParameter
    /// </summary>
    public void CreateRandomGrid()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (Random.Range(0, 100) < randomParameter && y < sizeY - 1) // la deuxième condition pourrait simplement etre dans le for du Y
                    {
                        
                    }
                }
            }
        }
    }

   
    /// <summary>
    /// Fonction permettant de trouver et afficher un chemin d'un point A à un point B
    /// Seuls les cases sur lesquelles on est sûrs de pouvoir aller sont ajoutées
    /// <param name="deplacement">Valeur de déplacement d'un personnage</param>
    /// <param name="saut">Valeur de saut d'un personnage</param>
    /// <param name="positionBloc">Position du bloc de départ, c'est à dire celui SOUS le personnage</param>
    /// <returns>Retourne une grille qui permet d'avoir les positions de chacun des blocs du chemin, c'est à dire les blocs appartenant au SOL. Le personnage se déplacera une case au dessus</returns>
    /// </summary>
    public DistanceAndParent[,,] CanGo(LivingPlaceable livingPlaceable,int saut, Vector3 positionBloc)
    {
        int deplacement = livingPlaceable.PmActuels;
        
        DistanceAndParent[,,] gridBool = new DistanceAndParent[sizeX, sizeY, sizeZ];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    gridBool[x, y, z] = new DistanceAndParent(x,y,z);
                }
            }
        }

        Queue<Vector3> queue = new Queue<Vector3>();
        queue.Enqueue(positionBloc);
        DistanceAndParent parent = null;
        while (parent==null||(parent.GetDistance() < deplacement && queue.Count != 0))
        {
            //On mets ceux à coté, ceux au dessus accessibles ou ceux en dessous accessibles
            //coté

            Vector3 posBlocActuel = queue.Dequeue();
            parent = gridBool[(int)posBlocActuel.x, (int)posBlocActuel.y, (int)posBlocActuel.z];
     
            grid[(int)posBlocActuel.x, (int)posBlocActuel.y, (int)posBlocActuel.z].GetComponent<Renderer>().material.color = Color.cyan;
            //là mettre l'affichage du bloc en bleu
            for (int yactuel = -saut + 1; yactuel < saut; yactuel++) 
            {
               
                if ((int)posBlocActuel.y + yactuel>=0 && (int)posBlocActuel.y + yactuel<sizeY && posBlocActuel.x<sizeX - 1 &&
                    gridBool[(int)posBlocActuel.x + 1, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z].GetDistance() == -1 &&
                    grid[(int)posBlocActuel.x + 1, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z] != null &&
                    (grid[(int)posBlocActuel.x + 1, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z] == null
                    || grid[(int)posBlocActuel.x + 1, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z].TraversableChar==TraversableType.ALLTHROUGH
                    ||grid[(int)posBlocActuel.x + 1, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z].TraversableChar==TraversableType.ALLIESTHROUGH &&
                    grid[(int)posBlocActuel.x + 1, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z].Joueur==livingPlaceable.Joueur)
                    && grid[(int)posBlocActuel.x + 1, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z].Walkable
                    )
                {//si le sommet n'est pas marqué et qu'il existe, et qu'il n'y a rien au dessus ou que celui du dessus est traversable, que le bloc est walkable
                  
                    queue.Enqueue(new Vector3(posBlocActuel.x + 1, posBlocActuel.y + yactuel, posBlocActuel.z));
                    gridBool[(int)posBlocActuel.x + 1, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z].SetDistance(
                        parent.GetDistance()+1);
                    gridBool[(int)posBlocActuel.x + 1, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z].SetParent(parent);
                }

                if ((int)posBlocActuel.y + yactuel >= 0 && (int)posBlocActuel.y + yactuel < sizeY && posBlocActuel.x > 0 &&
                    gridBool[(int)posBlocActuel.x - 1, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z].GetDistance() == -1 &&
                    grid[(int)posBlocActuel.x - 1, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z] != null &&
                    (grid[(int)posBlocActuel.x - 1, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z] == null
                    || grid[(int)posBlocActuel.x - 1, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z].TraversableChar == TraversableType.ALLTHROUGH
                    || grid[(int)posBlocActuel.x - 1, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z].TraversableChar == TraversableType.ALLIESTHROUGH &&
                    grid[(int)posBlocActuel.x - 1, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z].Joueur == livingPlaceable.Joueur)
                    && grid[(int)posBlocActuel.x - 1, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z].Walkable)
                {//si le sommet n'est pas marqué et qu'il existe, et qu'il n'y a rien au dessus
                   
                    queue.Enqueue(new Vector3(posBlocActuel.x - 1, posBlocActuel.y + yactuel, posBlocActuel.z));
                    gridBool[(int)posBlocActuel.x - 1, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z].SetDistance(
                        parent.GetDistance() + 1);
                    gridBool[(int)posBlocActuel.x - 1, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z].SetParent(parent);
                    
                    
                }
                if ((int)posBlocActuel.y + yactuel >= 0 && (int)posBlocActuel.y + yactuel < sizeY && posBlocActuel.z < sizeZ - 1 &&
                    gridBool[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z + 1].GetDistance() == -1 &&
                    grid[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z + 1] != null &&
                    (grid[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z + 1] == null
                    || grid[(int)posBlocActuel.x , (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z + 1].TraversableChar == TraversableType.ALLTHROUGH
                    || grid[(int)posBlocActuel.x , (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z + 1].TraversableChar == TraversableType.ALLIESTHROUGH &&
                    grid[(int)posBlocActuel.x , (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z + 1].Joueur == livingPlaceable.Joueur)
                    && grid[(int)posBlocActuel.x , (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z + 1].Walkable)

                {//si le sommet n'est pas marqué et qu'il existe et qu'il n'y a rien au dessus
                    queue.Enqueue(new Vector3(posBlocActuel.x, posBlocActuel.y + yactuel, posBlocActuel.z + 1));
                    gridBool[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z + 1].SetDistance(
                        parent.GetDistance() + 1);
                    gridBool[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z+1].SetParent(parent);

                }
                if ((int)posBlocActuel.y + yactuel >= 0 && (int)posBlocActuel.y + yactuel < sizeY && posBlocActuel.z > 0 &&
                    gridBool[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z - 1].GetDistance() == -1 &&
                    grid[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z - 1] != null &&
                    (grid[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z + 1] == null
                    || grid[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z - 1].TraversableChar == TraversableType.ALLTHROUGH
                    || grid[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z - 1].TraversableChar == TraversableType.ALLIESTHROUGH &&
                    grid[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel + 1, (int)posBlocActuel.z - 1].Joueur == livingPlaceable.Joueur)
                    && grid[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z - 1].Walkable)
                {//si le sommet n'est pas marqué et qu'il existe, et qu'il n'y a rien au dessus
                    queue.Enqueue(new Vector3(posBlocActuel.x, posBlocActuel.y + yactuel, posBlocActuel.z - 1));
                    gridBool[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z - 1].SetDistance(
                        parent.GetDistance() + 1);
                    gridBool[(int)posBlocActuel.x, (int)posBlocActuel.y + yactuel, (int)posBlocActuel.z - 1].SetParent(parent);
                }
            }
       
        }
        return gridBool;
    }

    /// <summary>
    /// Constructeur standard
    /// </summary>
    public Grid()
    {
       
    }

    /// <summary>
    /// Fonction lancée dès le début du jeu.
    /// </summary>
    void Start()
    {
        grid = new Placeable[sizeX, sizeY, sizeZ];

        CreateRandomGrid();
       // CanGo(4, 1, new Vector3(25, 3, 25));



    }

    /// <summary>
    /// Fonction qui intitalise la valeur du booléen explored des blocs de la grille
    /// </summary>
	private void InitialiseExplored(bool value)
	{
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				for (int z = 0; z < sizeZ; z++)
				{
                    if (grid[x, y, z] != null)
                    {
                        grid[x, y, z].Explored = value;
                    }
				}
			}
		}
        
	}
    /// <summary>
    /// Parcours de recherche en profondeur pour la gravite connexe
    /// ça les explore ok, mais il manque la partie chute si non gravité
    /// </summary>
    /// <param name="positionSommetX"></param>
    /// <param name="positionSommetY"></param>
    /// <param name="positionSommetZ"></param>
	private void Explore(int positionSommetX,int positionSommetY,int positionSommetZ)
	{
		grid[positionSommetX, positionSommetY, positionSommetZ].Explored = true;

        //on considère les 4 à cotés et ceux au dess(o)us ... donc les 6 cotés ?
        //si les voisins sont  non nuls et non explorés et toujours dans la map
        if (positionSommetX > 0 && grid[positionSommetX - 1, positionSommetY, positionSommetZ] != null &&
            !grid[positionSommetX - 1, positionSommetY, positionSommetZ].Explored)
        {
            Explore(positionSommetX - 1, positionSommetY, positionSommetZ);
        }
        if (positionSommetX < sizeX - 1 && grid[positionSommetX + 1, positionSommetY, positionSommetZ] != null &&
            !grid[positionSommetX + 1, positionSommetY, positionSommetZ].Explored)
        {
            Explore(positionSommetX + 1, positionSommetY, positionSommetZ);
        }
        if (positionSommetY > 0 && grid[positionSommetX , positionSommetY - 1, positionSommetZ] != null &&
            !grid[positionSommetX, positionSommetY - 1, positionSommetZ].Explored)
        {
            Explore(positionSommetX , positionSommetY - 1, positionSommetZ);
        }
        if (positionSommetY < sizeY - 1 && grid[positionSommetX, positionSommetY + 1, positionSommetZ] != null &&
            !grid[positionSommetX, positionSommetY + 1, positionSommetZ].Explored)
        {
            Explore(positionSommetX, positionSommetY + 1, positionSommetZ);
        }
        if (positionSommetZ > 0 && grid[positionSommetX, positionSommetY, positionSommetZ -1 ] != null &&
            !grid[positionSommetX, positionSommetY, positionSommetZ - 1].Explored)
        {
            Explore(positionSommetX, positionSommetY, positionSommetZ - 1);
        }
        if (positionSommetZ > sizeZ - 1 && grid[positionSommetX, positionSommetY , positionSommetZ + 1] != null &&
            !grid[positionSommetX, positionSommetY , positionSommetZ + 1].Explored)
        {
            Explore(positionSommetX, positionSommetY , positionSommetZ + 1);
        }
    }

    /// <summary>
    /// Vérifie que toute la grille a été explorée
    /// </summary>
    /// <returns></returns>
    public bool IsGridAllExplored()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if(grid[x,y,z]!=null && !grid[x,y,z].Explored)
                    // si on a des trucs non explorés, c'est à dire qu'ils doivent tomber donc faux
                    {
                        return false;

                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Seule fonction qui sera appelée pour gérer toute la gravité.
    /// </summary>
    public void Gravite()
    {
        GraviteSimple();
        //VERIFIER que a la creation explored = false. Tant que tous ne sont pas explorés
        while (!IsGridAllExplored())
        {
            //  On commence sur tous ceux d'en bas qui ne sont pas marqués, on expand,
            //  puis on passe aux floaties, on expand. Tous les autres tomberont. 
            //  0 = pas vu rien a faire; 1 vu
            InitialiseExplored(false);

            //On commence sur tous ceux d'en bas qui ne sont pas marqués, on expand , puis on passe aux floaties, on expand. Tous les autres tomberont. 

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        if (grid[x, y, z] != null && !grid[x, y, z].Explored && (y == 0 || grid[x, y, z].GravityType == GravityType.GRAVITE_NULLE))
                        //On lance le dfs sur les non vides qui ne sont pas explorés, au sol ou sans gravité
                        {
                            Explore(x, y, z);

                        }


                    }
                }
            }
            //Ensuite tous les blocs non nuls avec explored à false tombent de 1
            TombeConnexe();
        }
    }
    /// <summary>
    /// Fait tomber les blocs qui sont en gravite connexe. Il s'agit de tous les non nuls non explorés
    /// On tombe tant qu'il n'y a rien en dessous ou une une une une ?
    /// </summary>
    public void TombeConnexe()
    {
        bool mustStop = false;
        while (!mustStop)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        if (grid[x, y, z] != null && !grid[x, y, z].Explored) // si le bloc est non nul, que le bloc était sensé tombé
                                                                              //Alors on tombe.
                        {

                            GereEcrasageBloc(x, y, z,1); // On peut descendre de 1 il faut juste appliquer le bon ecrasage si besoin
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Fonction qui va s'occuper de gérer les colisions verticale d'un bloc par un autre
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="ydescente"></param>
    public void GereEcrasageBloc(int x, int y, int z,int ydescente)
    {
           if (grid[x, y - ydescente, z] == null)// On copie et on détruit
            {
                grid[x, y - ydescente, z] = grid[x, y, z].Clone();
                grid[x, y, z] = null;
            }
            else if (grid[x, y - ydescente, z].Ecrasable == EcraseType.ECRASEDESTROYBLOC)// On détruit le bloc et on trigger ses effets
            {
                grid[x, y, z].Destroy();
                grid[x, y, z] = null;

            }
            else if (grid[x, y - ydescente, z].Ecrasable == EcraseType.ECRASELIFT)// On copie et on détruit
            {
                int ymontee = y;
                while (ymontee < sizeY && grid[x, ymontee, z] != null) // la verification devrait être non nécessaire en y mais bon -_-
                {
                    ymontee++;
                }
                grid[x, ymontee, z] = grid[x, y - ydescente, z].Clone();
                grid[x, y - ydescente, z] = grid[x, y, z].Clone();
                grid[x, y, z] = null;
            }
            else if (grid[x, y - ydescente, z].Ecrasable == EcraseType.ECRASEDEATH)
            {
                grid[x, y - ydescente, z].Destroy();
                grid[x, y - ydescente, z] = grid[x, y, z].Clone();
                grid[x, y, z] = null;

            }
            


            //Soit c'était un écrasable de type ecrasestay, soit c'était du vide
        
    }
    /// <summary>
    /// Gère la gravite des objets de type gravite simple, c'est à dire s'il n'y a rien en dessous on tombe
    /// </summary>
    public void GraviteSimple()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (grid[x, y, z] != null && grid[x, y, z].GravityType == GravityType.GRAVITE_SIMPLE)
                    {
                        int ydescente = 0;

                        while (y > 0 && (grid[x, y - ydescente, z] != null || grid[x, y - ydescente, z].Ecrasable == EcraseType.ECRASESTAY))

                        {
                            ydescente++;
                        }

                        GereEcrasageBloc(x, y, z, ydescente);


                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
