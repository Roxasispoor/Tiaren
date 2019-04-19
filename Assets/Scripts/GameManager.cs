using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// Classe centrale gérant le déroulement des tours et répertoriant les objets
/// </summary>
public class GameManager : NetworkBehaviour
{
    /// <summary>
    /// Name of the file to charge for the map
    /// </summary>
    [SerializeField]
    private string mapToCharge;
    /// <summary>
    /// Enforce singleton pattern
    /// </summary>
    public static GameManager instance;
    /// <summary>
    /// Material used to highlight pathfinding
    /// </summary>
    public Material pathFindingMaterial;
    /// <summary>
    /// Material used to highlight target cubes quads when mouse hover it
    /// </summary>
    public Material highlightingMaterial;
    /// <summary>
    /// Specifies current GameMode
    /// </summary>
    public GameMode gameMode = GameMode.FLAG;
    /// <summary>
    ///  Material used to highlight target cubes (not quads) when skill is used
    /// </summary>
    public Material targetMaterial;
    /// <summary>
    /// Material used for ally block (atm : spawn and goals)
    /// </summary>
    public Material spawnAllyMaterial;
    /// <summary>
    /// Material used for ally block (atm : spawn and goals)
    /// </summary>
    public Material spawnEnemyMaterial;
    /// <summary>
    /// Length of turn in second
    /// </summary>
    public float timerLength;
    /// <summary>
    /// Manages all the network part once in game
    /// </summary>
    public NetworkManager networkManager;
    /// <summary>
    /// Max number of vertexes by batch and enforce the limit of 65536 when combining them
    /// </summary>
    private const int maxBatchVertexes = 2300;
    /// <summary>
    /// Turn number
    /// </summary>
    private int numberTurn = 0;
    /// <summary>
    /// Allows to find placeable corresponding to netId efficiently
    /// </summary>
    public Dictionary<int, NetIdeable> idPlaceable;
    /// <summary>
    /// Prefab that will be the placeholder for batches of cubes
    /// </summary>
    public GameObject batchPrefab;
    /// <summary>
    /// Parent of batches in hierarchy
    /// </summary>
    public GameObject batchFolder;
    /// <summary>
    /// Array where are stored all the character's prefabs
    /// </summary>
    public GameObject[] prefabCharacs;
    /// <summary>
    /// GameObject representing player 1
    /// </summary>
    public GameObject player1; //Should be Object
    /// <summary>
    /// GameObject representing player 1
    /// </summary>
    public GameObject player2; //Should be Object
    /// <summary>
    /// Used during the spawn, the character selected
    /// </summary>
    private LivingPlaceable characterToSpawn;
    /// <summary>
    /// Username of player 1 (Used for reconnection)
    /// </summary>
    public string player1Username = "";
    /// <summary>
    /// Username of player 1 (Used for reconnection)
    /// </summary>
    public string player2Username = ""; //TODOShould instead be account and serialize 
    /// <summary>
    /// Color representing the local player
    /// </summary>
    public Color localPlayerColor = Color.blue;
    /// <summary>
    /// Color reprensenting the remote player
    /// </summary>
    public Color ennemyPlayerColor = Color.red;
    /// <summary>
    /// Skill currently selected
    /// </summary>
    private Skill activeSkill;
    /// <summary>
    /// The states of the game
    /// </summary>
    private States state;
    /// <summary>
    /// The selection used to launch a skill.
    /// </summary>
    public SelectionInfo currentSelection;
    /// <summary>
    /// A list sorted by the apparition order of the next characters (the list is wide enough
    /// to have each characters appearing at least once).
    /// </summary>
    private List<StackAndPlaceable> turnOrder;
    /// <summary>
    /// Dictionnary referencing the batch per material
    /// </summary>
    private Dictionary<string, List<Batch>> dictionaryMaterialsFilling;
    /// <summary>
    /// True if the game started (Set before the 1st turn)
    /// </summary>
    public bool isGameStarted = false;
    /// <summary>
    /// Represents the time during which the tittle card is on the screen
    /// </summary>
    public float timeBetweenTurns;
    /// <summary>
    /// Used for the network
    /// </summary>
    public TransmitterNoThread transmitter;
    /// <summary>
    /// The winner of the game.
    /// </summary>
    public Player winner;
    /// <summary>
    /// The character currently playing
    /// </summary>
    private LivingPlaceable playingPlaceable;
    /// <summary>
    /// Characters you can create
    /// </summary>
    private List<SpriteAndName> possibleCharacters = new List<SpriteAndName>(); // list of all the characters in the game
    /// <summary>
    /// Local Player
    /// </summary>
    private Player localPlayer;
    /// <summary>
    /// The raycast selector of the local player
    /// </summary>
    private RaycastSelector raycastSelector;
    /// <summary>
    /// Dictionary to make the correspondance between the choices of the players and the skill to create
    /// </summary>
    public Dictionary<SkillTypes, System.Type> SkillDictionary;
    /// <summary>
    /// References the grid folder
    /// </summary>
    public GameObject gridFolder;

