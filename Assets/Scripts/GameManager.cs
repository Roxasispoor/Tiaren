using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System;

/// <summary>
/// Classe centrale gérant le déroulement des tours et répertoriant les objets
/// </summary>
public class GameManager : NetworkBehaviour
{
    [SerializeField]
    string mapToCharge = "Grid.json";

    //can't be the network manager or isServer can't work
    public static GameManager instance;
    public Material pathFindingMaterial;
    public GameMode gameMode = GameMode.DEATHMATCH;
    public Material targetMaterial;
    public NetworkManager networkManager;
    private const int maxBatchVertexes= 2300;
    private int numberTurn = 0;
    public Dictionary<int, Placeable> idPlaceable;
    public GameObject batchPrefab;
    public GameObject batchFolder;
    public GameObject[] prefabCharacs;
    public GameObject[] prefabWeapons;
    public GameObject gridFolder;
    public GameObject player1; //Should be Object

    public GameObject player2; //Should be Object
    public GameObject[] prefabMonsters;
    public Skill activeSkill;
    public States state;
    

    private List<StackAndPlaceable> turnOrder;
    Dictionary<string, List<Batch>> dictionaryMaterialsFilling;

    private Player winner;
    public LivingPlaceable playingPlaceable;
    private UIManager UIManager;

    /// <summary>
    /// allows to know if the game ended
    /// </summary>
    /// <returns></returns>

    /// <summary>
    /// display number of the current turn
    /// </summary>
    /// 


    public int NumberTurn
    {
        get
        {
            return numberTurn;
        }

        set
        {
            numberTurn = value;
        }
    }

    /// <summary>
    /// Indique player having turn
    /// </summary>


    
    public Player Winner
    {
        get
        {
            return winner;
        }

        set
        {
            winner = value;
        }
    }


    public LivingPlaceable PlayingPlaceable
    {
        get
        {
            return playingPlaceable;
        }

        set
        {
            playingPlaceable = value;
        }
    }

    public List<StackAndPlaceable> TurnOrder
    {
        get
        {
            return turnOrder;
        }

        set
        {
            turnOrder = value;
        }
    }



    /// <summary>
    /// Return the player who has authority
    /// </summary>
    /// <returns></returns>
    public Player GetLocalPlayer()
    {
        Player toreturn = player1.GetComponent<Player>().hasAuthority ? player1.GetComponent<Player>() : player2.GetComponent<Player>();
        return toreturn;
    }

    private void Awake()

    {

        //CSVReader.ReadVectors("untitled.txt");
        //MapConverter.ConvertGridFromText("Towers.txt", "Towers.json");
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);


