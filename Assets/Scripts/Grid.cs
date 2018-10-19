using System.Collections;
using System.Collections.Generic;
using System.IO;

using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Class representing a cube in a path
/// </summary>

public class NodePath
{
    public int x, y, z;
    NodePath parent;

    private int distanceFromStart;

    public int DistanceFromStart
    {
        get
        {
            return distanceFromStart;
        }

        private set
        {
            distanceFromStart = value;
        }
    }

    public NodePath(int x, int y, int z, int distanceFromStart, NodePath parent)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.DistanceFromStart = distanceFromStart;
        this.parent = parent;
    }

    public static NodePath startPath(Vector3 pos)
    {
        return startPath((int)pos.x, (int)pos.y, (int)pos.z);
    }

    public static NodePath startPath(int x, int y, int z)
    {
        return new NodePath(x, y, z-1, 0, null);
    }

    public Vector3[] getFullPath()
    {
        Vector3[] path = new Vector3[DistanceFromStart + 1];
        NodePath currentNode = this;
        for (int i = DistanceFromStart + 1; i > 0; i--)
        {
            path[i] = new Vector3(this.x, this.y, this.z);
            currentNode = currentNode.parent;
        }
        return path;
    }

    public Vector3 GetVector3()
    {
        return new Vector3(x, y, z);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj.GetType() == typeof(NodePath))
        {
            return false;
        }
        NodePath other = (NodePath)obj;
        return x == other.x && y == other.y && z == other.z;
    }

    public override int GetHashCode()
    {
        var hashCode = 373119288;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        hashCode = hashCode * -1521134295 + z.GetHashCode();
        return hashCode;
    }
}

/// <summary>
/// Class representing a grid for a game
/// </summary>

public class Grid : MonoBehaviour
{
    public static Grid instance;
    //  50 x 50 x 5 = 12 500 blocs
    public int sizeX = 50;
    public int sizeY = 6;
    public int sizeZ = 50;
    public DistanceAndParent[,,] gridBool;


    /// <summary>
    /// Represents grid for the game
    /// </summary>

    private Placeable[,,] gridMatrix;
    /// <summary>
    /// Parameters for generation the grid
    /// </summary>
    public int randomParameter = 90;
    /// <summary>
    /// Put all the floaties in order to avoid to look for them when applying related gravity
    /// </summary>
    public List<Placeable> floaties;

    public GameObject[] prefabsList;


    public Placeable[,,] GridMatrix
    {
        get
        {
            return gridMatrix;
        }

        set
        {
            gridMatrix = value;
        }
    }

   



