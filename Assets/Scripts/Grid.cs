using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

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
    [SerializeField]
    private int falldamage = 15;
    [SerializeField]
    private int blockfalldamage = 20;
    //public DistanceAndParent[,,] gridBool;

    public bool UseAwakeLiving = false;
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

    public List<Vector3Int> spawnPlayer1;
    public List<Vector3Int> spawnPlayer2;

    public int FallDamage
    {
        get
        {
            return falldamage;
        }

        set
        {
            falldamage = value;
        }
    }

    public int BlockFallDamage
    {
        get
        {
            return blockfalldamage;
        }

        set
        {
            blockfalldamage = value;
        }
    }

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

    public List<Vector3Int> SpawnPlayer2
    {
        get
        {
            return spawnPlayer2;
        }

        set
        {
            spawnPlayer2 = value;
        }
    }

    public List<Vector3Int> SpawnPlayer1
    {
        get
        {
            return spawnPlayer1;
        }

        set
        {
            spawnPlayer1 = value;
        }
    }

    public Placeable GetPlaceableFromVector(Vector3Int pos)
    {
        if(CheckRange(pos))
        { 
        return GridMatrix[pos.x, pos.y, pos.z];
        }
        else
        {
            Debug.Log("Out of range in getplaceablefromvector"+ pos);
            return null;
        }
    }

    public Placeable GetPlaceableFromVector(Vector3 pos)
    {
        Vector3Int intpos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
        if (CheckRange(intpos))
        {
            return GridMatrix[intpos.x, intpos.y, intpos.z];
        }
        else
        {
            Debug.Log("Out of range in getplaceablefromvector" + pos);
            return null;
        }
    }

    /// <summary>
    /// Return the spawnlist for the given player
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public List<Vector3Int> GetSpawnPlayer (Player player)
    {
        if (player.gameObject == GameManager.instance.player1)
        {
            return SpawnPlayer1;
        }
        else
        {
            return SpawnPlayer2;
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
    public GameObject InstanciateObjectOnBloc(GameObject prefab,Vector3Int position)
    {
        GameObject newObjectOnBloc = Instantiate(prefab, Grid.instance.GridMatrix[position.x,position.y,position.z].gameObject.transform.Find("Inventory"));
        newObjectOnBloc.GetComponent<NetIdeable>().netId = NetIdeable.currentMaxId;
        NetIdeable.currentMaxId++;
        return newObjectOnBloc;
    }
    /// <summary>
    /// Instanciate the new cube
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    public void InstantiateCube(GameObject prefab, Vector3Int position)
    {
        if (!prefab.GetComponent<StandardCube>())
        {
            Debug.LogError("InstantiateCube: trying to instantiate a non-cube prefab: " + prefab.name);
        }
        if (CheckNull(position))
        {
            GameObject newBlock = Instantiate(prefab, new Vector3(position.x, position.y, position.z), Quaternion.identity, Grid.instance.transform);
            gridMatrix[position.x, position.y, position.z] = newBlock.GetComponent<Placeable>();

            Placeable blockUnder = GetPlaceableFromVector(position + Vector3Int.down);
            if (blockUnder && !blockUnder.IsLiving())
            {
                ((StandardCube)blockUnder).SomethingPutAbove();
            }
            
            newBlock.GetComponent<Placeable>().netId = Placeable.currentMaxId;
            GameManager.instance.idPlaceable[Placeable.currentMaxId] = newBlock.GetComponent<Placeable>();
            Placeable.currentMaxId++;

            if (GameManager.instance.isClient)
            {
                MeshFilter meshFilter = newBlock.GetComponent<MeshFilter>();

                if (meshFilter != null)
                {
                    CombineInstance currentInstance = new CombineInstance
                    {
                        mesh = newBlock.GetComponent<MeshFilter>().sharedMesh,
                        transform = meshFilter.transform.localToWorldMatrix
                    };

                    GameManager.instance.AddMeshToBatches(meshFilter, currentInstance);
                    newBlock.GetComponent<StandardCube>().meshInCombined = currentInstance;
                    GameManager.instance.RefreshBatch(newBlock.GetComponent<StandardCube>());
                }
            }
        }

    }

    /// <summary>
    /// Return true if the position is null in the grid.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool CheckNull(Vector3Int position)
    {
        return CheckRange(position) && (gridMatrix[position.x, position.y, position.z] == null);
    }

    public bool CheckRange(Vector3Int position)
    {
        return position.x >= 0 && position.x < sizeX 
            && position.y >= 0 && position.y < sizeY 
            && position.z >= 0 && position.z < sizeZ;
    }


    private int CheckUnder(int x, int y, int z, int jumpValue) // z correspond à la hauteur du bloc sur lequel marche le joueur
    {
        for (int i = 0; i < jumpValue + 1; i++)
        {
            if (y - i < 0)
                return -1;

            if (GridMatrix[x, y - i, z] != null)
            {
                if (GridMatrix[x, y - i, z].walkable)
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
            if (GridMatrix[x, y + i, z] != null && GridMatrix[x, y + i + 1, z] == null)
            {
                if (GridMatrix[x, y + i, z].walkable)
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
        Debug.Log("Accessible blocs : " + accessibleBloc.Count);
        return accessibleBloc;
    }



    public bool CheckPath(Vector3[] path, LivingPlaceable currentCharacter) // The end is where the Character stand (under him)
    {
        if (path == null || path.Length == 0)
            return true;
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
    /// Update position of every bloc of the grid Can be used for resynchrnisation
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
    /// Explore adjacent blocks in order too find those who are linked to the ground
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>if the block is linked to the ground</returns>
    public bool ExploreConnexity(int x, int y, int z)
    {
        //Debug.Log("Exploration du bloc :" + x + " " + y + " " + z);
        //Initialise variable connected, value is true if the curretn block is connected to the ground
        bool connected = false;
        //We've explored this block, so no need to check in again (avoiding loops)
        gridMatrix[x, y, z].explored = true;

        //first we check the block under us, if there is one
        if (y > 0 && gridMatrix[x, y - 1, z] != null && !gridMatrix[x, y - 1, z].IsLiving()) {
            //if it's the ground (level 0) to the current block is connected to the ground
            if (y - 1 == 0) return true;
            //if the block has no been explored, we check it
            if (!gridMatrix[x, y - 1, z].explored)
                connected = ExploreConnexity(x, y - 1, z);
            //else then we check if it's a floating block or a grounded one
            else connected = gridMatrix[x, y - 1, z].grounded;
            // then if we find something, no need to check the other block, let's return true
            if (connected)
            {
                gridMatrix[x, y, z].grounded = true;
                return connected;
            }
        }

        //And here is the same with other directions
        if (x + 1 < sizeX && gridMatrix[x + 1, y, z] != null && !gridMatrix[x+1, y, z].IsLiving()) {
            if (!gridMatrix[x + 1, y, z].explored)
                connected = ExploreConnexity(x + 1, y, z);
            else connected = gridMatrix[x + 1, y, z].grounded;
            if (connected)
            {
                gridMatrix[x, y, z].grounded = true;
                return connected;
            }
        }

        if (z + 1 < sizeZ && gridMatrix[x, y, z + 1] != null && !gridMatrix[x, y , z+1].IsLiving()) {
            if (!gridMatrix[x, y, z + 1].explored)
                connected = ExploreConnexity(x, y, z + 1);
            else connected = gridMatrix[x, y, z + 1].grounded;
        }
        if (connected)
        {
            gridMatrix[x, y, z].grounded = true;
            return connected;
        }

        if (x > 0 && gridMatrix[x - 1, y, z] != null && !gridMatrix[x-1, y, z].IsLiving()) {
            if (!gridMatrix[x - 1, y, z].explored)
                connected = ExploreConnexity(x - 1, y, z);
            else connected = gridMatrix[x - 1, y, z].grounded;
            if (connected)
            {
                gridMatrix[x, y, z].grounded = true;
                return connected;
            }
        }

        if (z > 0 && gridMatrix[x, y, z - 1] != null && !gridMatrix[x, y, z-1].IsLiving()) {
            if (!gridMatrix[x, y, z - 1].explored)
                connected = ExploreConnexity(x, y, z - 1);
            else connected = gridMatrix[x, y, z - 1].grounded;
            if (connected)
            {
                gridMatrix[x, y, z].grounded = true;
                return connected;
            }
        }

        if (y + 1 < sizeY && gridMatrix[x, y + 1, z] != null && !gridMatrix[x, y + 1, z].IsLiving()) {
            if (!gridMatrix[x, y + 1, z].explored)
                connected = ExploreConnexity(x, y + 1, z);
            else connected=gridMatrix[x, y + 1, z].grounded;
        }
        
        return connected;
    }

    /// <summary>
    /// Called when a block is destroyed or moved, check if some block needs to fall
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void ConnexeFall(int x, int y, int z)
    {
        //Debug.Log("Gravité Connexe appelée :" + x + " " + y + " " + z);
        bool somethingfall = false;
        //clearing the previous search
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                for (int k = 0; k < sizeZ; k++)
                {
                    if (gridMatrix[i,j,k]!=null)
                    {
                        gridMatrix[i, j, k].explored = false;
                        gridMatrix[i, j, k].grounded = false;
                    }
                }

            }
        }
        //no need to check this block back
        if (gridMatrix[x, y, z] != null)
            gridMatrix[x, y, z].explored = true;

        //if the block is in , it's the ground, this shouldn't have been destroyed or moved
        if (y != 0)
        {
            //for each direction, we check each block that need to fall due to the destruction of the current block
            if (y > 0 && gridMatrix[x, y - 1, z] != null && !gridMatrix[x, y - 1, z].IsLiving())
            {
                if (y - 1 == 0) gridMatrix[x, y - 1, z].grounded = true;
                else gridMatrix[x, y - 1, z].grounded = ExploreConnexity(x, y - 1, z);
                somethingfall = !gridMatrix[x, y - 1, z].grounded || somethingfall;
            }
            if (y + 1 < sizeY && gridMatrix[x, y + 1, z] != null && !gridMatrix[x, y + 1, z].IsLiving()) {
                gridMatrix[x, y + 1, z].grounded = ExploreConnexity(x, y + 1, z);
                somethingfall = somethingfall || !gridMatrix[x, y + 1, z].grounded;
            }
            if (x + 1 < sizeX && gridMatrix[x + 1, y, z] != null && !gridMatrix[x+1, y, z].IsLiving()) {
                gridMatrix[x + 1, y, z].grounded = ExploreConnexity(x + 1, y, z);
                somethingfall = somethingfall || !gridMatrix[x+1, y, z].grounded;
            }
            if (x > 0 && gridMatrix[x - 1, y, z] != null && !gridMatrix[x-1, y , z].IsLiving()) { 
                gridMatrix[x - 1, y, z].grounded = ExploreConnexity(x - 1, y, z);
                somethingfall = somethingfall || !gridMatrix[x-1, y, z].grounded;
            }
            if (z + 1 < sizeZ && gridMatrix[x, y, z + 1] != null && !gridMatrix[x, y, z+1].IsLiving()) { 
                gridMatrix[x, y, z + 1].grounded = ExploreConnexity(x, y, z + 1);
                somethingfall = somethingfall || !gridMatrix[x, y, z+1].grounded;
            }
            if (z > 0 && gridMatrix[x, y, z - 1] != null && !gridMatrix[x, y, z-1].IsLiving()) {
                gridMatrix[x, y, z - 1].grounded = ExploreConnexity(x, y, z - 1);
                somethingfall = somethingfall || !gridMatrix[x, y, z-1].grounded;
            }
        }
    }

    public void MovePlaceable(Placeable bloc, Vector3Int desiredPosition,bool updateTransform=true)
    {
        if (bloc != null && bloc.GetPosition() != desiredPosition && desiredPosition.x >= 0 && desiredPosition.x < sizeX
           && desiredPosition.y >= 0 && desiredPosition.y < sizeY
           && desiredPosition.z >= 0 && desiredPosition.z < sizeZ &&
         (gridMatrix[desiredPosition.x, desiredPosition.y, desiredPosition.z] == null ||
          gridMatrix[desiredPosition.x, desiredPosition.y, desiredPosition.z].Crushable != CrushType.CRUSHSTAY))
        {
            Vector3 oldPosition = bloc.GetPosition();

            gridMatrix[desiredPosition.x, desiredPosition.y, desiredPosition.z] = bloc;//adding a link
            if (true)
            {
                gridMatrix[(int)oldPosition.x, (int)oldPosition.y, (int)oldPosition.z] = null;//put former place to null
            }

            if (updateTransform)
            {
                if(bloc.isMoving)
                {
                    if(bloc.IsLiving())
                    {
                        Animator anim = bloc.gameObject.GetComponent<Animator>();
                        anim.SetTrigger("idle");
                    }
                    bloc.isMoving = false;
                    StopCoroutine(bloc.moveCoroutine);
                }
                bloc.transform.position = desiredPosition; //shifting model
            }
            if(!bloc.IsLiving()
                && GetPlaceableFromVector(desiredPosition + Vector3Int.down) != null
                && !GetPlaceableFromVector(desiredPosition + Vector3Int.down).IsLiving())
            {
                ((StandardCube)GetPlaceableFromVector(desiredPosition + Vector3Int.down)).SomethingPutAbove();
            }

        }
        else
        {
            Debug.LogError("MoveBlock error: To define");
        }
    }

    public void SwitchPlaceable(Placeable placeableA, Placeable placeableB)
    {
        if (placeableA == null)
        {
            Debug.LogError("SwitchPlaceable: first placeable is null");
        }
        if (placeableA == null)
        {
            Debug.LogError("SwitchPlaceable: second placeable is null");
        }

        Vector3Int oldPositionA = placeableA.GetPosition();
        Vector3Int oldPositionB = placeableB.GetPosition();

        Grid.instance.gridMatrix[oldPositionB.x, oldPositionB.y, oldPositionB.z] = placeableA;
        placeableA.transform.position = oldPositionB;

        Grid.instance.gridMatrix[oldPositionA.x, oldPositionA.y, oldPositionA.z] = placeableB;
        placeableB.transform.position = oldPositionA;
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
        if (gridMatrix[x, y, z].IsLiving())
        {
            LivingPlaceable Character = (LivingPlaceable)gridMatrix[x, y, z];
            int damage = Math.Max(ydrop - Character.Jump, 0) * falldamage;
            if (damage > 0)
            {
                EffectManager.instance.DirectAttack(new Damage(Character, gridMatrix[x, y - ydrop, z], Math.Max(ydrop - Character.Jump, 0) * falldamage));
            }
        }

        if (gridMatrix[x, y - ydrop, z] == null)// copying and destroying
        {
            MovePlaceable(gridMatrix[x, y, z], new Vector3Int(x, y - ydrop, z));
        }
        else if (gridMatrix[x, y - ydrop, z].Crushable == CrushType.CRUSHDAMAGE)
        {
            EffectManager.instance.DirectAttack(new Damage((LivingPlaceable)gridMatrix[x, y - ydrop, z], gridMatrix[x, y, z], blockfalldamage * ydrop));
            gridMatrix[x, y, z].Destroy(); 
            gridMatrix[x, y, z] = null;
        }
    }

    /// <summary>
    /// Handle all the gravity
    /// </summary>
    public void Gravity(int x, int y, int z)
    {
        ConnexeFall(x, y, z);
        simpleGravity();
    }
    
    public void simpleGravity()
    {
        int y = 0;
        //bool blockfallen = false;
        //List<Placeable> batchlist = new List<Placeable>();
        while (y < sizeY)
        {
            for (int x = 0; x < sizeX; x++)
            {

                for (int z = 0; z < sizeZ; z++)
                {
                    if (gridMatrix[x, y, z] != null &&
                       (gridMatrix[x, y, z].GravityType == GravityType.SIMPLE_GRAVITY ||
                       (gridMatrix[x, y, z].explored && !gridMatrix[x, y, z].grounded)))
                    {
                        //batchlist.Add(gridMatrix[x, y, z]);
                        //blockfallen = true;
                        if (!gridMatrix[x, y, z].IsLiving()) GameManager.instance.RemoveBlockFromBatch((StandardCube)gridMatrix[x, y, z]);
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
    public void SaveGridNetwork()
    {
        NetworkJaggedGrid jagged = new NetworkJaggedGrid();
        jagged.ToJagged(this);
        jagged.Save("NewGrid.json");

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
        



    }
    public void FillGridAndSpawnNetwork(GameObject parent, string json)
    {
        Debug.Log("Load Map");
        NetworkJaggedGrid jagged = JsonUtility.FromJson<NetworkJaggedGrid>(json);

        sizeX = jagged.sizeX;
        sizeY = jagged.sizeY;
        sizeZ = jagged.sizeZ;
        int maxfound = -1;
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (jagged.gridTable[y * sizeZ * sizeX + z * sizeX + x] != 0 
                        && (prefabsList[jagged.gridTable[y * sizeZ * sizeX + z * sizeX + x] - 1].GetComponent<Placeable>()== null ||
                        !prefabsList[jagged.gridTable[y * sizeZ * sizeX + z * sizeX + x] - 1].GetComponent<Placeable>().IsLiving())) //not zero, and is either not a placeable, or not a living one
                    {
                       /* Debug.Log("Instanciating prefab numberd" + (y * sizeZ * sizeX + z * sizeX + x));
                        Debug.Log("Its associated numberpprefab is" + jagged.gridTable[y * sizeZ * sizeX + z * sizeX + x]);*/
                        GameObject obj = Instantiate(prefabsList[jagged.gridTable[y * sizeZ * sizeX + z * sizeX + x] - 1],
                            new Vector3(x, y, z), Quaternion.identity, parent.transform);

                        gridMatrix[x, y, z] = obj.GetComponent<Placeable>(); //we're not interested in the gameObject
                        obj.GetComponent<Placeable>().netId = jagged.netId[y * sizeZ * sizeX + z * sizeX + x];
                        if(maxfound < jagged.netId[y * sizeZ * sizeX + z * sizeX + x])
                        {
                            maxfound = jagged.netId[y * sizeZ * sizeX + z * sizeX + x];
                        }


                        GameManager.instance.idPlaceable[obj.GetComponent<Placeable>().netId] = obj.GetComponent<Placeable>();


                    }
                }
            }
        }
        SpawnPlayer1 = jagged.GetSpawnsP1();
        SpawnPlayer2 = jagged.GetSpawnsP2();
        Placeable.currentMaxId=Mathf.Max(Placeable.currentMaxId,maxfound+1);
    }


    /// <summary>
    /// Precondition: empty grid never filled
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="pathJson"> The path to the map in Json</param>
    public void CreareGrid(GameObject parent, string pathJson)
    {
        Debug.Log("Load Map");
        JaggedGrid jagged = JaggedGrid.FillGridFromJSON(pathJson);

        sizeX = jagged.sizeX;
        sizeY = jagged.sizeY;
        sizeZ = jagged.sizeZ;
        //Debug.Log(sizeX + " " + sizeY + " " + sizeZ);
        gridMatrix = new Placeable[sizeX, sizeY, sizeZ];
        //gridMatrix = new Placeable[sizeX, sizeY, sizeZ];

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (jagged.gridTable[y * sizeZ * sizeX + z * sizeX + x] != 0) //assuming grid was empty and never filled
                    {
                        GameObject obj;
                        if (y == 0)
                        {
                            obj = Instantiate(prefabsList[13], // bedrock
                                   new Vector3(x, y, z), Quaternion.identity, parent.transform);
                            obj.GetComponent<StandardCube>().Destroyable = false;
                        }
                        else
                        {
                            obj = Instantiate(prefabsList[jagged.gridTable[y * sizeZ * sizeX + z * sizeX + x] - 1],
                                   new Vector3(x, y, z), Quaternion.identity, parent.transform);
                        }
                        //Debug.Log(x + "-" + y + "-" + z);
                        gridMatrix[x, y, z] = obj.GetComponent<Placeable>(); //we're not interested in the gameObject
                        obj.GetComponent<NetIdeable>().netId = Placeable.currentMaxId;
                        GameManager.instance.idPlaceable[Placeable.currentMaxId] = obj.GetComponent<Placeable>();

                        Placeable.currentMaxId++;
                        // NetworkServer.Spawn(obj);


                    }

                }
            }
        }

        Vector3Int posFlag = jagged.GetFlagPos() + Vector3Int.down;

        GameObject flag = Instantiate(Grid.instance.prefabsList[3], 
            GetPlaceableFromVector(posFlag).gameObject.transform.Find("Inventory"));
        flag.GetComponent<NetIdeable>().netId = NetIdeable.currentMaxId;
        NetIdeable.currentMaxId++;

        foreach (Vector3Int coord in jagged.GetGoalsP1())
        {
            GameObject goal = Instantiate(Grid.instance.prefabsList[4], coord, Quaternion.identity, transform);
            goal.GetComponent<NetIdeable>().netId = NetIdeable.currentMaxId;
            Grid.instance.GridMatrix[coord.x, coord.y, coord.z] = goal.GetComponent<Placeable>();
            goal.GetComponent<Placeable>().Player = GameManager.instance.player1.GetComponent<Player>();
            if (GameManager.instance.Player1 == GameManager.instance.GetLocalPlayer())
            {
                goal.GetComponent<MeshRenderer>().material = GameManager.instance.spawnAllyMaterial;
            }
            else
            {
                goal.GetComponent<MeshRenderer>().material = GameManager.instance.spawnEnemyMaterial;
            }
            NetIdeable.currentMaxId++;
        }

        foreach (Vector3Int coord in jagged.GetGoalsP2())
        {
            GameObject goal = Instantiate(Grid.instance.prefabsList[4], coord, Quaternion.identity, transform);
            goal.GetComponent<NetIdeable>().netId = NetIdeable.currentMaxId;
            Grid.instance.GridMatrix[coord.x, coord.y, coord.z] = goal.GetComponent<Placeable>();
            goal.GetComponent<Placeable>().Player = GameManager.instance.player2.GetComponent<Player>();
            if (GameManager.instance.Player2 == GameManager.instance.GetLocalPlayer())
            {
                goal.GetComponent<MeshRenderer>().material = GameManager.instance.spawnAllyMaterial;
            } else
            {
                goal.GetComponent<MeshRenderer>().material = GameManager.instance.spawnEnemyMaterial;
            }
            NetIdeable.currentMaxId++;
        }

        SpawnPlayer1 = jagged.GetSpawnsP1();
        SpawnPlayer2 = jagged.GetSpawnsP2();
        Debug.Log("CurrentMaxId: " + Placeable.currentMaxId);

    }

    /// <summary>
    /// Return a list of blocks i can target with a skill
    /// </summary>
    /// <param name="Playerposition"></param>
    /// <param name="minrange"></param>
    /// <param name="maxrange"></param>
    /// <param name="throughtblocks"></param>
    /// <param name="highrange"></param>
    /// <param name="viewRange"> True for physical attacks to show the range on the floor</param>
    /// <returns></returns>
    public List<Vector3Int> FindTargetableBlocks(Vector3 Playerposition, Skill skill, bool viewRange)
    {
        //get usefull things from skill
        int minrange = skill.MinRange;
        int maxrange = skill.MaxRange;
        bool throughblocks = skill.Throughblocks;
        bool highrange = minrange > 0 ? true : false;
        Vector3 shift = viewRange ? new Vector3(0, 1, 0) : new Vector3(0, 0, 0);
        int remainingrangeYZ; //remaining range
        int dirx; //x direction : 0 / -1 / 1
        List<Vector3Int> targetableblocs = new List<Vector3Int>();

        //Case x = 0 exploration 
        Depthreading(Playerposition, targetableblocs, minrange, maxrange, maxrange, 0, 0, throughblocks, highrange, viewRange, shift);

        //Now case when x > 0
        dirx = 1;
        for (int i = 1; i <= maxrange; i++)
        {
            //Updating remaining range  
            remainingrangeYZ = maxrange - i;

            //Targeted block position
            int x = (int)Playerposition.x + i;
            if (x < sizeX)
            {
                //exploring in y and z
                if (skill.SquareShaped)
                {
                    Depthreading(Playerposition, targetableblocs, minrange, maxrange, maxrange, i, dirx, throughblocks, highrange, viewRange, shift);
                }
                else
                {
                    Depthreading(Playerposition, targetableblocs, minrange, maxrange, remainingrangeYZ, i, dirx, throughblocks, highrange, viewRange, shift);
                }
            }
        }

        //Case x < 0 
        dirx = -1;
        for (int i = -1; i >= -maxrange; i--)
        {
            remainingrangeYZ = maxrange + i;
            int x = (int)Playerposition.x + i;
            if (x >= 0)
            {
                //Exploring in y and z                 
                if (skill.SquareShaped)
                {
                    Depthreading(Playerposition, targetableblocs, minrange, maxrange, maxrange, i, dirx, throughblocks, highrange, viewRange, shift);
                }
                else
                {
                    Depthreading(Playerposition, targetableblocs, minrange, maxrange, remainingrangeYZ, i, dirx, throughblocks, highrange, viewRange, shift);
                }
            }
        }

        //Removing block where is standing the player, if it has been selected
        targetableblocs.Remove(new Vector3Int((int)Playerposition.x, (int)Playerposition.y, (int)Playerposition.z));
        return targetableblocs;

    }


    /// <summary>
    /// Use raycast to determine if the character can see a block and add the good ones to the targetableblocs list
    /// </summary>
    /// <param name="Playerposition"></param>
    /// <param name="targetableblocs">List of blocs position</param>
    /// <param name="minrange">Minimal range of the skill</param>
    /// <param name="remainingrange">Remaining range of the skill (without x and z)</param>
    /// <param name="i">number of block on x axis</param>
    /// <param name="j">number of block on z axis</param>
    /// <param name="dirx">x direction (0,-1,1)</param>
    /// <param name="dirz">z direction (0,-1,1)</param>
    private void Highreading(Vector3 Playerposition, List<Vector3Int> targetableblocs, int minrange, int maxrange, int remainingrange,
        int i, int j, int dirx, int dirz, bool throughblocks, bool highrange, bool viewRange, Vector3 shift)
    {
        int diry = 0; //y direction
        //real block position
        int x = (int)Playerposition.x + i;
        int z = (int)Playerposition.z + j;

        if (!viewRange)
        {
            //Case y >= 0
            for (int k = 0; k <= remainingrange; k++)
            {
                if (Math.Abs(i) + Math.Abs(j) + k > minrange)
                {
                    //real block position
                    int y = (int)Playerposition.y + k;
                    if (y < sizeY)
                    {
                        //trying to see the targeted block, if true, adding it to the target list
                        if (GridMatrix[x, y, z] != null && (throughblocks|| !RayCastBlock(i, k, j, dirx, diry, dirz, Playerposition, shift)))
                        {
                            if (!gridMatrix[x, y, z].IsLiving())
                            {
                                Vector3Int newblock = new Vector3Int(x, y, z);
                                targetableblocs.Add(newblock);
                            }
                        }
                    }
                }
                //at the end of fonction because case y=0, allow one iteraction for 0
                diry = 1;
            }
        }

        //When y<0
        diry = -1;
        //Starting from -2, because not including footing
        for (int k = -2; k >= (highrange ? -maxrange+1 : -remainingrange); k--)
        {
            if (Math.Abs(i) + Math.Abs(j) - k > minrange)
            {
                //real block position
                int y = (int)Playerposition.y + k;
                if (y >= 0)
                {

                    //trying to see the targeted block, if true, adding it to the target list
                    if (GridMatrix[x, y, z] != null && (throughblocks|| !RayCastBlock(i, k, j, dirx, diry, dirz, Playerposition, shift)))
                    {
                        if (!gridMatrix[x, y, z].IsLiving())
                        {
                            Vector3Int newblock = new Vector3Int(x, y, z);
                            targetableblocs.Add(newblock);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called by HighlightTargetableBlocks. Used to search along the (x,z) plan some blocks to add in targetableblocs
    /// </summary>
    /// <param name="Playerposition"></param>
    /// <param name="targetableblocs">List of blocs position</param>
    /// <param name="minrange">Minimal range of the spell</param>
    /// <param name="remainingrange">Remaining range of the spell (without x)</param>
    /// <param name="i">number of block on x axis</param>
    /// <param name="dirx">x direction (0,-1,1)</param>
    private void Depthreading(Vector3 Playerposition, List<Vector3Int> targetableblocs, int minrange, int maxrange, int remainingrange,
        int i, int dirx, bool throughblocks, bool highrange, bool viewRange, Vector3 shift)
    {
        int dirz = 0; //z direction
        int trueremainingrange = remainingrange; //store the remainingrange at the algorithm's start
        //real block position
        int x = (int)Playerposition.x + i;

        //Case z >= 0
        for (int j = 0; j <= trueremainingrange; j++)
        {
            //new remaining range
            remainingrange = trueremainingrange - j;
            //real block position
            int z = (int)Playerposition.z + j;
            if (z < sizeZ)
            {
                //if in the good range interval
                if (Math.Abs(i) + j >= minrange && remainingrange >= 0)
                {
                    //search for the floor in range if in line of sight
                    if (GridMatrix[x, (int)Playerposition.y - 1, z] != null && (GridMatrix[x, (int)Playerposition.y, z] == null || GridMatrix[x, (int)Playerposition.y, z].IsLiving())
                        && (throughblocks || !RayCastBlock(i, -1, j, dirx, -1, dirz, Playerposition, shift)))
                    {
                        if (!gridMatrix[x, (int)Playerposition.y - 1, z].IsLiving())
                        {
                            Vector3Int newblock = new Vector3Int(x, (int)Playerposition.y - 1, z);
                            targetableblocs.Add(newblock);
                        }
                    }
                }
                Highreading(Playerposition, targetableblocs, minrange, maxrange, remainingrange, i, j, dirx, dirz, throughblocks, highrange, viewRange, shift);
            }
            dirz = 1;
        }

        //Case z < 0
        dirz = -1;
        for (int j = -1; j >= -trueremainingrange; j--)
        {
            //new remaining range
            remainingrange = trueremainingrange + j;
            //real block position
            int z = (int)Playerposition.z + j;
            if (z >= 0)
            {
                //if in the good range interval
                if (Math.Abs(i) - j >= minrange && remainingrange >= 0)
                {
                    //search for the floor in range if in line of sight and if so, add it to the bloc list
                    if (GridMatrix[x, (int)Playerposition.y - 1, z] != null && (GridMatrix[x, (int)Playerposition.y, z] == null || GridMatrix[x, (int)Playerposition.y, z].IsLiving())
                        && (throughblocks ||  !RayCastBlock(i, -1, j, dirx, -1, dirz, Playerposition, shift)))
                    {
                        if (!gridMatrix[x, (int)Playerposition.y - 1, z].IsLiving())
                        {
                            Vector3Int newblock = new Vector3Int(x, (int)Playerposition.y - 1, z);
                            targetableblocs.Add(newblock);
                        }
                    }
                }
                //Search on the y axis
                Highreading(Playerposition, targetableblocs, minrange, maxrange, remainingrange, i, j, dirx, dirz, throughblocks, highrange, viewRange, shift);
            }
        }
    }

    /// <summary>
    /// Check if a block can be seem and so targeted by the player
    /// </summary>
    /// <param name="x">block position relative to player</param>
    /// <param name="y">block position relative to player</param>
    /// <param name="z">block position relative to player</param>
    /// <param name="dirx">x direction</param>
    /// <param name="diry">x direction</param>
    /// <param name="dirz">x direction</param>
    /// <param name="Playerposition"></param>
    /// <returns>True if ther is a colision</returns>
    public bool RayCastBlock(int x, int y, int z, int dirx, int diry, int dirz, Vector3 Playerposition, Vector3 shift)
    {
        Vector3 playerside;
        Vector3 blockside;
        Vector3 blockside2;
        Vector3 blockside3;

        int layerMask = 1 << 9;
        layerMask = ~layerMask;
        //RaycastHit hit = new RaycastHit();

        //used to know how many axis are involved
        int activex = Math.Abs(dirx);
        int activey = Math.Abs(diry);
        int activez = Math.Abs(dirz);

        //Number of axis involved, the more, the more raycast i must shoot
        int sides = activex + activey + activez;

        switch (sides)
        {
            case 0:
                return false;

            //One axis = straight line, i only check for any obstacle on that line
            case 1:
                //facing the block
                playerside = new Vector3(Playerposition.x + dirx * 0.485f, Playerposition.y + diry * 0.485f, Playerposition.z + dirz * 0.485f);
                //facing the player
                blockside = new Vector3(Playerposition.x + x - dirx * 0.485f, Playerposition.y + y + shift.y - diry * 0.485f, Playerposition.z + z - dirz * 0.485f);

                //if the block is stuck to the player
                if (Vector3.Distance(playerside, blockside) < 0.1)
                    return false;
                else return (Physics.Raycast(playerside, blockside - playerside, Vector3.Distance(playerside, blockside) - 0.15f, layerMask));

            //2 axis = plane, i first determine which one it is, and then, pick to interest points on the block, and 2 on the player
            //i shoot (if necessary) a raycast from each player point to each block point, and check for any obsacle
            //if even one of them is free to go, then that mean i can target the block
            case 2:
                //check the x axis
                if (activex == 1)
                {
                    //setting first interest point
                    blockside = new Vector3(Playerposition.x + x - dirx * 0.485f, Playerposition.y + y + shift.y, Playerposition.z + z);
                    //checking for y axis
                    if (activey == 1)
                    {
                        //setting first interest point
                        blockside2 = new Vector3(Playerposition.x + x, Playerposition.y + y - diry * 0.485f + shift.y, Playerposition.z + z);
                        //Do i really need to shoot if there is a wall ?
                        if (GridMatrix[(int)Playerposition.x + dirx, (int)Playerposition.y, (int)Playerposition.z] == null || GridMatrix[(int)Playerposition.x + dirx, (int)Playerposition.y, (int)Playerposition.z].IsLiving())
                        {
                            //setting player interest point and shooting
                            playerside = new Vector3(Playerposition.x + dirx * 0.485f, Playerposition.y, Playerposition.z);
                            if (!Physics.Raycast(playerside, blockside - playerside, Vector3.Distance(playerside, blockside) - 0.15f, layerMask) ||
                            !Physics.Raycast(playerside, blockside2 - playerside, Vector3.Distance(playerside, blockside2) - 0.15f, layerMask))
                                return false;
                        }
                        //Do i really need to shoot if there is a wall ?
                        if (GridMatrix[(int)Playerposition.x, (int)Playerposition.y + diry, (int)Playerposition.z] == null || GridMatrix[(int)Playerposition.x, (int)Playerposition.y+diry, (int)Playerposition.z].IsLiving())
                        {
                            //setting player interest point and shooting
                            playerside = new Vector3(Playerposition.x, Playerposition.y + diry * 0.485f, Playerposition.z);
                            if (!Physics.Raycast(playerside, blockside - playerside, Vector3.Distance(playerside, blockside) - 0.15f, layerMask) ||
                            !Physics.Raycast(playerside, blockside2 - playerside, Vector3.Distance(playerside, blockside2) - 0.15f, layerMask))
                                return false;
                        }
                        return true;
                    }
                    //same with z
                    else
                    {
                        blockside2 = new Vector3(Playerposition.x + x, Playerposition.y + y + shift.y, Playerposition.z + z - dirz * 0.485f);
                        if (GridMatrix[(int)Playerposition.x + dirx, (int)Playerposition.y, (int)Playerposition.z] == null || GridMatrix[(int)Playerposition.x + dirx, (int)Playerposition.y, (int)Playerposition.z].IsLiving())
                        {
                            playerside = new Vector3(Playerposition.x + dirx * 0.485f, Playerposition.y, Playerposition.z);
                            if (!Physics.Raycast(playerside, blockside - playerside, Vector3.Distance(playerside, blockside) - 0.15f, layerMask) ||
                            !Physics.Raycast(playerside, blockside2 - playerside, Vector3.Distance(playerside, blockside2) - 0.15f, layerMask))
                                return false;
                        }
                        if (GridMatrix[(int)Playerposition.x, (int)Playerposition.y, (int)Playerposition.z + dirz] == null || GridMatrix[(int)Playerposition.x, (int)Playerposition.y, (int)Playerposition.z+dirz].IsLiving())
                        {
                            playerside = new Vector3(Playerposition.x, Playerposition.y, Playerposition.z + dirz * 0.485f);
                            if (!Physics.Raycast(playerside, blockside - playerside, Vector3.Distance(playerside, blockside) - 0.15f, layerMask) ||
                            !Physics.Raycast(playerside, blockside2 - playerside, Vector3.Distance(playerside, blockside2) - 0.15f, layerMask))
                                return false;
                        }
                        return true;
                    }

                }
                //same with plane yz
                else
                {
                    blockside = new Vector3(Playerposition.x + x, Playerposition.y + y + shift.y, Playerposition.z + z - dirz * 0.485f);
                    blockside2 = new Vector3(Playerposition.x + x, Playerposition.y + y - diry * 0.485f + shift.y, Playerposition.z + z);
                    if (GridMatrix[(int)Playerposition.x, (int)Playerposition.y, (int)Playerposition.z + dirz] == null || GridMatrix[(int)Playerposition.x, (int)Playerposition.y, (int)Playerposition.z+dirz].IsLiving())
                    {
                        playerside = new Vector3(Playerposition.x, Playerposition.y, Playerposition.z + dirz * 0.485f);
                        if (!Physics.Raycast(playerside, blockside - playerside, Vector3.Distance(playerside, blockside) - 0.15f, layerMask) ||
                            !Physics.Raycast(playerside, blockside2 - playerside, Vector3.Distance(playerside, blockside2) - 0.15f, layerMask))
                            return false;
                    }
                    if (GridMatrix[(int)Playerposition.x, (int)Playerposition.y + diry, (int)Playerposition.z] == null || GridMatrix[(int)Playerposition.x, (int)Playerposition.y+diry, (int)Playerposition.z].IsLiving())
                    {
                        playerside = new Vector3(Playerposition.x, Playerposition.y + diry * 0.485f, Playerposition.z);
                        if (!Physics.Raycast(playerside, blockside - playerside, Vector3.Distance(playerside, blockside) - 0.15f, layerMask) ||
                            !Physics.Raycast(playerside, blockside2 - playerside, Vector3.Distance(playerside, blockside2) - 0.15f, layerMask))
                            return false;
                    }
                    return true;
                }

            //3 axis = 3 interest point on the player and 3 on the block
            //i shoot (if necessary) a raycast from each player point to each block point, and check for any obsacle
            //if even one of them is free to go, then that mean i can target the block
            case 3:
                //setting block's interest points
                blockside = new Vector3(Playerposition.x + x - dirx * 0.485f, Playerposition.y + y + shift.y, Playerposition.z + z);
                blockside2 = new Vector3(Playerposition.x + x, Playerposition.y + y + shift.y, Playerposition.z + z - dirz * 0.485f);
                blockside3 = new Vector3(Playerposition.x + x, Playerposition.y + y - diry * 0.485f + shift.y, Playerposition.z + z);

                //Do i really need to shoot if there is a wall ?
                if (GridMatrix[(int)Playerposition.x + dirx, (int)Playerposition.y, (int)Playerposition.z] == null || GridMatrix[(int)Playerposition.x + dirx, (int)Playerposition.y, (int)Playerposition.z].IsLiving())
                {
                    //setting player interest point and shooting
                    playerside = new Vector3(Playerposition.x + dirx * 0.485f, Playerposition.y, Playerposition.z);
                    if (!Physics.Raycast(playerside, blockside2 - playerside, Vector3.Distance(playerside, blockside2) - 0.1f, layerMask)
                        || !Physics.Raycast(playerside, blockside - playerside, Vector3.Distance(playerside, blockside) - 0.1f, layerMask)
                        || !Physics.Raycast(playerside, blockside3 - playerside, Vector3.Distance(playerside, blockside3) - 0.1f, layerMask))
                        return false;
                }
                //same
                if (GridMatrix[(int)Playerposition.x, (int)Playerposition.y, (int)Playerposition.z + dirz] == null || GridMatrix[(int)Playerposition.x, (int)Playerposition.y, (int)Playerposition.z+dirz].IsLiving())
                {
                    playerside = new Vector3(Playerposition.x, Playerposition.y, Playerposition.z + dirz * 0.485f);
                    if (!Physics.Raycast(playerside, blockside - playerside, Vector3.Distance(playerside, blockside) - 0.1f, layerMask)
                        || !Physics.Raycast(playerside, blockside2 - playerside, Vector3.Distance(playerside, blockside2) - 0.1f, layerMask)
                        || !Physics.Raycast(playerside, blockside3 - playerside, Vector3.Distance(playerside, blockside3) - 0.1f, layerMask))
                        return false;
                }
                //same
                if (GridMatrix[(int)Playerposition.x, (int)Playerposition.y + diry, (int)Playerposition.z] == null || GridMatrix[(int)Playerposition.x, (int)Playerposition.y + diry, (int)Playerposition.z].IsLiving())
                {
                    playerside = new Vector3(Playerposition.x, Playerposition.y + diry * 0.485f, Playerposition.z);
                    if (!Physics.Raycast(playerside, blockside - playerside, Vector3.Distance(playerside, blockside) - 0.1f, layerMask)
                        || !Physics.Raycast(playerside, blockside2 - playerside, Vector3.Distance(playerside, blockside2) - 0.1f, layerMask)
                        || !Physics.Raycast(playerside, blockside3 - playerside, Vector3.Distance(playerside, blockside3) - 0.1f, layerMask))
                        return false;
                }
                return true;
            default:
                break;
        }
        return true;
    }

    /// <summary>
    /// Return a list of all living targets in range
    /// </summary>
    /// <param name="Playerposition"></param>
    /// <param name="minrange"></param>
    /// <param name="maxrange"></param>
    /// <param name="throughtblocks">If we can see targets through blocks</param>
    /// <returns></returns>
    public List<LivingPlaceable> FindTargetableLiving(Vector3 Playerposition, Skill skill)
    {
        //get usefull parameters from skill
        int minrange = skill.MinRange;
        int maxrange = skill.MaxRange;
        bool throughblocks = skill.Throughblocks;
        bool highrange = minrange > 0 ? true : false;

        List<LivingPlaceable> targetableliving = new List<LivingPlaceable>();

        foreach (LivingPlaceable gameObjCharacter in GameManager.instance.player1.GetComponent<Player>().Characters)
        {
            Vector3 distance = gameObjCharacter.GetPosition() - Playerposition;
            float yrange = (highrange ? (distance.y >= 0 ? distance.y : Math.Max(0, (-(maxrange - 1) + distance.y)) * maxrange) : distance.y);
            float totaldist = Mathf.Abs(distance.x) + Mathf.Abs(distance.z) + Math.Abs(yrange);
            int dirx = distance.x >= 0 ? (distance.x == 0 ? 0 : 1) : -1;
            int diry = distance.y >= 0 ? (distance.y == 0 ? 0 : 1) : -1;
            int dirz = distance.z >= 0 ? (distance.z == 0 ? 0 : 1) : -1;
            if ((totaldist == 0 && minrange == 0) || totaldist <= maxrange && totaldist >= minrange && (throughblocks || !RayCastBlock((int)distance.x, (int)distance.y, (int)distance.z,
               dirx, diry, dirz, Playerposition, new Vector3(0,0,0))))
            {
                targetableliving.Add(gameObjCharacter);
            }
        }

        foreach (LivingPlaceable gameObjCharacter in GameManager.instance.player2.GetComponent<Player>().Characters)
        {
            Vector3 distance = gameObjCharacter.GetPosition() - Playerposition;
            float yrange = (highrange ? (distance.y >= 0 ? distance.y : Math.Max(0, (-(maxrange-1)+distance.y))*maxrange) : distance.y);
            float totaldist = Mathf.Abs(distance.x) + Mathf.Abs(distance.z) + Math.Abs(yrange);
            int dirx = distance.x >= 0 ? (distance.x == 0 ? 0 : 1) : -1;
            int diry = distance.y >= 0 ? (distance.y == 0 ? 0 : 1) : -1;
            int dirz = distance.z >= 0 ? (distance.z == 0 ? 0 : 1) : -1;
            if ((totaldist == 0 && minrange == 0) || totaldist <= maxrange && totaldist >= minrange && (throughblocks || !RayCastBlock((int)distance.x, (int)distance.y, (int)distance.z,
                dirx, diry, dirz, Playerposition, new Vector3(0, 0, 0))))
            {
                targetableliving.Add(gameObjCharacter);
            }
        }

        return targetableliving;
    }
}