        idPlaceable = new Dictionary<int, Placeable>();
        TurnOrder = new List<StackAndPlaceable>();
        //    DontDestroyOnLoad(gameObject);
        //Initialize la valeur statique de chaque placeable, devrait rester identique entre deux versions du jeu, et ne pas poser problème si les new prefabs sont bien rajoutés a la fin
        networkManager = (NetworkManager) FindObjectOfType(typeof(NetworkManager));
        for (int i = 0; i < networkManager.spawnPrefabs.Count; i++)
        {
            networkManager.spawnPrefabs[i].GetComponent<Placeable>().serializeNumber = i + 1; // kind of value shared by all prefab, doesn't need to be static

        }

    }



    /** Running
1)reset pm and bool for shot/skill
Apply effects that activate at the beginning of a turn
Substract a turn to persistent linked effects
Let the user act, using a skill, pm...
When an effect trigger, added to gameManager
gameManager apply, check effect is activable, not stopped, etc... and use()
 Repeat until the end of 30s
**/


    // Use this for initialization
   
    IEnumerator Start()
    {
        //PHASE 0 : SET THE GAME UP

        //If you want to create one and save it
        // Grid.instance.CreateRandomGrid(gridFolder);
        //Grid.instance.SaveGridFile();

        //If you want to load one

        Grid.instance.FillGridAndSpawn(gridFolder, mapToCharge);
           
        while (player1 == null )
        {
               
                yield return null;
        }
        Debug.Log("Please load on client 1 ffs");
        while (player2 == null )
        { 
            yield return null;
        }
        CreateCharacters(player1, new Vector3Int(0, 4, 0));
        CreateCharacters(player2, new Vector3Int(3, 4, 0));
        
        Grid.instance.Gravity();
        //To activate for perf, desactivate for pf
        if (isClient)
        {
            InitialiseBatchFolder();
        }
        
            //Retrieve data 

        //RpcStartGame();
        BeginningOfTurn();
    
    }

    public void CheckWinCondition()
    {
            if (gameMode == GameMode.DEATHMATCH)
        {
            player2.GetComponent<Player>().isWinner = true;
            foreach (GameObject character in player1.GetComponent<Player>().characters)
            {
                if(!character.GetComponent<LivingPlaceable>().IsDead)
                {
                    player2.GetComponent<Player>().isWinner = false;
                    break;
                }
            }
            player1.GetComponent<Player>().isWinner = true;
            foreach (GameObject character in player2.GetComponent<Player>().characters)
            {
                if (!character.GetComponent<LivingPlaceable>().IsDead)
                {
                    player1.GetComponent<Player>().isWinner = false;
                    break;
                }
            }
            if(player1.GetComponent<Player>().isWinner || player2.GetComponent<Player>().isWinner)
            {
                //Disable canvas element
                Canvas c = player1.transform.GetComponentInChildren<Canvas>();
                if(c!=null)
                { 
                foreach (Transform des in c.transform)
                {
                    des.gameObject.SetActive(false);
                }
                }
                c = player2.transform.GetComponentInChildren<Canvas>();
                if (c != null)
                {
                    foreach (Transform des in player2.GetComponentInChildren<Canvas>().transform)
                    {
                        des.gameObject.SetActive(false);
                    }
                }

                player1.GetComponent<Player>().winText.gameObject.SetActive(true);
                player2.GetComponent<Player>().winText.gameObject.SetActive(true);
            }
            if(player1.GetComponent<Player>().isWinner && !player2.GetComponent<Player>().isWinner)
            {
                
                player1.GetComponent<Player>().winText.text = "VICTORY";
                player2.GetComponent<Player>().winText.text = "DEFEAT";
            }
            else if(player2.GetComponent<Player>().isWinner && !player1.GetComponent<Player>().isWinner)
            {
           
                player2.GetComponent<Player>().winText.text = "VICTORY";
                player1.GetComponent<Player>().winText.text = "DEFEAT";
            }
            else if(player2.GetComponent<Player>().isWinner && player1.GetComponent<Player>().isWinner)
            {
                player2.GetComponent<Player>().winText.text = "DRAW";
                player1.GetComponent<Player>().winText.text = "DRAW";
            }

        }

        
    }
    private void UpdateTimeline()
    {
        TurnOrder.Clear();
        //add every character once and sort them
        foreach (GameObject character in player1.GetComponent<Player>().Characters)
        {
            LivingPlaceable placeable = character.GetComponent<LivingPlaceable>();
            StackAndPlaceable newCharacter = new StackAndPlaceable(placeable, placeable.SpeedStack, false);
            TurnOrder.Add(newCharacter);
        }
        foreach (GameObject character in player2.GetComponent<Player>().Characters)
        {
            LivingPlaceable placeable = character.GetComponent<LivingPlaceable>();
            StackAndPlaceable newCharacter = new StackAndPlaceable(placeable, placeable.SpeedStack, false);
            TurnOrder.Add(newCharacter);
        }
        TurnOrder.Sort((x, y) => x.SpeedStack == y.SpeedStack ? 1 : (int)((x.SpeedStack - y.SpeedStack) / (Mathf.Abs(x.SpeedStack - y.SpeedStack))));

        //Check for every Character if it has to play more than once in the "Grand Turn"

        int numberOfTurns = 1;
        for (int i = 0; i < turnOrder.Count -1; i++)
        {
            StackAndPlaceable check = turnOrder[i];
            if (!check.SeenBefore)
            {
                for (int j = i + 1; j < turnOrder.Count - 1; j++)
                {
                    if (turnOrder[j].SpeedStack > check.SpeedStack + numberOfTurns * (j-i) / check.Character.Speed)
                    {
                        turnOrder.Insert(j, new StackAndPlaceable(check.Character, check.SpeedStack + numberOfTurns * (j - i) / check.Character.Speed, true));
                        j++;
                    }
                }
            }
            check.SeenBefore = true;
        }

    }

    public void RefreshBatch(Placeable block)
    {
        block.batch.batchObject.GetComponent<MeshRenderer>().material = block.GetComponent<MeshRenderer>().material;
        block.batch.batchObject.GetComponent<MeshFilter>().mesh = new Mesh();
        block.batch.batchObject.GetComponent<MeshFilter>().mesh.CombineMeshes(
        block.batch.combineInstances.ToArray(), true, true);
    }


    /// <summary>
    /// Removes block From Batch
    /// </summary>
    /// <param name="block"></param>
    public void RemoveBlockFromBatch(Placeable block)
    {
        if (!block.IsLiving())
        {
            block.batch.combineInstances.Remove(block.MeshInCombined);
            block.GetComponent<MeshRenderer>().enabled = true;

            RefreshBatch(block);


        }
    }
    /// <summary>
    /// Creates a new batch from the material given, combines instances in dico 
    /// </summary>
    /// <param name="meshFilter"></param>
    /// <param name="dictionaryMaterialsFilling"></param>
    private void CreateNewBatch(Batch batch)
    {
        GameObject newBatch = Instantiate(batchPrefab, batchFolder.transform);
        batch.batchObject = newBatch;
        Material saved = null;
        //get the good material
        foreach (Material mat in newBatch.GetComponent<Renderer>().materials)
        {
            if(mat.name==batch.material)
            {
                saved = mat;
                break;
            }
        }
        if (saved != null)

        {
            newBatch.GetComponent<MeshRenderer>().material = saved;
            newBatch.GetComponent<MeshFilter>().mesh = new Mesh();


            newBatch.GetComponent<MeshFilter>().mesh.CombineMeshes(
                batch.combineInstances.ToArray(), true, true);
            newBatch.GetComponent<MeshRenderer>().materials = new Material[] { newBatch.GetComponent<MeshRenderer>().materials[0] };

        }
        else
        {
            Debug.Log("erreur,batch material non trouvé" + batch.material);
        }

        }
    public void ResetAllBatches()
    { 
     foreach (Transform child in batchFolder.transform)
        {
            Destroy(child.gameObject);
        }
        GameManager.instance.InitialiseBatchFolder();
    }

    public void MoveLogic(List<Vector3> bezierPath)
    {
        if(playingPlaceable.player.isLocalPlayer)
        {
            playingPlaceable.ResetAreaOfMovement();

            Debug.Log("PM: " + playingPlaceable.CurrentPM);
            playingPlaceable.AreaOfMouvement = Grid.instance.CanGo(bezierPath[bezierPath.Count - 1] + new Vector3(0, 1, 0), playingPlaceable.CurrentPM,
            playingPlaceable.Jump, playingPlaceable.Player);

            playingPlaceable.ChangeMaterialAreaOfMovement(pathFindingMaterial);

        }

    }
    public void OnEndAnimationEffectEnd()
    {
        if(playingPlaceable.player.isLocalPlayer)
        { 
        MoveLogic(new List<Vector3>() { playingPlaceable.GetPosition() });
            GameManager.instance.state = States.Move;
        }
    }
    /// <summary>
    /// Add current combine instance to its batch
    /// </summary>
    /// <param name="meshFilter"></param>
    /// <param name="combineInstance"></param>
    public void AddMeshToBatches(MeshFilter meshFilter,CombineInstance combineInstance)
    {

        if (!dictionaryMaterialsFilling.ContainsKey(meshFilter.GetComponent<MeshRenderer>().material.name))
        {
            //nouvelle liste de batch, avec un batch de crée dedans
            dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name] = new List<Batch>
            {
                new Batch(meshFilter.GetComponent<MeshRenderer>().material.name)
            };
            
        }
        if (dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name]
            [dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name].Count - 1].combineInstances.Count >= maxBatchVertexes)
        //hard limit for number of cube ~2370 =>65500 vertexes; Overload
        //If last batch is full, well new one necessary
        {
            Debug.Log("New batch necessary, capacity exceeded");
            CreateNewBatch(dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name]
            [dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name].Count - 1]);
            dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name].Add(new Batch(meshFilter.GetComponent<MeshRenderer>().material.name));
  

        }
        if(meshFilter.GetComponent<Placeable>())//Add batch to placeable
        {
            meshFilter.GetComponent<Placeable>().batch = dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name]
            [dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name].Count - 1];
        }
        dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name]
            [dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name].Count - 1].combineInstances.Add(combineInstance);
        // Add the current combine to the last list

        meshFilter.GetComponent<MeshRenderer>().enabled = false;
    }
    public void InitialiseBatchFolder()
    {
        dictionaryMaterialsFilling=new Dictionary<string, List<Batch>>();


        MeshFilter[] meshFilters = gridFolder.GetComponentsInChildren<MeshFilter>();
        //Todo: if necessary chose them by big cube or something
        foreach(MeshFilter meshFilter in meshFilters)
        {

            CombineInstance currentInstance = new CombineInstance
            {
                mesh = meshFilter.sharedMesh,
                transform = meshFilter.transform.localToWorldMatrix
            };
            // If it is the first of this material
            AddMeshToBatches(meshFilter,currentInstance);
            meshFilter.GetComponent<Placeable>().MeshInCombined = currentInstance;

        }
        //Then at the end we create all the batches that are not full
        foreach(List<Batch> batches in dictionaryMaterialsFilling.Values)
        {
            CreateNewBatch(batches[batches.Count-1]); //instanciate last batch
        }
    }

    private void BeginningOfTurn()
    {
        Grid.instance.Gravity();
        UpdateTimeline();
        playingPlaceable = TurnOrder[0].Character;
        playingPlaceable.SpeedStack += 1 / playingPlaceable.Speed;
        if (playingPlaceable.IsDead)
        {
            playingPlaceable.TurnsRemaingingCemetery--;
            EndOFTurn();
        }
        else
        {

            //initialise UI
            
            player2.GetComponent<UIManager>().ChangeTurn();
            player1.GetComponent<UIManager>().ChangeTurn();
            state = States.Move;
            // reducing cooldown of skill by 1
            foreach (Skill sk in playingPlaceable.Skills)
            {
                if (sk.TourCooldownLeft > 0)
                {
                    sk.TourCooldownLeft--;
                }
            }
            if(isClient && playingPlaceable.Player.isLocalPlayer)
            {

                playingPlaceable.Player.cameraScript.target = playingPlaceable.GetComponent<Placeable>().gameObject.transform;

            }
            playingPlaceable.CurrentPM = playingPlaceable.MaxPM;
            playingPlaceable.CurrentPA = playingPlaceable.PaMax;
            playingPlaceable.Player.clock.IsFinished = false;
            if(playingPlaceable.Player.isLocalPlayer)
            {
                playingPlaceable.AreaOfMouvement = Grid.instance.CanGo(playingPlaceable.GetPosition(), playingPlaceable.CurrentPM,
                playingPlaceable.Jump,playingPlaceable.Player);
                playingPlaceable.ChangeMaterialAreaOfMovement(pathFindingMaterial);
            }
            player1.GetComponent<Timer>().StartTimer(30f);
            player2.GetComponent<Timer>().StartTimer(30f);

        }
    }

    public void EndOFTurn()
    {
        if(winner==null)
        { 
        //cleaning and checks and synchro with banana dancing if needed
        Debug.Log("tour suivaaaaaaaaant Area of movement="+playingPlaceable.AreaOfMouvement.Count);
        if (playingPlaceable.Player.isLocalPlayer)
        {
            playingPlaceable.ResetAreaOfTarget();
            playingPlaceable.ResetAreaOfMovement();
            //ResetAllBatches();
        }
            BeginningOfTurn();
        }
    }

    //TODO : MANAGE SKILL CREATION AND USAGE (WAITING FOR SKILL'S PROPER IMPLEMENTATION)
    //Make a copy from model skill in skills, and fill with additional info (caster, targets)
    public void UseSkill(int skillID, LivingPlaceable caster, List<Placeable> targets)
    {
        Skill skill = playingPlaceable.Skills[skillID];
        if (caster.CurrentPA > skill.Cost && skill.Use(caster, targets))
        {
            caster.CurrentPA= caster.CurrentPA - skill.Cost>0? caster.CurrentPA - skill.Cost:0; //On clamp à 0, on est pas trop sur de ce qui a pu se passer dans le use
        }
    }

    public Placeable FindLocalObject(int id)
    {

        return idPlaceable[id];
    }
    public Vector3[] GetPathFromClicked(Placeable arrival)
    {

        NodePath destination = new NodePath(arrival.GetPosition().x, arrival.GetPosition().y, arrival.GetPosition().z, 0, null);
        NodePath inListDestination = playingPlaceable.AreaOfMouvement.Find(destination.Equals);
        if (inListDestination != null)
        {
            Vector3[] realPath = inListDestination.GetFullPath();
            return realPath;
        }
        return null;
    }
    private void InitialiseCharacter(GameObject charac, GameObject player, Vector3Int spawnCoordinates)
    {
        LivingPlaceable charac1 = charac.GetComponent<LivingPlaceable>();

        player.GetComponent<Player>().Characters.Add(charac);
        charac1.Player = player.GetComponent<Player>();

        Vector3Int posPers = (Vector3Int) spawnCoordinates;
        Grid.instance.GridMatrix[posPers.x, posPers.y, posPers.z] = charac1;
        charac1.Weapons.Add(Instantiate(prefabWeapons[0], charac.transform)); // to change in function of the start weapon
        charac1.EquipedWeapon = charac1.Weapons[0].GetComponent<Weapon>();
        charac1.netId = Placeable.currentMaxId;
        Debug.Log(charac1.netId);
        idPlaceable[charac1.netId]= charac1;
        Placeable.currentMaxId++;

    }

    
    public void CreateCharacters(GameObject player, Vector3Int spawnCoordinates)
    {
        Player playerComponent = player.GetComponent<Player>();
        for (int i = 0; i < playerComponent.NumberPrefab.Count; i++)
        {
            GameObject charac = Instantiate(prefabCharacs[player.GetComponent<Player>().NumberPrefab[i]], new Vector3(spawnCoordinates.x, spawnCoordinates.y, spawnCoordinates.z), Quaternion.identity);

            InitialiseCharacter(charac, player, spawnCoordinates);

            Vector3 realCoordinates = new Vector3Int(spawnCoordinates.x, spawnCoordinates.y, spawnCoordinates.z);
        }

    }
    
    /// <summary>
    /// Unused function to apply function to all visible characters
    /// </summary>
    /// <param name="shooter"></param>
    public void Shootable(LivingPlaceable shooter)
    {
        Vector3 shootPos = shooter.GetPosition() + shooter.ShootPosition;
        foreach (GameObject iterCharac1 in player1.GetComponent<Player>().Characters)
        {

            LivingPlaceable charac1 = iterCharac1.GetComponent<LivingPlaceable>();

            if (shooter.CanHit(charac1).Count > 0)
            {

                //typically, changing color
            }

        }

        foreach (GameObject iterCharac2 in player2.GetComponent<Player>().Characters)
        {
            LivingPlaceable charac2 = iterCharac2.GetComponent<LivingPlaceable>();

            if (shooter.CanHit(charac2).Count > 0)
            {

                //typically, changing color
            }


        }
      
    }
    
}