    /// <summary>
    /// Create a random grid from randomParameter
    /// </summary>
    public void CreateRandomGrid(GameObject parent)
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (Random.Range(0, 100) < randomParameter && y < sizeY - 2) // second condition could be in fory 
                    {
                        GameObject obj = Instantiate(prefabsList[0], new Vector3(x, y, z), Quaternion.identity, parent.transform);
                        gridMatrix[x, y, z] = obj.GetComponent<Placeable>();
                        NetworkServer.Spawn(obj);



                    }
                }
            }
        }

    }


    /// <summary>
    /// Function finding and displaying path from point A to point B
    /// Only cases where the character can go are added
    /// <param name="livingPlaceable">Character to study</param>
    /// <param name="jumpValue">Value of jump for the character</param>
    /// <param name="positionBloc">Position of starting bloc (under the character)</param>
    /// <returns>Returns a grid which can give positions of every cube on the path (belonging to floor). Character will be set on top of the cube target</returns>
    /// </summary>
    public DistanceAndParent[,,] CanGo(LivingPlaceable livingPlaceable, int jumpValue, Vector3Int positionBloc)
    {

        int shifting = livingPlaceable.CurrentPM;



        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    gridBool[x, y, z] = new DistanceAndParent(x, y, z);
                }
            }
        }

        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(positionBloc);
        DistanceAndParent parent = null;
        gridBool[positionBloc.x, positionBloc.y, positionBloc.z].SetDistance(0);
        while (parent == null || ((parent.GetDistance() <= shifting) && queue.Count != 0))
        {
            //adding side cubes, top cubes if reachable, bottom cubes if reachable

            Vector3Int posCurrentBloc = queue.Dequeue();


            parent = gridBool[posCurrentBloc.x, posCurrentBloc.y, posCurrentBloc.z];
            if (parent.GetDistance() <= shifting)
            {

                Placeable testa = gridMatrix[posCurrentBloc.x, posCurrentBloc.y, posCurrentBloc.z];
                if (testa.gameObject && testa.gameObject.GetComponent<Renderer>() != null)
                {
                    //Sending id because nothing assures on client side that blocs are well placed
                    GameManager.instance.PlayingPlaceable.Player.RpcMakeCubeBlue(testa.netId);
                  
                }
                //then displaying bloc on blue
                for (int ycurrent = -jumpValue + 1; ycurrent < jumpValue; ycurrent++)
                {

                    if (posCurrentBloc.y + ycurrent >= 0 && posCurrentBloc.y + ycurrent < sizeY && posCurrentBloc.x < sizeX - 1 && //above 0, under max, inside terrain in x
                        gridBool[posCurrentBloc.x + 1, posCurrentBloc.y + ycurrent, posCurrentBloc.z].GetDistance() == -1 && //if not already seen at this point
                        gridMatrix[posCurrentBloc.x + 1, posCurrentBloc.y + ycurrent, posCurrentBloc.z] != null &&        // and only if the bloc exists
                        (gridMatrix[posCurrentBloc.x + 1, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z] == null  //if the bloc on the top is empty
                        || gridMatrix[posCurrentBloc.x + 1, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z].TraversableChar == TraversableType.ALLTHROUGH //or crossable in general
                        || gridMatrix[posCurrentBloc.x + 1, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z].TraversableChar == TraversableType.ALLIESTHROUGH && //or only by an ally
                        gridMatrix[posCurrentBloc.x + 1, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z].Player == livingPlaceable.Player) //if we are the ally
                        && gridMatrix[posCurrentBloc.x + 1, posCurrentBloc.y + ycurrent, posCurrentBloc.z].Walkable
                        )
                    {//if edge is not marked and exists, if there is nothing above it or if the bloc above is crossable, if bloc is walkable

                        queue.Enqueue(new Vector3Int(posCurrentBloc.x + 1, posCurrentBloc.y + ycurrent, posCurrentBloc.z));
                        gridBool[posCurrentBloc.x + 1, posCurrentBloc.y + ycurrent, posCurrentBloc.z].SetDistance(
                            parent.GetDistance() + 1);
                        gridBool[posCurrentBloc.x + 1, posCurrentBloc.y + ycurrent, posCurrentBloc.z].SetParent(parent);
                    }

                    if (posCurrentBloc.y + ycurrent >= 0 && posCurrentBloc.y + ycurrent < sizeY && posCurrentBloc.x > 0 &&
                        gridBool[posCurrentBloc.x - 1, posCurrentBloc.y + ycurrent, posCurrentBloc.z].GetDistance() == -1 &&
                        gridMatrix[posCurrentBloc.x - 1, posCurrentBloc.y + ycurrent, posCurrentBloc.z] != null &&
                        (gridMatrix[posCurrentBloc.x - 1, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z] == null
                        || gridMatrix[posCurrentBloc.x - 1, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z].TraversableChar == TraversableType.ALLTHROUGH
                        || gridMatrix[posCurrentBloc.x - 1, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z].TraversableChar == TraversableType.ALLIESTHROUGH &&
                        gridMatrix[posCurrentBloc.x - 1, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z].Player == livingPlaceable.Player)
                        && gridMatrix[posCurrentBloc.x - 1, posCurrentBloc.y + ycurrent, posCurrentBloc.z].Walkable)
                    {//if edge is not marked and exists, if there is nothing above

                        queue.Enqueue(new Vector3Int(posCurrentBloc.x - 1, posCurrentBloc.y + ycurrent, posCurrentBloc.z));
                        gridBool[posCurrentBloc.x - 1, posCurrentBloc.y + ycurrent, posCurrentBloc.z].SetDistance(
                            parent.GetDistance() + 1);
                        gridBool[posCurrentBloc.x - 1, posCurrentBloc.y + ycurrent, posCurrentBloc.z].SetParent(parent);


                    }
                    if (posCurrentBloc.y + ycurrent >= 0 && posCurrentBloc.y + ycurrent < sizeY && posCurrentBloc.z < sizeZ - 1 &&
                        gridBool[posCurrentBloc.x, posCurrentBloc.y + ycurrent, posCurrentBloc.z + 1].GetDistance() == -1 &&
                        gridMatrix[posCurrentBloc.x, posCurrentBloc.y + ycurrent, posCurrentBloc.z + 1] != null &&
                        (gridMatrix[posCurrentBloc.x, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z + 1] == null
                        || gridMatrix[posCurrentBloc.x, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z + 1].TraversableChar == TraversableType.ALLTHROUGH
                        || gridMatrix[posCurrentBloc.x, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z + 1].TraversableChar == TraversableType.ALLIESTHROUGH &&
                        gridMatrix[posCurrentBloc.x, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z + 1].Player == livingPlaceable.Player)
                        && gridMatrix[posCurrentBloc.x, posCurrentBloc.y + ycurrent, posCurrentBloc.z + 1].Walkable)

                    {//if edge is not marked and exists, if there is nothing above
                        queue.Enqueue(new Vector3Int(posCurrentBloc.x, posCurrentBloc.y + ycurrent, posCurrentBloc.z + 1));
                        gridBool[posCurrentBloc.x, posCurrentBloc.y + ycurrent, posCurrentBloc.z + 1].SetDistance(
                            parent.GetDistance() + 1);
                        gridBool[posCurrentBloc.x, posCurrentBloc.y + ycurrent, posCurrentBloc.z + 1].SetParent(parent);

                    }
                    if (posCurrentBloc.y + ycurrent >= 0 && posCurrentBloc.y + ycurrent < sizeY && posCurrentBloc.z > 0 &&
                        gridBool[posCurrentBloc.x, posCurrentBloc.y + ycurrent, posCurrentBloc.z - 1].GetDistance() == -1 &&
                        gridMatrix[posCurrentBloc.x, posCurrentBloc.y + ycurrent, posCurrentBloc.z - 1] != null &&
                        (gridMatrix[posCurrentBloc.x, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z - 1] == null
                        || gridMatrix[posCurrentBloc.x, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z - 1].TraversableChar == TraversableType.ALLTHROUGH
                        || gridMatrix[posCurrentBloc.x, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z - 1].TraversableChar == TraversableType.ALLIESTHROUGH &&
                        gridMatrix[posCurrentBloc.x, posCurrentBloc.y + ycurrent + 1, posCurrentBloc.z - 1].Player == livingPlaceable.Player)
                        && gridMatrix[posCurrentBloc.x, posCurrentBloc.y + ycurrent, posCurrentBloc.z - 1].Walkable)
                    {//if edge is not marked and exists, if there is nothing above
                        queue.Enqueue(new Vector3Int(posCurrentBloc.x, posCurrentBloc.y + ycurrent, posCurrentBloc.z - 1));
                        gridBool[posCurrentBloc.x, posCurrentBloc.y + ycurrent, posCurrentBloc.z - 1].SetDistance(
                            parent.GetDistance() + 1);
                        gridBool[posCurrentBloc.x, posCurrentBloc.y + ycurrent, posCurrentBloc.z - 1].SetParent(parent);
                    }
                }
            }

        }

        return gridBool;
    }

    private int CheckUnder(int x, int y, int z, int jumpValue) // z correspond à la hauteur du bloc sur lequel marche le joueur
    {
        for (int i=0; i < jumpValue; i++)
        {
            if (z - i < 0)
                return -1;

            if (GridMatrix[x,y,z - i] != null)
            {
                if (GridMatrix[x, y, z - i].Walkable)
                {
                    return z - i;
                }else
                {
                    return -1;
                }
            }
        }
        return -1;
    }

    private List<int> CheckUp(int x, int y, int z, int x_orig, int y_orig, int jumpValue) // z correspond à la hauteur du bloc sur lequel marche le joueur
    {
        List<int> returnList = new List<int>();

        for (int i = 1; i < jumpValue+1; i++)
        {
            if (z + i > sizeZ)
                return returnList;
            if (z + i + 1 < sizeZ && GridMatrix[x, y, z + i + 1] != null)
            {
                return returnList;
            }
            if (GridMatrix[x, y, z + i] != null)
            {
                if (GridMatrix[x, y, z + i].Walkable)
                {
                    returnList.Add(z+i);
                }
                else
                {
                    return returnList;
                }
            }
        }
        return returnList;
    }

    // startPosition : position du joueur
    public List<NodePath> CanGo(Vector3 startPosition, int distance, int jumpValue, Player player = null)
    {
        Queue<NodePath> toCheck = new Queue<NodePath>();
        List<NodePath> accessibleBloc = new List<NodePath>();

        toCheck.Enqueue(NodePath.startPath(startPosition));

        int n_iteration = 0;

        while(toCheck.Count > 0)
        {
            n_iteration++;

            if (n_iteration%1000 == 0)
            {
                Debug.Log("CanGo: iteration " + n_iteration);
            }

            NodePath current = toCheck.Dequeue();
            if (current.x - 1 > 0)
            {
                if (GridMatrix[(int)startPosition.x - 1, (int)startPosition.y, (int)startPosition.z] == null)
                {
                    int heightDown = CheckUnder(current.x - 1, current.y, current.z, jumpValue);
                    NodePath newNode = new NodePath(current.x - 1, current.y, heightDown, current.DistanceFromStart, current);
                    toCheck.Enqueue(newNode);
                    accessibleBloc.Add(newNode);
                }
                List<int> HeigthsUp = CheckUp(current.x - 1, current.y, current.z, current.x, current.y, jumpValue);
                foreach (int z in HeigthsUp)
                {
                    NodePath newNode = new NodePath(current.x - 1, current.y, z, current.DistanceFromStart, current);
                    toCheck.Enqueue(newNode);
                    accessibleBloc.Add(newNode);
                }
            }
            if (current.x + 1 > 0)
            {
                if (GridMatrix[(int)startPosition.x + 1, (int)startPosition.y, (int)startPosition.z] == null)
                {
                    int heightDown = CheckUnder(current.x + 1, current.y, current.z, jumpValue);
                    NodePath newNode = new NodePath(current.x + 1, current.y, heightDown, current.DistanceFromStart, current);
                    toCheck.Enqueue(newNode);
                    accessibleBloc.Add(newNode);
                }
                List<int> HeigthsUp = CheckUp(current.x + 1, current.y, current.z, current.x, current.y, jumpValue);
                foreach (int z in HeigthsUp)
                {
                    NodePath newNode = new NodePath(current.x + 1, current.y, z, current.DistanceFromStart, current);
                    toCheck.Enqueue(newNode);
                    accessibleBloc.Add(newNode);
                }
            }
        }
        
        return accessibleBloc;
    }

    /// <summary>
    /// Update position of every bloc of the grid
    /// </summary>
    public void UpdatePosition()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (gridMatrix[x, y, z] != null)
                    {
                        gridMatrix[x, y, z].gameObject.transform.position = new Vector3(x, y, z);

                    }
                }
            }
        }
    }

 

    /// <summary>
    /// Initialize value of boolean explored of blocs of the grid
    /// </summary>
    public void InitializeExplored(bool value)
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (gridMatrix[x, y, z] != null)
                    {
                        gridMatrix[x, y, z].Explored = value;
                    }
                }
            }
        }

    }
    /// <summary>
    /// Algorithm of deep research for related gravity
    /// Exploration is ok, but part for falling if no gravity is missing
    /// </summary>
    /// <param name="positionEdgeX"></param>
    /// <param name="positionEdgeY"></param>
    /// <param name="positionEdgeZ"></param>
	public void Explore(int positionEdgeX, int positionEdgeY, int positionEdgeZ)
    {
        if (!gridMatrix[positionEdgeX, positionEdgeY, positionEdgeZ].Explored)
        {
            gridMatrix[positionEdgeX, positionEdgeY, positionEdgeZ].Explored = true;
            // considering the 4 side blocs, the up bloc and down bloc... so 6 sides ?
            // if neighbours are not null and not explored, and still in the map
            if (positionEdgeX > 0 && gridMatrix[positionEdgeX - 1, positionEdgeY, positionEdgeZ] != null &&
                !gridMatrix[positionEdgeX - 1, positionEdgeY, positionEdgeZ].Explored)
            {
                Explore(positionEdgeX - 1, positionEdgeY, positionEdgeZ);
            }
            if (positionEdgeX < sizeX - 1 && gridMatrix[positionEdgeX + 1, positionEdgeY, positionEdgeZ] != null &&
                !gridMatrix[positionEdgeX + 1, positionEdgeY, positionEdgeZ].Explored)
            {
                Explore(positionEdgeX + 1, positionEdgeY, positionEdgeZ);
            }
            if (positionEdgeY > 0 && gridMatrix[positionEdgeX, positionEdgeY - 1, positionEdgeZ] != null &&
                !gridMatrix[positionEdgeX, positionEdgeY - 1, positionEdgeZ].Explored)
            {
                Explore(positionEdgeX, positionEdgeY - 1, positionEdgeZ);
            }
            if (positionEdgeY < sizeY - 1 && gridMatrix[positionEdgeX, positionEdgeY + 1, positionEdgeZ] != null &&
                !gridMatrix[positionEdgeX, positionEdgeY + 1, positionEdgeZ].Explored)
            {
                Explore(positionEdgeX, positionEdgeY + 1, positionEdgeZ);
            }
            if (positionEdgeZ > 0 && gridMatrix[positionEdgeX, positionEdgeY, positionEdgeZ - 1] != null &&
                !gridMatrix[positionEdgeX, positionEdgeY, positionEdgeZ - 1].Explored)
            {
                Explore(positionEdgeX, positionEdgeY, positionEdgeZ - 1);
            }
            if (positionEdgeZ < sizeZ - 1 && gridMatrix[positionEdgeX, positionEdgeY, positionEdgeZ + 1] != null &&
                !gridMatrix[positionEdgeX, positionEdgeY, positionEdgeZ + 1].Explored)
            {
                Explore(positionEdgeX, positionEdgeY, positionEdgeZ + 1);
            }
        }
    }

    /// <summary>
    /// Check if all the grid has been explored
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
                    if (gridMatrix[x, y, z] != null && !gridMatrix[x, y, z].Explored)
                    // if unexplored part, false 
                    {
                        return false;

                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Function called to handle all the gravity by herself
    /// </summary>
    public void Gravity()
    {
        SimpleGravity();

        // check at the creation that explored = false. 
        while (!IsGridAllExplored())
        {
            // starting by the one from the bottom that are not marked, then expanding. 
            //Then go to floaties, expand. 
            //Others will fall
            //  0 = not seen, nothing to do; 1 seen
            InitializeExplored(false);
            

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        if (gridMatrix[x, y, z] != null && !gridMatrix[x, y, z].Explored && (y == 0 || gridMatrix[x, y, z].GravityType == GravityType.NULL_GRAVITY))
                        // throw dfs on unempty bloc that are not explored, on floor or without gravity
                        {
                            Explore(x, y, z);

                        }


                    }
                }
            }
            //then, all blocs not empty with explored to false fall from s1
            FallConnexe();

        }
    }
    /// <summary>
    /// make blocs under related gravity fall (all non null unexplored)
    /// </summary>
    public void FallConnexe()
    {

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (gridMatrix[x, y, z] != null && !gridMatrix[x, y, z].Explored) // if bloc is not null and supposed to fall
                                                                          //then fall.
                    {

                        HandleCrush(x, y, z, 1); // falling from one, applying right crushing if needed
                    }
                }
            }
        }

    }
    public void DeplaceBloc(Placeable bloc, Vector3Int desiredPosition)
    {
        if (bloc != null && bloc.GetPosition() != desiredPosition && desiredPosition.x >= 0 && desiredPosition.x < sizeX
           && desiredPosition.y >= 0 && desiredPosition.y < sizeY
           && desiredPosition.z >= 0 && desiredPosition.z < sizeZ &&
         (gridMatrix[desiredPosition.x, desiredPosition.y, desiredPosition.z] == null ||
          gridMatrix[desiredPosition.x, desiredPosition.y, desiredPosition.z].Crushable!=CrushType.CRUSHSTAY))
        {
            Vector3 oldPosition = bloc.transform.position;

            gridMatrix[desiredPosition.x, desiredPosition.y, desiredPosition.z] = bloc;//adding a link
            gridMatrix[desiredPosition.x, desiredPosition.y, desiredPosition.z].transform.position += (desiredPosition - bloc.GetPosition());//shifting model
            gridMatrix[(int)oldPosition.x, (int)oldPosition.y, (int)oldPosition.z] = null;//put former place to 0
            bloc.RpcMoveOnClient(desiredPosition);

        }
    }
    /// <summary>
    /// Function handling vertical collisions of bloc by another
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="ydrop"></param>
    public void HandleCrush(int x, int y, int z, int ydrop)
    {
        if (gridMatrix[x, y - ydrop, z] == null)// copying and destroying
        {
            DeplaceBloc(gridMatrix[x, y, z], new Vector3Int(x, y - ydrop, z));
        }

        else if (gridMatrix[x, y - ydrop, z].Crushable == CrushType.CRUSHDESTROYBLOC)// destroy bloc, trigger effects
        {
            gridMatrix[x, y, z].DestroyLivingPlaceable();
            gridMatrix[x, y, z] = null;

        }
        else if (gridMatrix[x, y - ydrop, z].Crushable == CrushType.CRUSHLIFT)// copying and destroying
        {
            int ymontee = y;
            while (ymontee < sizeY && gridMatrix[x, ymontee, z] != null) // checking is not necessary in y though
            {
                ymontee++;
            }

            gridMatrix[x, ymontee, z] = gridMatrix[x, y - ydrop, z].Cloner();
            //gridMatrix[x, ymontee, z].Position.Set(x, ymontee, z);


            gridMatrix[x, y - ydrop, z] = gridMatrix[x, y, z].Cloner();
            //gridMatrix[x, y - ydrop, z].Position.Set(x, y - ydrop, z);
            gridMatrix[x, y, z] = null;
        }
        else if (gridMatrix[x, y - ydrop, z].Crushable == CrushType.CRUSHDEATH)
        {
            gridMatrix[x, y - ydrop, z].DestroyLivingPlaceable();
            gridMatrix[x, y - ydrop, z] = gridMatrix[x, y, z].Cloner();
           // gridMatrix[x, y - ydrop, z].Position.Set(x, y - ydrop, z);
            gridMatrix[x, y, z] = null;

        }



        //could be a crushable of type crushstay, or empty

    }
    /// <summary>
    /// Handle gravity for object of type SIMPLE_GRAVITY. if nothing below => fall
    /// </summary>
    public void SimpleGravity()
    {
        int y = 0;
        while (y < sizeY)
        {
            for (int x = 0; x < sizeX; x++)
            {

                for (int z = 0; z < sizeZ; z++)
                {
                    if (gridMatrix[x, y, z] != null && gridMatrix[x, y, z].GravityType == GravityType.SIMPLE_GRAVITY)
                    {
                        int ydrop = 0;

                        while (y - ydrop > 0 && (gridMatrix[x, y - ydrop - 1, z] == null
                            || (gridMatrix[x, y - ydrop - 1, z].Crushable != CrushType.CRUSHSTAY)))

                        {
                            ydrop++;
                        }
                        if (ydrop > 0)
                        {
                            HandleCrush(x, y, z, ydrop);




                        }


                    }
                }
            }
            y++;
        }
    }
    /// <summary>
    /// Save grid in json file using jaggedarray
    /// </summary>
    public void SaveGridFile()
    {
        JaggedGrid jagged = new JaggedGrid();
        jagged.ToJagged(this);
        jagged.Save();

    }



    void Awake()
    {

        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        gridBool = new DistanceAndParent[sizeX, sizeY, sizeZ];
        gridMatrix = new Placeable[sizeX, sizeY, sizeZ];



    }

    /// <summary>
    /// Precondition: empty grid never filled
    /// </summary>
    /// <param name="grid"></param>
    public void FillGridAndSpawn(GameObject parent)
    {
        JaggedGrid jagged = JaggedGrid.FillGridFromJSON();

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (jagged.gridTable[y * sizeZ * sizeX + z * sizeX + x] != 0) //assuming grid was empty and never filled
                    {

                        GameObject obj = Instantiate(GameManager.instance.networkManager.spawnPrefabs[jagged.gridTable[y * sizeZ * sizeX + z * sizeX + x] - 1],
                            new Vector3(x, y, z), Quaternion.identity, parent.transform);
                        

                        gridMatrix[x, y, z] = obj.GetComponent<Placeable>(); //we're not interested in the gameObject
                        NetworkServer.Spawn(obj);


                    }

                }
            }
        }
    }

}