    /// <summary>
    /// Display number of the current turn
    /// </summary>
    /// 
    public int NumberTurn
    {
        get
        {
            return numberTurn;
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
            if (playingPlaceable != null)
            {
                playingPlaceable.EndingMyTurn();
            }
            if (value != null)
            {
                value.BeginningMyTurn();
            }
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

    public LivingPlaceable CharacterToSpawn
    {
        get
        {
            return characterToSpawn;
        }

        set
        {

            if (characterToSpawn != null)
            {
                characterToSpawn.UnHighlightTarget();
            }

            if (value != null)
            {
                value.HighlightForSpawn();
            }

            characterToSpawn = value;
        }
    }

    public List<SpriteAndName> PossibleCharacters
    {
        get
        {
            return possibleCharacters;
        }

        set
        {
            possibleCharacters = value;
        }
    }

    public Player Player1
    {
        get
        {
            return player1.GetComponent<Player>();
        }
    }

    public Player Player2
    {
        get
        {
            return player2.GetComponent<Player>();
        }
    }

    public States State
    {
        get
        {
            return state;
        }

        set
        {
            if (state == States.Spawn) // Previous state : SPAWN
            {
                EndSpawn();
            } else if (state == States.UseSkill && value != state) // Previous state UseSkill and next != UseSkill
            {
                raycastSelector.UnHighligthAndUnPreviewCurrent();
            }

            if (value == States.Move)
            {

            }
            state = value;
        }
    }
    /*
    public Placeable Hovered
    {
        get
        {
            return hovered;
        }

        set
        {
            if (hovered != null)
            {

                if (State == States.UseSkill && activeSkill != null)
                {
                    ActiveSkill.UnPreview(hovered);
                }
                hovered.UnHighlight();
            }

            if (value != null)
            {
                value.Highlight();
                if (State == States.UseSkill && playingPlaceable.IsPlaceableInTarget(value))
                {
                    ActiveSkill.Preview(value);
                }
            }
            hovered = value;
        }
    }*/

    public RaycastSelector RaycastSelector
    {
        get
        {
            return raycastSelector;
        }

        set
        {
            raycastSelector = value;
        }
    }

    public Skill ActiveSkill
    {
        get
        {
            return activeSkill;
        }

        set
        {
            activeSkill = value;
        }
    }

    /// <summary>
    /// Return the player who has authority
    /// </summary>
    /// <returns></returns>
    public Player GetLocalPlayer()
    {
        return localPlayer;
    }

    private void Awake()

    {
        // Singleton patern
        if (instance == null)
        {

            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }


        idPlaceable = new Dictionary<int, NetIdeable>();
        TurnOrder = new List<StackAndPlaceable>();
        batchFolder = new GameObject("Batch Folder");
        //Initialize la valeur statique de chaque placeable, devrait rester identique entre deux versions du jeu, et ne pas poser problème si les new prefabs sont bien rajoutés a la fin
        networkManager = (NetworkManager)FindObjectOfType(typeof(NetworkManager));
        for (int i = 0; i < networkManager.spawnPrefabs.Count; i++)
        {
            networkManager.spawnPrefabs[i].GetComponent<Placeable>().serializeNumber = i + 1; // kind of value shared by all prefab, doesn't need to be static
        }
        

        //init Posissible characters
        string path = Path.Combine(Application.streamingAssetsPath , "Teams.json");
        string line;

        StreamReader reader = new StreamReader(path);
        while ((line = reader.ReadLine()) != null)
        {
            SpriteAndName spriteAndName = JsonUtility.FromJson<SpriteAndName>(line);
            PossibleCharacters.Add(spriteAndName);
        }

        transmitter = GetComponent<TransmitterNoThread>();
        ParameterChangeV2<LivingPlaceable, float>.MethodsForEffects.Add(o => o.MaxPMFlat);
        ParameterChangeV2<LivingPlaceable, float>.MethodsForEffects.Add(o => o.JumpFlat);
        ParameterChangeV2<LivingPlaceable, float>.MethodsForEffects.Add(o => o.CurrentPAFlat);

        SkillDictionary = new Dictionary<SkillTypes, System.Type>();
        SkillDictionary.Add(SkillTypes.PUSH, System.Type.GetType("PushSkill"));
        SkillDictionary.Add(SkillTypes.CREATE, System.Type.GetType("CreateSkill"));
        SkillDictionary.Add(SkillTypes.DESTROY, System.Type.GetType("DestroySkill"));
        SkillDictionary.Add(SkillTypes.SWORDATTACK, System.Type.GetType("SwordAttack"));
        SkillDictionary.Add(SkillTypes.BLEEDING, System.Type.GetType("BleedingAttack"));
        SkillDictionary.Add(SkillTypes.LEGSWIPE, System.Type.GetType("LegSwipe"));
        SkillDictionary.Add(SkillTypes.SPINNINGATTACK, System.Type.GetType("SpinningAttack"));
        SkillDictionary.Add(SkillTypes.TACKLE, System.Type.GetType("Tackle"));
        SkillDictionary.Add(SkillTypes.BOWSHOT, System.Type.GetType("BowShot"));
        SkillDictionary.Add(SkillTypes.PIERCINGSHOT, System.Type.GetType("PiercingShot"));
        SkillDictionary.Add(SkillTypes.HIGHGROUND, System.Type.GetType("HighGround"));
        SkillDictionary.Add(SkillTypes.ZIPLINE, System.Type.GetType("CreateZiplineSkill"));
        SkillDictionary.Add(SkillTypes.ACROBATIC, System.Type.GetType("AcrobaticSkill"));
        SkillDictionary.Add(SkillTypes.MAGICMISSILE, System.Type.GetType("MagicMissile"));
        SkillDictionary.Add(SkillTypes.FISSURE, System.Type.GetType("Fissure"));
        SkillDictionary.Add(SkillTypes.FIREBALL, System.Type.GetType("Fireball"));
        SkillDictionary.Add(SkillTypes.WALL, System.Type.GetType("Wall"));
        SkillDictionary.Add(SkillTypes.EARTHBENDING, System.Type.GetType("EarthBending"));
        SkillDictionary.Add(SkillTypes.REPULSIVEGRENADE, System.Type.GetType("RepulsiveGrenade"));
        SkillDictionary.Add(SkillTypes.GRAPPLE, System.Type.GetType("GrappleHook"));
        SkillDictionary.Add(SkillTypes.MAKIBISHI, System.Type.GetType("MakibishiSkill"));
        SkillDictionary.Add(SkillTypes.CREATETOTEMHP, System.Type.GetType("CreateTotemHP"));
        SkillDictionary.Add(SkillTypes.CREATETOTEMAP, System.Type.GetType("CreateTotemAP"));
        SkillDictionary.Add(SkillTypes.PICKOBJECT, System.Type.GetType("PickObject"));

        // ***** BELOW - Initialise the skills given in game *****
        StreamReader readerJSON = new StreamReader(Path.Combine(Application.streamingAssetsPath, "Skills.json"));
        string JSON = readerJSON.ReadToEnd();

        PickObject.reference = new PickObject(JSON);
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

    private IEnumerator Start()
    {
        Debug.Log("Game starting");

        //PHASE 0 : SET THE GAME UP

        //If you want to create one and save it

        state = States.TeamSelect;
        while (player1 == null)
        {
            yield return null;
        }
        while (player2 == null)
        {
            yield return null;
        }
        if (isServer)
        {
            Player1.RpcSetPlayer1();
            Player2.RpcSetPlayer2();
        }
        if (isClient)
        {
            GameManager.instance.localPlayer = player1.GetComponent<Player>().hasAuthority ? player1.GetComponent<Player>() : player2.GetComponent<Player>();
        }
        gridFolder = new GameObject("GridFolder");
        gridFolder.AddComponent<NetworkIdentity>();
        string pathToMap = Path.Combine(Application.streamingAssetsPath, mapToCharge);
        Grid.instance.CreateGrid(gridFolder, pathToMap);
        transmitter.networkManager = networkManager;
        
        Debug.Log("Right before select");
        TeamSelectDisplay();
        InitialiseBatchFolder();
        player1.gameObject.name = "player1";
        player2.gameObject.name = "player2";

        Player1.spawnList = Grid.instance.GetSpawnPlayer(Player1);
        Player2.spawnList = Grid.instance.GetSpawnPlayer(Player2);

        if (isClient)
        {
            localPlayer.color = localPlayerColor;

            Player otherPlayer = GetOtherPlayer(localPlayer.gameObject).GetComponent<Player>();
            otherPlayer.color = ennemyPlayerColor;

            
            localPlayer.SendSpawnToCamera();
            otherPlayer.SendSpawnToCamera();

            ModifyNormalBlockToSpawnBlock(localPlayer, GameManager.instance.spawnAllyMaterial);
            ModifyNormalBlockToSpawnBlock(otherPlayer, GameManager.instance.spawnEnemyMaterial);
        } else
        {
            Debug.LogError("Creating the spawn points for the server");
            ModifyNormalBlockToSpawnBlock(Player1);
            ModifyNormalBlockToSpawnBlock(Player2);
        }
        //To activate for perf, desactivate for pf
        transmitter.networkManager = networkManager;
        NetworkManagerHUD hud = FindObjectOfType<NetworkManagerHUD>();
        if (hud != null)
            hud.showGUI = false;
        if (GameManager.instance.isClient)
        {
            SoundHandler.Instance.StartFightMusic();
        }
    }

    /// <summary>
    /// Function called when the states was changed from spawn, reset the variable
    /// </summary>
    private void EndSpawn()
    {
        CharacterToSpawn = null;
    }

    public void TeamSelectDisplay()
    {
        player1.GetComponent<Player>().GetComponent<UIManager>().TeamSelectUI();
        player2.GetComponent<Player>().GetComponent<UIManager>().TeamSelectUI();
    }

    public GameObject GetOtherPlayer(GameObject player)
    {
        if (player != player1)
        {
            return player1;
        }
        else
        {
            return player2;
        }

    }

    public void ResetGrid()
    {
        ResetBatchFolder();
        Grid.instance.GridMatrix = new Placeable[Grid.instance.sizeX, Grid.instance.sizeY, Grid.instance.sizeZ];
        foreach (Transform child in Grid.instance.gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        idPlaceable = new Dictionary<int, NetIdeable>();
        TurnOrder = new List<StackAndPlaceable>();
        //TODO CHECK WITH new spawner
        LivingPlaceable[] livings = FindObjectsOfType<LivingPlaceable>();
        foreach (LivingPlaceable living in livings)
        {
            Destroy(living.gameObject);
        }
    }

    public void CheckWinCondition()
    {
        if (gameMode == GameMode.DEATHMATCH)
        {
            player2.GetComponent<Player>().isWinner = true;
            foreach (LivingPlaceable character in player1.GetComponent<Player>().characters)
            {
                if (!character.IsDead)
                {
                    player2.GetComponent<Player>().isWinner = false;
                    break;
                }
            }
            player1.GetComponent<Player>().isWinner = true;
            foreach (LivingPlaceable character in player2.GetComponent<Player>().characters)
            {
                if (!character.IsDead)
                {
                    player1.GetComponent<Player>().isWinner = false;
                    break;
                }
            }
            WinAndDisableUI();

        }
        if (gameMode == GameMode.FLAG)
        {
            WinAndDisableUI();

        }
    }
    public void WinAndDisableUI()
    {
        if (player1.GetComponent<Player>().isWinner || player2.GetComponent<Player>().isWinner)
        {
            //Disable canvas element
            Canvas c = player1.transform.GetComponentInChildren<Canvas>();
            if (c != null)
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
            State = States.GameOver;
        }
        if (player1.GetComponent<Player>().isWinner && !player2.GetComponent<Player>().isWinner)
        {

            player1.GetComponent<Player>().winText.text = "VICTORY";
            player2.GetComponent<Player>().winText.text = "DEFEAT";
        }
        else if (player2.GetComponent<Player>().isWinner && !player1.GetComponent<Player>().isWinner)
        {

            player2.GetComponent<Player>().winText.text = "VICTORY";
            player1.GetComponent<Player>().winText.text = "DEFEAT";
        }
        else if (player2.GetComponent<Player>().isWinner && player1.GetComponent<Player>().isWinner)
        {
            player2.GetComponent<Player>().winText.text = "DRAW";
            player1.GetComponent<Player>().winText.text = "DRAW";
        }
    }

    public void UpdateTimeline()
    {
        TurnOrder.Clear();
        //add every character once and sort them
        foreach (LivingPlaceable character in player1.GetComponent<Player>().Characters)
        {
            StackAndPlaceable newCharacter = new StackAndPlaceable(character, character.SpeedStack, false);
            TurnOrder.Add(newCharacter);
        }
        foreach (LivingPlaceable character in player2.GetComponent<Player>().Characters)
        {
            StackAndPlaceable newCharacter = new StackAndPlaceable(character, character.SpeedStack, false);
            TurnOrder.Add(newCharacter);
        }
        TurnOrder.Sort((x, y) => x.SpeedStack == y.SpeedStack ? 1 : (int)((x.SpeedStack - y.SpeedStack) / (Mathf.Abs(x.SpeedStack - y.SpeedStack))));

        //Check for every Character if it has to play more than once in the "Grand Turn"

        int numberOfTurns = 1;
        for (int i = 0; i < turnOrder.Count - 1; i++)
        {
            StackAndPlaceable check = turnOrder[i];
            if (!check.SeenBefore)
            {
                for (int j = i + 1; j < turnOrder.Count - 1; j++)
                {
                    float compare = check.SpeedStack + ((numberOfTurns * (j - i)) / check.Character.Speed);
                    if (turnOrder[j].SpeedStack > compare)
                    {
                        turnOrder.Insert(j, new StackAndPlaceable(check.Character, check.SpeedStack + numberOfTurns * (j - i) / check.Character.Speed, true));
                        j++;
                    }
                }
            }
            check.SeenBefore = true;
        }

    }

    public void RefreshBatch(StandardCube block)
    {
        if (block != null && block.batch != null)
        {
            if (block.batch.batchObject == null)
            {
                CreateNewBatch(block.batch);
            }
            if (block.batch.batchObject != null)
            {
                block.batch.batchObject.GetComponent<MeshRenderer>().material = block.GetComponent<MeshRenderer>().material;
                block.batch.batchObject.GetComponent<MeshFilter>().mesh = new Mesh();
                block.batch.batchObject.GetComponent<MeshFilter>().mesh.CombineMeshes(
                block.batch.combineInstances.ToArray(), true, true);
            }
        }
    }

    public void ClickOnPlaceable(Placeable placeable)
    {

        switch (State)
        {
            case States.Spawn:

                if (GetLocalPlayer().Isready == true)
                {
                    return;
                }

                if (!placeable.IsLiving() // If the placeable is a cube                   
                    && Grid.instance.GetSpawnPlayer(GetLocalPlayer()).Contains(placeable.GetPosition() + Vector3Int.up)) // If it is an ally spawnpoint
                {

                    Vector3Int position = placeable.GetPosition();
                    Placeable placeableAbove = Grid.instance.GetPlaceableFromVector(position + Vector3Int.up);


                    if (placeableAbove != null // something above
                        && placeableAbove.IsLiving() // If a living is above
                        && placeableAbove.player == GetLocalPlayer()) // If it is from the our team
                    {
                        if (!CharacterToSpawn)
                        {
                            CharacterToSpawn = (LivingPlaceable)placeableAbove;
                        }
                        else
                        {
                            Grid.instance.SwitchPlaceable(CharacterToSpawn, (LivingPlaceable)placeableAbove);
                            CharacterToSpawn = null;
                        }
                    }
                    else if (placeableAbove == null // If nothing above
                        && CharacterToSpawn != null)
                    {

                        Grid.instance.MovePlaceable(CharacterToSpawn, position + Vector3Int.up);
                        CharacterToSpawn = null;

                    }
                    else
                    {
                        Debug.Log("WHYYYY ? :(");
                    }

                }
                else if (placeable.player == GetLocalPlayer() && placeable.IsLiving())
                {
                    if (!CharacterToSpawn)
                    {
                        CharacterToSpawn = (LivingPlaceable)placeable;
                    }
                    else
                    {
                        Grid.instance.SwitchPlaceable(placeable, CharacterToSpawn);

                        CharacterToSpawn = null;
                    }
                }

                break;

            case States.Move:
                if (placeable.walkable && playingPlaceable.player.isLocalPlayer)
                {
                    Vector3[] path = GetPathFromClicked(placeable);//Check and move on server
                    playingPlaceable.Player.CmdMoveTo(path, playingPlaceable.netId);
                }
                break;

            case States.UseSkill:
                if (ActiveSkill == null)
                {
                    throw new System.NullReferenceException("States is UseSkill but no active skill");
                }

                if (playingPlaceable.Player.isLocalPlayer // If it is the player turn
                    && (playingPlaceable.TargetArea != null && playingPlaceable.TargetArea.Contains(placeable))  // If is a targetable cube
                    || (null != placeable as IHurtable && playingPlaceable.TargetableHurtable.Contains(placeable))) // Or is if a targetable living
                {

                    playingPlaceable.player.OnUseSkill(Player.SkillToNumber(playingPlaceable, ActiveSkill), placeable.netId);

                    RaycastSelector.UnHighligthAndUnPreviewCurrent();
                }
                break;

            default:
                throw new System.NotImplementedException("State not recognize : " + state.ToString());

        }
    }

    /// <summary>
    /// Removes block From Batch
    /// </summary>
    /// <param name="block"></param>
    public void RemoveBlockFromBatch(StandardCube block)
    {
        if (block != null && block.batch != null && block.batch.combineInstances != null)
        {
            block.batch.combineInstances.Remove(block.meshInCombined);
            block.GetComponent<MeshRenderer>().enabled = true;
            RefreshBatch(block);
        }
    }
    /// <summary>
    /// Creates a new batch from the material given, combines instances in       
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
            if (mat.name == batch.material)
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
            Destroy(newBatch.GetComponent<MeshFilter>().mesh);
            Destroy(newBatch.GetComponent<MeshFilter>().sharedMesh);
            Destroy(newBatch);
        }

    }
    public void DestroyAllBatches()
    {
        if(dictionaryMaterialsFilling!=null)
        { 
        foreach (List<Batch> batches in dictionaryMaterialsFilling.Values)
        {
            foreach (Batch batch in batches)
            {
                batch.combineInstances.Clear();
            }
            batches.Clear();
        }
        dictionaryMaterialsFilling.Clear();
        }
        ResetBatchFolder();
    }
    public void ResetBatchFolder()
    {
        foreach (Transform child in batchFolder.transform)
        {
            MeshFilter meshFilter = child.gameObject.GetComponent<MeshFilter>();
            Destroy(meshFilter.sharedMesh);
            Destroy(meshFilter.mesh);
            Destroy(child.gameObject);
        }

    }
    public void  ResetAllBatches()
    {
        Resources.UnloadUnusedAssets();
        GameManager.instance.InitialiseBatchFolder();
    }

    public ObjectOnBloc[] GetObjectsOnBlockUnder(Vector3Int pos)
    {
        return Grid.instance.GridMatrix[pos.x, pos.y - 1, pos.z] != null ? Grid.instance.GridMatrix[pos.x, pos.y - 1, pos.z].transform.Find("Inventory").GetComponentsInChildren<ObjectOnBloc>() : new ObjectOnBloc[0];
    }

    // TODO: rename/supprimer cette fonction, son objectif est incertain (Pierrick)
    public void MoveLogic(List<Vector3> bezierPath)
    {
        if (PlayingPlaceable.Player.isLocalPlayer)
        {
            PlayingPlaceable.ClearAreaOfMovement();

            Vector3 positionChara = playingPlaceable.GetPosition();

            PlayingPlaceable.AreaOfMouvement = Grid.instance.CanGo(positionChara, PlayingPlaceable.CurrentPM,
               PlayingPlaceable.Jump, PlayingPlaceable.Player);
            PlayingPlaceable.HighlightAreaOfMovement();

            PlayingPlaceable.Player.GetComponent<UIManager>().UpdateAbilities(PlayingPlaceable, Vector3Int.FloorToInt(positionChara));
        }
    }


    public void InitStartGameServer()
    {
        List<float> biases = new List<float>();
        for (int i = 0; i < Player1.Characters.Count * 2; i++)
        {
            biases.Add(Random.value/1000);
            Debug.Log(biases[i]);
        }
        List<float> biasesJ1 = new List<float>();
        List<float> biasesJ2 = new List<float>();
        biases.Sort();
        for (int i = 0; i < biases.Count; i++)
        {
            if (i%2 == 0)
            {
                biasesJ1.Add(biases[i]);
            }
            else
            {
                biasesJ2.Add(biases[i]);
            }
        }
        Player1.RpcBiasSpeed(biasesJ1.ToArray());
        Player1.BiasSpeed(biasesJ1.ToArray());
        Player2.RpcBiasSpeed(biasesJ2.ToArray());
        Player2.BiasSpeed(biasesJ2.ToArray());

    }
    /// <summary>
    /// Add current combine instance to its batch
    /// </summary>
    /// <param name="meshFilter"></param>
    /// <param name="combineInstance"></param>
    public void AddMeshToBatches(MeshFilter meshFilter, CombineInstance combineInstance)
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
        if (meshFilter.GetComponent<Placeable>())//Add batch to placeable
        {
            meshFilter.GetComponent<StandardCube>().batch = dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name]
            [dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name].Count - 1];
        }
        dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name]
            [dictionaryMaterialsFilling[meshFilter.GetComponent<MeshRenderer>().material.name].Count - 1].combineInstances.Add(combineInstance);
        // Add the current combine to the last list

        meshFilter.GetComponent<MeshRenderer>().enabled = false;
    }
    public void InitialiseBatchFolder()
    {
        DestroyAllBatches();
        if (dictionaryMaterialsFilling != null)
        {
            dictionaryMaterialsFilling.Clear();
        }
        else
        {
            dictionaryMaterialsFilling = new Dictionary<string, List<Batch>>();
        }

        MeshFilter[] meshFilters = gridFolder.GetComponentsInChildren<MeshFilter>();
        //Todo: if necessary chose them by big cube or something
        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.GetComponent<NetIdeable>() != null && meshFilter.GetComponent<NetIdeable>().shouldBatch 
                && !meshFilter.GetComponent<NetIdeable>().isMoving)
            {
                CombineInstance currentInstance = new CombineInstance
                {
                    mesh = meshFilter.sharedMesh,
                    transform = meshFilter.transform.localToWorldMatrix
                };
                // If it is the first of this material
                AddMeshToBatches(meshFilter, currentInstance);
                if (meshFilter.GetComponent<StandardCube>() != null)
                {
                    meshFilter.GetComponent<StandardCube>().meshInCombined = currentInstance;
                }
            }
        }
        //Then at the end we create all the batches that are not full
        foreach (List<Batch> batches in dictionaryMaterialsFilling.Values)
        {
            CreateNewBatch(batches[batches.Count - 1]); //instanciate last batch
        }
    }

    private IEnumerator WaitForTurnStart(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        BeginningOfTurn();
    }

    /// <summary>
    /// called before the beginning of turn to show the tittle card
    /// </summary>
    public void TransitBetweenTurn()
    {
        State = States.TurnChange;
        numberTurn++;
        UpdateTimeline();
        PlayingPlaceable = TurnOrder[0].Character;
        PlayingPlaceable.SpeedStack += 1 / PlayingPlaceable.Speed;
        if (isClient)
        {
            localPlayer.GetComponent<UIManager>().ShowTurnCard();
        }
        else
        {
            StartCoroutine(WaitForTurnStart(5));
        }
    }

    public void BeginningOfTurn()
    {
        for (int i = PlayingPlaceable.AttachedEffects.Count - 1; i >= 0; i--)
        {
            EffectManager.instance.StartTurnUseEffect(PlayingPlaceable.AttachedEffects[i]);
        }

        if (PlayingPlaceable.IsDead && PlayingPlaceable.TurnsRemaingingCemetery > 0)
        {
            PlayingPlaceable.TurnsRemaingingCemetery--;
            EndOFTurn();
        }
        else
        {
            if (PlayingPlaceable.IsDead)
            {
                PlayingPlaceable.IsDead = false;
                PlayingPlaceable.Player.Respawn(PlayingPlaceable);
            }

            player1.GetComponent<Timer>().StartTimer(timerLength);
            player2.GetComponent<Timer>().StartTimer(timerLength);

            PlayingPlaceable.CurrentPM = PlayingPlaceable.MaxPM;
            PlayingPlaceable.CurrentPA = PlayingPlaceable.PaMax;

            //initialise UI
            // reducing cooldown of skill by 1
            foreach (Skill sk in PlayingPlaceable.Skills)
            {
                if (sk.cooldownTurnLeft > 0)
                {
                    sk.cooldownTurnLeft--;
                }
            }
            if (isClient)
            {
                localPlayer.GetComponent<UIManager>().ChangeTurn();
            }
            State = States.Move;
            
            SetCamera();

            EffectManager.instance.TriggerTotems(PlayingPlaceable);

            PlayingPlaceable.Player.cameraScript.BackToMovement();
        }
    }
    
    public void SetCamera()
    {
        if (isClient)
        {
            if (PlayingPlaceable.Player.isLocalPlayer)
            {
                PlayingPlaceable.Player.cameraScript.SetTarget(PlayingPlaceable.transform);
                PlayingPlaceable.Player.cameraScript.Freecam = 0;
                //GetOtherPlayer(PlayingPlaceable.Player.gameObject).GetComponent<Player>().cameraScript.SetTarget(PlayingPlaceable.GetComponent<Placeable>().gameObject.transform);
                //GetOtherPlayer(PlayingPlaceable.Player.gameObject).GetComponent<Player>().cameraScript.Freecam = 1;
            }
            else
            {
                GetOtherPlayer(PlayingPlaceable.Player.gameObject).GetComponent<Player>().cameraScript.SetTarget(PlayingPlaceable.transform);
                GetOtherPlayer(PlayingPlaceable.Player.gameObject).GetComponent<Player>().cameraScript.Freecam = 1;
            }
        }
    }

    public void EndOFTurn()
    {
        if (winner == null)
        {
            //cleaning and checks and synchro
            Debug.Log("tour suivaaaaaaaaant Area of movement=" + PlayingPlaceable.AreaOfMouvement.Count);
            if (PlayingPlaceable.Player.isLocalPlayer)
            {
                PlayingPlaceable.ResetTargets();
                PlayingPlaceable.ClearAreaOfMovement();
                PlayingPlaceable.ResetHighlightSkill();
                PlayingPlaceable.Player.GetComponent<UIManager>().ResetEndTurn();
                //PlayingPlaceable.Player.cameraScript.Freecam = 1;
                //ResetAllBatches();
            }
            TransitBetweenTurn();
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public NetIdeable FindLocalIdeable(int id)
    {

        return idPlaceable[id];
    }
    public Placeable FindLocalObject(int id)
    {

        return (Placeable)idPlaceable[id];
    }
    public Vector3[] GetPathFromClicked(Placeable arrival)
    {

        NodePath destination = new NodePath(arrival.GetPosition().x, arrival.GetPosition().y, arrival.GetPosition().z, 0, null);
        NodePath inListDestination = PlayingPlaceable.AreaOfMouvement.Find(destination.Equals);
        if (inListDestination != null)
        {
            Vector3[] realPath = inListDestination.GetPathFromStart();
            return realPath;
        }
        Debug.LogError("inlistdest = null");
        return null;
    }
    private void InitialiseCharacter(GameObject charac, GameObject player, Vector3Int spawnCoordinates, string className, int prefabNumber)
    {
        LivingPlaceable charac1 = charac.GetComponent<LivingPlaceable>();

        charac1.Player = player.GetComponent<Player>();
        charac1.Init(prefabNumber);
        Vector3Int posPers = spawnCoordinates;
        Grid.instance.GridMatrix[posPers.x, posPers.y, posPers.z] = charac1;
        charac1.netId = Placeable.currentMaxId;
        //Debug.Log(charac1.netId);
        idPlaceable[charac1.netId] = charac1;
        Placeable.currentMaxId++;
    }

    public void CreateCharacter(GameObject player, Vector3Int spawnCoordinates, int prefaToSpawn)
    {
        Player playerComponent = player.GetComponent<Player>();

        GameObject charac = Instantiate(prefabCharacs[prefaToSpawn], new Vector3(spawnCoordinates.x, spawnCoordinates.y, spawnCoordinates.z), Quaternion.identity);

        InitialiseCharacter(charac, player, spawnCoordinates, GameManager.instance.PossibleCharacters[prefaToSpawn].className, prefaToSpawn);

        playerComponent.characters.Add(charac.GetComponent<LivingPlaceable>());
    }

    //TODO: Use the class component
    /// <summary>
    /// Set the parameters for the blocks in the spawnList of the player.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="material"></param>
    private static void ModifyNormalBlockToSpawnBlock(Player player, Material material = null)
    {
        StandardCube spawnpoint;
        for (int i = 0; i < player.spawnList.Count; i++)
        {
            spawnpoint = (StandardCube)Grid.instance.GetPlaceableFromVector(player.spawnList[i] + Vector3.down);
            if (material != null)
            {
                spawnpoint.oldMaterial = spawnpoint.GetComponent<MeshRenderer>().material;
                spawnpoint.GetComponent<MeshRenderer>().material = material;
            }
            spawnpoint.isConstructableOn = false;
            spawnpoint.isSpawnPoint = true;
            spawnpoint.Destroyable = false;
            spawnpoint.movable = false;
            spawnpoint.gravityType = GravityType.NULL_GRAVITY;
        }
    }

    private void Update()
    {

    }

}
