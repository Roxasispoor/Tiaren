﻿using System;
using System.Collections.Generic;
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
        return new NodePath(x, y-1, z, 0, null);
    }

    public Vector3[] GetFullPath()
    {
        Vector3[] path = new Vector3[DistanceFromStart + 1];
        NodePath currentNode = this;
        for (int i = DistanceFromStart; i >= 0; i--)
        {
            path[i] = new Vector3(currentNode.x, currentNode.y, currentNode.z);
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
        if (obj.GetType() != typeof(NodePath))
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
    //public DistanceAndParent[,,] gridBool;


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
                    if (UnityEngine.Random.Range(0, 100) < randomParameter && y < sizeY - 2) // second condition could be in fory 
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
    /// TODO
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool VerifyPath(Vector3[] path)
    {
        return true;
    }


    /// <summary>
    /// Function finding and displaying path from point A to point B
    /// Only cases where the character can go are added
    /// <param name="livingPlaceable">Character to study</param>
    /// <param name="jumpValue">Value of jump for the character</param>
    /// <param name="positionBloc">Position of starting bloc (under the character)</param>
    /// <returns>Returns a grid which can give positions of every cube on the path (belonging to floor). Character will be set on top of the cube target</returns>
    /// </summary>
    /* public DistanceAndParent[,,] CanGo(LivingPlaceable livingPlaceable, int jumpValue, Vector3Int positionBloc)
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
                    GameManager.instance.PlayingPlaceable.Player.MakeCubeBlue(testa.netId);
                  
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
    */
    /// <summary>
    /// Instanciate the new cube
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    public void InstantiateCube(GameObject prefab, Vector3Int position)
      {
          if (CheckNull(position))
          {
              GameObject newBlock = Instantiate(prefab, GameManager.instance.gridFolder.transform);
              gridMatrix[position.x, position.y, position.z] = newBlock.GetComponent<Placeable>();
              MeshFilter meshFilter = newBlock.GetComponent<MeshFilter>();

              if (meshFilter != null)
              {
                  CombineInstance currentInstance = new CombineInstance
                  {
                      mesh = newBlock.GetComponent<MeshFilter>().sharedMesh,
                      transform = meshFilter.transform.localToWorldMatrix
                  };

                  GameManager.instance.AddMeshToBatches(meshFilter, currentInstance);
              }
          }

      }
    public bool CheckNull(Vector3Int position)
      {
          return CheckRange(position) && gridMatrix[position.x, position.y, position.z] != null;
      }
      public bool CheckRange(Vector3Int position)
      {
          return position.x > 0 && position.x<sizeX &&
              position.y> 0 && position.y<sizeY &&
              position.z> 0 && position.z<sizeZ;
      }


    private int CheckUnder(int x, int y, int z, int jumpValue) // z correspond à la hauteur du bloc sur lequel marche le joueur
    {
        for (int i = 0; i < jumpValue + 1; i++)
        {
            if (y - i < 0)
                return -1;

            if (GridMatrix[x, y - i, z] != null)
            {
                if (GridMatrix[x, y - i, z].Walkable)
                {
                    return y - i;
                }
                else
                {
                    return -1;
                }
            }
        }
        return -1;
    }

    private List<int> CheckUp(int x, int y, int z, int x_orig, int z_orig, int jumpValue) // z correspond à la hauteur du bloc sur lequel marche le joueur
    {
        List<int> returnList = new List<int>();

        for (int i = 1; i < jumpValue + 1; i++)
        {
            if (y + i > sizeY)
                return returnList;
            if (y + i + 1 < sizeY && GridMatrix[x_orig, y + i + 1, z_orig] != null)
            {
                return returnList;
            }
            if (GridMatrix[x, y + i, z] != null)
            {
                if (GridMatrix[x, y + i, z].Walkable)
                {
                    returnList.Add(y + i);
                }
                else
                {
                    return returnList;
                }
            }
        }
        return returnList;
    }

    void CheckColumn(NodePath current, int shift_x, int shift_y, int shift_z, Queue<NodePath> toCheck, ref List<NodePath> accessibleBloc, int distance, int jumpValue)
    {
        if (GridMatrix[current.x + shift_x, current.y + shift_y + 1, current.z + shift_z] == null)
        {
            int heightDown = CheckUnder(current.x + shift_x, current.y + shift_y, current.z + shift_z, jumpValue);
            if (heightDown >= 0)
            {
                NodePath newNode = new NodePath(current.x + shift_x, heightDown, current.z + shift_z, current.DistanceFromStart + 1, current);
                if (newNode.DistanceFromStart < distance)
                    toCheck.Enqueue(newNode);
                accessibleBloc.Add(newNode);
            }
            
        }
        List<int> HeigthsUp = CheckUp(current.x + shift_x, current.y + shift_y, current.z + shift_z, current.x, current.z, jumpValue);
        foreach (int y in HeigthsUp)
        {
            NodePath newNode = new NodePath(current.x + shift_x, y, current.z + shift_z, current.DistanceFromStart + 1, current);
            if (newNode.DistanceFromStart < distance)
                toCheck.Enqueue(newNode);
            accessibleBloc.Add(newNode);
        }
    }

    // startPosition : position du joueur
    /// <summary>
    /// Function finding all the block accessible by walking from "startPosition" to a max distance of "distance"
    /// <param name="startPosition">current position of the character/object who need to move</param>
    /// <param name="distance">distance max it can move</param>
    /// <param name="jumpValue">Value of jump for the character</param>
    /// <param name="player">(Optionnal) The team the object is if necessary</param>
    /// <returns>Returns a list of "nodePath" corresponding to all the accessible blocs</returns>
    /// </summary>
    public List<NodePath> CanGo(Vector3 startPosition, int distance, int jumpValue, Player player = null)
    {
        Queue<NodePath> toCheck = new Queue<NodePath>();
        List<NodePath> accessibleBloc = new List<NodePath>();

        if (distance <= 0)
            return accessibleBloc;

        toCheck.Enqueue(NodePath.startPath(startPosition));

        int n_iteration = 0;

        while (toCheck.Count > 0)
        {
            n_iteration++;

            if (n_iteration % 1000 == 0)
            {
                Debug.Log("CanGo: iteration " + n_iteration);
            }

            NodePath current = toCheck.Dequeue();
            if (current.x - 1 >= 0)
            {
                CheckColumn(current, -1, 0, 0, toCheck, ref accessibleBloc, distance, jumpValue);
            }
            if (current.x + 1 < sizeX)
            {
                CheckColumn(current, +1, 0, 0, toCheck, ref accessibleBloc, distance, jumpValue);
            }
            if (current.z - 1 >= 0)
            {
                CheckColumn(current, 0, 0, -1, toCheck, ref accessibleBloc, distance, jumpValue);
            }
            if (current.z + 1 < sizeZ)
            {
                CheckColumn(current, 0, 0, +1, toCheck, ref accessibleBloc, distance, jumpValue);
            }
        }

        return accessibleBloc;
    }



    public bool CheckPath(Vector3[] path, LivingPlaceable currentCharacter) // The end is where the Character stand (under him)
    {
        Vector3 current = path[0];
        //TODO: rajouter le test de current is walkable
        for (int i = 1; i < path.Length; i++)
        {
            Vector3 next = path[i];
            //TODO: rajouter le test de next is walkable
            Vector3 diff = next - current;
            if (Mathf.Abs(diff.x) == 1 ^ Mathf.Abs(diff.z) == 1)
            {
                
                int yTested = (int)current.y + 1;
                while (yTested < next.y + 1)
                {
                    if (Grid.instance.gridMatrix[(int)current.x, (int)yTested, (int)current.z] != currentCharacter
                         && Grid.instance.gridMatrix[(int)current.x, (int)yTested, (int)current.z] != null)
                    {
                        return false;
                    }
                    yTested++;
                }

                while (yTested > next.y)
                {
                    if (Grid.instance.gridMatrix[(int)next.x, (int)yTested, (int)next.z] != null)
                    {
                        return false;
                    }
                    yTested--;
                }
            }
            else return false;
            current = next;
        }
        return true;
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

    public void MoveBlock(Placeable bloc, Vector3Int desiredPosition,bool updateTransform=true)
    {
        if (bloc != null && bloc.GetPosition() != desiredPosition && desiredPosition.x >= 0 && desiredPosition.x < sizeX
           && desiredPosition.y >= 0 && desiredPosition.y < sizeY
           && desiredPosition.z >= 0 && desiredPosition.z < sizeZ &&
         (gridMatrix[desiredPosition.x, desiredPosition.y, desiredPosition.z] == null ||
          gridMatrix[desiredPosition.x, desiredPosition.y, desiredPosition.z].Crushable != CrushType.CRUSHSTAY))
        {
            Vector3 oldPosition = bloc.transform.position;

            gridMatrix[desiredPosition.x, desiredPosition.y, desiredPosition.z] = bloc;//adding a link
            if(updateTransform)
            {
                gridMatrix[desiredPosition.x, desiredPosition.y, desiredPosition.z].transform.position += (desiredPosition - bloc.GetPosition());//shifting model
            }
            gridMatrix[(int)oldPosition.x, (int)oldPosition.y, (int)oldPosition.z] = null;//put former place to 0

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
            MoveBlock(gridMatrix[x, y, z], new Vector3Int(x, y - ydrop, z));
        }
        /*
        else if (gridMatrix[x, y - ydrop, z].Crushable == CrushType.CRUSHDESTROYBLOC)// destroy bloc, trigger effects
        {
            gridMatrix[x, y, z].Destroy();
            gridMatrix[x, y, z] = null;

        }
        else if (gridMatrix[x, y - ydrop, z].Crushable == CrushType.CRUSHLIFT)// copying and destroying
        {
            int ymontee = y;
            while (ymontee < sizeY && gridMatrix[x, ymontee, z] != null) // checking is not necessary in y though
            {
                ymontee++;
            }

            gridMatrix[x, ymontee, z] = gridMatrix[x, y - ydrop, z];
            //gridMatrix[x, ymontee, z].Position.Set(x, ymontee, z);


            gridMatrix[x, y - ydrop, z] = gridMatrix[x, y, z].Cloner();
            //gridMatrix[x, y - ydrop, z].Position.Set(x, y - ydrop, z);
            gridMatrix[x, y, z] = null;
        }
        else if (gridMatrix[x, y - ydrop, z].Crushable == CrushType.CRUSHDEATH)
        {
            gridMatrix[x, y - ydrop, z].Destroy();
            gridMatrix[x, y - ydrop, z] = gridMatrix[x, y, z].Cloner();
            // gridMatrix[x, y - ydrop, z].Position.Set(x, y - ydrop, z);
            gridMatrix[x, y, z] = null;

        }
        */


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

        //gridBool = new DistanceAndParent[sizeX, sizeY, sizeZ];
        gridMatrix = new Placeable[sizeX, sizeY, sizeZ];



    }

    /// <summary>
    /// Precondition: empty grid never filled
    /// </summary>
    /// <param name="grid"></param>
    public void FillGridAndSpawn(GameObject parent)
    {
        Debug.Log("Load Map");
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
                        obj.GetComponent<Placeable>().netId = Placeable.currentMaxId;
                        GameManager.instance.idPlaceable[Placeable.currentMaxId] = obj.GetComponent<Placeable>();
                        Placeable.currentMaxId++;
                        // NetworkServer.Spawn(obj);


                    }

                }
            }
        }
        Debug.Log(Placeable.currentMaxId);

    }

    public List<Vector3Int> HighlightTargetableBlocks(Vector3 Playerposition, int minrange, int maxrange, bool dummy)
    {
        int remainingrangeYZ;
        int remainingrangeY;
        int dirx, diry, dirz;
        List<Vector3Int> targetableblocs = new List<Vector3Int>();
        dirx = 0;
        for (int i = 0; i <= maxrange; i++)
        {
            remainingrangeYZ = maxrange - i;
            int x = (int)Playerposition.x + i;
            if (x <= sizeX)
            {
                dirz = 0;
                for (int j = 0; j <= remainingrangeYZ; j++)
                {
                    if (i + j < minrange)
                    {
                        remainingrangeY = remainingrangeYZ + j;
                        int z = (int)Playerposition.z + j;
                        if (z <= sizeZ)
                        {
                            if (GridMatrix[x, (int)Playerposition.y, z] != null && GridMatrix[x, (int)Playerposition.y + 1, z] == null)
                            {
                                Vector3Int newblock = new Vector3Int(x, (int)Playerposition.y, z);
                                targetableblocs.Add(newblock);
                            }
                            diry = 0;
                            for (int k = 0; k < remainingrangeY; k++)
                            {
                                if (i + j + k < minrange)
                                {
                                    int y = (int)Playerposition.y + k;
                                    if (y <= sizeY)
                                    {
                                        if (!RayCastBlock(i, k, j, dirx, diry, dirz, Playerposition))
                                        {
                                            Vector3Int newblock = new Vector3Int(x, y, z);
                                            targetableblocs.Add(newblock);
                                        }
                                    }
                                }
                                diry = 1;
                            }
                            diry = -1;
                            for (int k = -2; k >= -remainingrangeY; k--)
                            {
                                if (i + j - k < minrange)
                                {
                                    int y = (int)Playerposition.y + k;
                                    if (y >= 0)
                                    {
                                        if (!RayCastBlock(i, k, j, dirx, diry, dirz, Playerposition))
                                        {
                                            Vector3Int newblock = new Vector3Int(x, y, z);
                                            targetableblocs.Add(newblock);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    dirz = 1;
                }
                dirz = -1;
                for (int j = -1; j >= -remainingrangeYZ; j--)
                {
                    if (i - j < minrange)
                    {
                        remainingrangeY = remainingrangeYZ - j;
                        int z = (int)Playerposition.z + j;
                        if (z >= 0)
                        {
                            if (GridMatrix[x, (int)Playerposition.y, z] != null && GridMatrix[x, (int)Playerposition.y + 1, z] == null)
                            {
                                Vector3Int newblock = new Vector3Int(x, (int)Playerposition.y, z);
                                targetableblocs.Add(newblock);
                            }
                            diry = 0;
                            for (int k = 0; k < remainingrangeY; k++)
                            {
                                if (i - j + k < minrange)
                                {
                                    int y = (int)Playerposition.y + k;
                                    if (y <= sizeY)
                                    {
                                        if (!RayCastBlock(i, k, j, dirx, diry, dirz, Playerposition))
                                        {
                                            Vector3Int newblock = new Vector3Int(x, y, z);
                                            targetableblocs.Add(newblock);
                                        }
                                    }
                                }
                                diry = 1;
                            }
                            diry = -1;
                            for (int k = -2; k >= -remainingrangeY; k--)
                            {
                                if (i - j - k < minrange)
                                {
                                    int y = (int)Playerposition.y + k;
                                    if (y >= 0)
                                    {
                                        if (!RayCastBlock(i, k, j, dirx, diry, dirz, Playerposition))
                                        {
                                            Vector3Int newblock = new Vector3Int(x, y, z);
                                            targetableblocs.Add(newblock);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            dirx = 1;
        }
        dirx = -1;
        for (int i = -1; i >= -maxrange; i++)
        {
            remainingrangeYZ = maxrange - i;
            int x = (int)Playerposition.x + i;
            if (x >= 0)
            {
                dirz = 0;
                for (int j = 0; j <= remainingrangeYZ; j++)
                {
                    if (-i + j < minrange)
                    {
                        remainingrangeY = remainingrangeYZ + j;
                        int z = (int)Playerposition.z + j;
                        if (z <= sizeZ)
                        {
                            if (GridMatrix[x, (int)Playerposition.y, z] != null && GridMatrix[x, (int)Playerposition.y + 1, z] == null)
                            {
                                Vector3Int newblock = new Vector3Int(x, (int)Playerposition.y, z);
                                targetableblocs.Add(newblock);
                            }
                            diry = 0;
                            for (int k = 0; k < remainingrangeY; k++)
                            {
                                if (-i + j + k < minrange)
                                {
                                    int y = (int)Playerposition.y + k;
                                    if (y <= sizeY)
                                    {
                                        if (!RayCastBlock(i, k, j, dirx, diry, dirz, Playerposition))
                                        {
                                            Vector3Int newblock = new Vector3Int(x, y, z);
                                            targetableblocs.Add(newblock);
                                        }
                                    }
                                }
                                diry = 1;
                            }
                            diry = -1;
                            for (int k = -2; k >= -remainingrangeY; k--)
                            {
                                if (-i + j - k < minrange)
                                {
                                    int y = (int)Playerposition.y + k;
                                    if (y >= 0)
                                    {
                                        if (!RayCastBlock(i, k, j, dirx, diry, dirz, Playerposition))
                                        {
                                            Vector3Int newblock = new Vector3Int(x, y, z);
                                            targetableblocs.Add(newblock);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    dirz = 1;
                }
                dirz = -1;
                for (int j = -1; j >= -remainingrangeYZ; j--)
                {
                    if (-i - j < minrange)
                    {
                        remainingrangeY = remainingrangeYZ - j;
                        int z = (int)Playerposition.z + j;
                        if (z >= 0)
                        {
                            if (GridMatrix[x, (int)Playerposition.y, z] != null && GridMatrix[x, (int)Playerposition.y + 1, z] == null)
                            {
                                Vector3Int newblock = new Vector3Int(x, (int)Playerposition.y, z);
                                targetableblocs.Add(newblock);
                            }
                            diry = 0;
                            for (int k = 0; k < remainingrangeY; k++)
                            {
                                if (-i - j + k < minrange)
                                {
                                    int y = (int)Playerposition.y + k;
                                    if (y <= sizeY)
                                    {
                                        if (!RayCastBlock(i, k, j, dirx, diry, dirz, Playerposition))
                                        {
                                            Vector3Int newblock = new Vector3Int(x, y, z);
                                            targetableblocs.Add(newblock);
                                        }
                                    }
                                }
                                diry = 1;
                            }
                            diry = -1;
                            for (int k = -2; k >= -remainingrangeY; k--)
                            {
                                if (-i - j - k < minrange)
                                {
                                    int y = (int)Playerposition.y + k;
                                    if (y >= 0)
                                    {
                                        if (!RayCastBlock(i, k, j, dirx, diry, dirz, Playerposition))
                                        {
                                            Vector3Int newblock = new Vector3Int(x, y, z);
                                            targetableblocs.Add(newblock);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return targetableblocs;

    }

    public List<Vector3Int> HighlightTargetableBlocks(Vector3 Playerposition, int minrange, int maxrange)
    {
        List<Vector3Int> targetableBlocks = new List<Vector3Int>();

        for (int x = Mathf.Max((int)Playerposition.x - maxrange, 0);
                x < Mathf.Min((int)Playerposition.x + maxrange + 1, Grid.instance.sizeX);
                x++)
        {
            for (int y = Mathf.Max((int)Playerposition.y - maxrange, 0);
                y < Mathf.Min((int)Playerposition.y + maxrange + 1, Grid.instance.sizeY);
                y++)
            {
                for (int z = Mathf.Max((int)Playerposition.z - maxrange, 0);
                z < Mathf.Min((int)Playerposition.z + maxrange + 1, Grid.instance.sizeZ);
                z++)
                {
                    if (gridMatrix[x, y, z] != null 
                        && !gridMatrix[x, y, z].IsLiving() 
                        && (y == sizeY-1 || gridMatrix[x,y+1,z] == null))
                    {
                        targetableBlocks.Add(new Vector3Int(x, y, z));
                    }
                }
            }
        }

        return targetableBlocks;
    }

        public bool RayCastBlock(int x, int y, int z, int dirx, int diry, int dirz, Vector3 Playerposition)
    {
        /*Vector3 playerside;
        Vector3 blockside;
        if (GridMatrix[(int)Playerposition.x + dirx, (int)Playerposition.y +diry, (int)Playerposition.z + dirz] != null)
        {
            if (dirx != 0) Playerposition.x -= dirx / 2;
            if (diry != 0) Playerposition.y -= diry / 2;
            if (dirz != 0) Playerposition.z -= dirz / 2;
        }
        if (GridMatrix[x + dirx, y + diry, z + dirz] != null)
        {

        }
        playerside = new Vector3(Playerposition.x + dirx * 0.5f, Playerposition.y + diry * 0.5f, Playerposition.z + dirz * 0.5f);
        blockside = new Vector3(x - dirx * 0.5f, y - diry * 0.5f, z - dirz * 0.5f);
        if (Physics.Raycast(playerside,blockside, Vector3.Distance(playerside,blockside)-0.2f)) {
            return false;
        }*/

        return false;
    }


    public List<LivingPlaceable> HighlightTargetableLiving(Vector3 Playerposition, int minrange, int maxrange)
    {
        List<LivingPlaceable> targetableliving = new List<LivingPlaceable>();

        //TODO
        foreach (GameObject gameObjCharacter in GameManager.instance.player1.GetComponent<Player>().Characters)
        {
            Vector3 distance = gameObjCharacter.GetComponent<LivingPlaceable>().GetPosition() - Playerposition;
            distance.x = Mathf.Abs(distance.x);
            distance.y = Mathf.Abs(distance.y);
            distance.z = Mathf.Abs(distance.z);
            if (distance.x + distance.y + distance.z <= maxrange
                && distance.x + distance.y + distance.z >= minrange)
            {
                targetableliving.Add(gameObjCharacter.GetComponent<LivingPlaceable>());
            }
        }

        foreach (GameObject gameObjCharacter in GameManager.instance.player2.GetComponent<Player>().Characters)
        {
            Vector3 distance = gameObjCharacter.GetComponent<LivingPlaceable>().GetPosition() - Playerposition;
            distance.x = Mathf.Abs(distance.x);
            distance.y = Mathf.Abs(distance.y);
            distance.z = Mathf.Abs(distance.z);
            if (distance.x + distance.y + distance.z <= maxrange
                && distance.x + distance.y + distance.z >= minrange)
            {
                targetableliving.Add(gameObjCharacter.GetComponent<LivingPlaceable>());
            }
        }

        return targetableliving;
    }
}
