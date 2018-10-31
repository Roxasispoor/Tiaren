using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Classe centrale gérant le déroulement des tours et répertoriant les objets
/// </summary>
public class GameManager : NetworkBehaviour
{
    //can't be the network manager or isServer can't work
    public static GameManager instance;
    public NetworkManager networkManager;
    private int numberTurn = 0;
    public bool isGameStarted = false;
    public Dictionary<int, Placeable> idPlaceable;
    private Placeable shotPlaceable;
    public GameObject[] prefabCharacs;
    public GameObject[] prefabWeapons;
    public GameObject gridFolder;
    public GameObject player1; //Should be Object

    public GameObject player2; //Should be Object
    public GameObject[] prefabMonsters;
    

    private List<StackAndPlaceable> turnOrder;

    private Player winner;
    public LivingPlaceable playingPlaceable;

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

    public Placeable ShotPlaceable
    {
        get
        {
            return shotPlaceable;
        }

        set
        {
            shotPlaceable = value;
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
        
            Grid.instance.FillGridAndSpawn(gridFolder);
           
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
        
        //    Debug.Log("NOW LOAD MAPS");
        //player1.GetComponent<Player>().RpcLoadMap();//Should call on both clients 
        //player2.GetComponent<Player>().RpcLoadMap();//Should call on both clients 
      //  player1.GetComponent<Player>().RpcCreateCharacters(new Vector3Int(0, 4, 0));
        //player2.GetComponent<Player>().RpcCreateCharacters(new Vector3Int(0, 4, 0));
       isGameStarted = true;

            //Retrieve data 

            //RpcStartGame();
//        BeginningOfTurn();
    
 }
    [ClientRpc]
    public void RpcStartGame()
    {
        isGameStarted = true; 
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


    private void BeginningOfTurn()
    {
        Grid.instance.Gravity();
        Grid.instance.InitializeExplored(false);

        UpdateTimeline();
        playingPlaceable = TurnOrder[0].Character;
        playingPlaceable.SpeedStack += 1 / playingPlaceable.Speed;
        if (playingPlaceable.IsDead)
        {
            playingPlaceable.TurnsRemaingingCemetery--;
            BeginningOfTurn();
        }
        else
        {
            //initialise character fields

            // reducing cooldown of skill by 1
            foreach (Skill sk in playingPlaceable.Skills)
            {
                if (sk.TourCooldownLeft > 0)
                {
                    sk.TourCooldownLeft--;
                }
            }
            if(isClient)
            {
                playingPlaceable.player.cameraScript.target = playingPlaceable.gameObject.transform;
            }
//            playingPlaceable.player.RpcSetCamera(playingPlaceable.netId);
            playingPlaceable.CurrentPM = playingPlaceable.MaxPM;
            playingPlaceable.CurrentPA = playingPlaceable.PaMax;
            playingPlaceable.Player.clock.IsFinished = false;
            playingPlaceable.Player.clock.StartTimer(30f);
  //          playingPlaceable.Player.RpcStartTimer(30f);
        }
    }

    public void EndOFTurn()
    {
        //cleaning and checks and synchro with banana dancing if needed
  
        BeginningOfTurn();
    }

    //TODO : MANAGE SKILL CREATION AND USAGE (WAITING FOR SKILL'S PROPER IMPLEMENTATION)
    //Make a copy from model skill in skills, and fill with additional info (caster, targets)
    public void UseSkill(int skillID, LivingPlaceable caster, List<Placeable> targets)
    {
        Skill skill = playingPlaceable.Skills[skillID];
        skill.Use();
    }
    public Placeable FindLocalObject(int id)
    {

        return idPlaceable[id];
    }
    public void CheckIfAccessible(Placeable arrival)
    {
        NodePath destination = new NodePath(arrival.GetPosition().x, arrival.GetPosition().y, arrival.GetPosition().z, 0, null);
        Vector3[] realPath = playingPlaceable.AreaOfMouvement.Find(destination.Equals).getFullPath();
        if (playingPlaceable.AreaOfMouvement.Contains(destination))
        {
            playingPlaceable.Player.CmdMoveTo(realPath);
            List<Vector3> bezierPath = new List<Vector3>(realPath);
            playingPlaceable.player.MoveAlongBezier(bezierPath, playingPlaceable, playingPlaceable.AnimationSpeed);
        }
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

   /* [ClientRpc]
    public void RpcCreatePerso(GameObject charac, GameObject player, Vector3 spawnCoordinates)
    {
        Debug.Log("Please create chars");
        Vector3Int realCoordinates = new Vector3Int((int)spawnCoordinates.x, (int)spawnCoordinates.y, (int)spawnCoordinates.z);
        InitialiseCharacter(charac, player, realCoordinates);
    }*/

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
