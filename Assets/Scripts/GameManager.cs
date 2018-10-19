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

    private Placeable shotPlaceable;
    public GameObject[] prefabCharacs;
    public GameObject[] prefabWeapons;
    public GameObject gridFolder;
    public GameObject player1; //Should be Object

    public GameObject player2; //Should be Object
    public GameObject[] prefabMonsters;
    

    private List<StackAndPlaceable> turnOrder;

    private Player winner;
    private LivingPlaceable playingPlaceable;

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



        TurnOrder = new List<StackAndPlaceable>();
        //    DontDestroyOnLoad(gameObject);
        //Initialize la valeur statique de chaque placeable, devrait rester identique entre deux versions du jeu, et ne pas poser problème si les new prefabs sont bien rajoutés a la fin
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
    [Server]
    IEnumerator Start()
    {
        //PHASE 0 : SET THE GAME UP
        if (isServer)
        {
            //If you want to create one and save it
            // Grid.instance.CreateRandomGrid(gridFolder);
            //Grid.instance.SaveGridFile();

            //If you want to load one
            Grid.instance.FillGridAndSpawn(gridFolder);
        }
        while (player1 == null)
        {
            yield return null;
        }

        CreateCharacters(player1, new Vector3Int(0, 4, 0));
        while (player2 == null)
        {
            yield return null;
        }
        CreateCharacters(player2, new Vector3Int(3, 4, 0));

        Grid.instance.Gravity();
        Grid.instance.InitializeExplored(false);
        BeginningOfTurn();
    }
    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    /*

            //PHASE 1: BEGINING OF TURN
            //Sort by speedstack, add inverse of speed each time it plays
            playingPlaceable = TurnOrder[0];
            playingPlaceable.SpeedStack += 1 / playingPlaceable.Speed;
            if (TurnOrder.Count != 0 && playingPlaceable.Player != null)
            {
                playingPlaceable.player.RpcSetCamera(playingPlaceable.netId);
                playingPlaceable.CurrentPM = playingPlaceable.MaxPM;
                playingPlaceable.TimesFiredThisTurn = 0;
                //Here user can act during 30s

                if (!playingPlaceable.IsDead)
                {
                    playingPlaceable.Player.clock.IsFinished = false;
                    playingPlaceable.Player.clock.StartTimer(30f);
                    playingPlaceable.Player.RpcStartTimer(30f);


                    playingPlaceable.Player.EndPhase = false;
                    //TODO select spawn place
                    Vector3Int positionDepart = playingPlaceable.GetPosition() - new Vector3Int(0, 1, 0);//pathfinding start under the character

                    DistanceAndParent[,,] inPlace = Grid.instance.CanGo(playingPlaceable, playingPlaceable.MaxPM, positionDepart);
                    Vector3Int vecTest = new Vector3Int(-1, -1, -1);
                    while (!playingPlaceable.Player.EndPhase && !playingPlaceable.Player.clock.IsFinished && (playingPlaceable.TimesFiredThisTurn < 1 || playingPlaceable.CurrentPM > 0))
                    {
                        if (shotPlaceable != null && playingPlaceable.CapacityinUse == 0 && shotPlaceable != playingPlaceable && playingPlaceable.CanHit(shotPlaceable).Count > 0)// si il se tire pas dessus et qu'il a bien sélectionné quelqu'un
                        {
                            Vector3 thePlaceToShoot = playingPlaceable.ShootDamage(shotPlaceable); // TODO: for animation
                        }
                        else if (shotPlaceable != null && playingPlaceable.CapacityinUse != 0 && playingPlaceable.Skills[playingPlaceable.CapacityinUse].TourCooldownLeft == 0 &&
                            playingPlaceable.Skills[playingPlaceable.CapacityinUse].SkillType == SkillType.ONECLICKLIVING
                          
                            && shotPlaceable.GetType() == typeof(LivingPlaceable) 
                            && playingPlaceable.Skills[playingPlaceable.CapacityinUse].condition())
                        {
                            playingPlaceable.Skills[playingPlaceable.CapacityinUse].Use();

                        }
                        else if (shotPlaceable == playingPlaceable && playingPlaceable.CapacityinUse != 0 && playingPlaceable.Skills[playingPlaceable.CapacityinUse].TourCooldownLeft == 0 &&
                            playingPlaceable.Skills[playingPlaceable.CapacityinUse].SkillType == SkillType.SELFSKILL
                             
                            && playingPlaceable.Skills[playingPlaceable.CapacityinUse].condition())
                        {
                            playingPlaceable.Skills[playingPlaceable.CapacityinUse].Use();
                          
                        }
                        else if (shotPlaceable != null && playingPlaceable.CapacityinUse != 0 && playingPlaceable.Skills[playingPlaceable.CapacityinUse].TourCooldownLeft == 0 &&
                          playingPlaceable.Skills[playingPlaceable.CapacityinUse].SkillType == SkillType.ONECLICKPLACEABLE
                         
                          && shotPlaceable.GetType() == typeof(Placeable)
                          && playingPlaceable.Skills[playingPlaceable.CapacityinUse].condition())
                        {
                            playingPlaceable.Skills[playingPlaceable.CapacityinUse].Use();
                          
                        }


                        else if (playingPlaceable.player.PlaceToGo != vecTest &&
                            inPlace[playingPlaceable.player.PlaceToGo.x, playingPlaceable.player.PlaceToGo.y, playingPlaceable.player.PlaceToGo.z].GetDistance() > 0
                            && inPlace[playingPlaceable.player.PlaceToGo.x, playingPlaceable.player.PlaceToGo.y, playingPlaceable.player.PlaceToGo.z].GetDistance() <= playingPlaceable.CurrentPM)
                        {
                            List<Vector3> listAnimator = new List<Vector3>();
                            DistanceAndParent parent = inPlace[playingPlaceable.player.PlaceToGo.x, playingPlaceable.player.PlaceToGo.y, playingPlaceable.player.PlaceToGo.z];
                            listAnimator.Add(parent.Pos);
                            while (parent.GetDistance() > 1)
                            {

                                parent = parent.GetParent();
                                listAnimator.Add(parent.Pos);
                            }
                            listAnimator.Add(playingPlaceable.GetPosition() - new Vector3Int(0, 1, 0)); //we want the position right below
                            RpcMoveAlongBezier(listAnimator.ToArray(), playingPlaceable.netId, 1);
                            playingPlaceable.CurrentPM -= listAnimator.Count - 1;

                            //actualize position
                            Grid.instance.GridMatrix[playingPlaceable.player.PlaceToGo.x, playingPlaceable.player.PlaceToGo.y + 1, playingPlaceable.player.PlaceToGo.z] = playingPlaceable;
                            Grid.instance.GridMatrix[playingPlaceable.GetPosition().x, playingPlaceable.GetPosition().y, playingPlaceable.GetPosition().z] = null; // we're not at the former position anymore
                            
                            //erase formers
                            PlayingPlaceable.Player.RpcChangeBackColor();

                            //moving => actualize what's seen. set vectest to 0 
                            inPlace = Grid.instance.CanGo(playingPlaceable, playingPlaceable.MaxPM, playingPlaceable.player.PlaceToGo);
                            playingPlaceable.player.PlaceToGo = vecTest;

                        }
                        yield return null;

                    }

                    //TODO :MOVE IN TOP POSITION
                    // reducing cooldown of skill by 1
                    foreach (Skill sk in playingPlaceable.Skills)
                    {
                        if (sk.TourCooldownLeft > 0)
                        {
                            sk.TourCooldownLeft--;
                        }
                    }
                    shotPlaceable = null;
                    //erasing blue ones still active at the end
                    PlayingPlaceable.Player.RpcChangeBackColor();

                    playingPlaceable.player.PlaceToGo = vecTest;


                    playingPlaceable.Player.clock.IsFinished = false;

                    //apply changes
 
                }
                else
                {
                    //fast forward to the next character
                    playingPlaceable.TurnsRemaingingCemetery--;
                }



             

            }
            else
            {
                yield return null;
            }


        }
    }
    */

    private void UpdateTimeline()
    {
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
                    if (turnOrder[j].SpeedStack > check.SpeedStack + numberOfTurns * 1 / check.Character.Speed)
                    {
                        turnOrder.Insert(j, new StackAndPlaceable(check.Character, check.SpeedStack + numberOfTurns * 1 / check.Character.Speed, true));
                        j++;
                    }
                }
            }
        }

    }


    private void BeginningOfTurn()
    {
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
            playingPlaceable.player.RpcSetCamera(playingPlaceable.netId);
            playingPlaceable.CurrentPM = playingPlaceable.MaxPM;
            playingPlaceable.CurrentPA = playingPlaceable.PaMax;
            playingPlaceable.Player.clock.IsFinished = false;
            playingPlaceable.Player.clock.StartTimer(30f);
            playingPlaceable.Player.RpcStartTimer(30f);
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
    }

    private void CreateCharacters(GameObject player, Vector3Int spawnCoordinates)
    {
        for (int i = 0; i < player.GetComponent<Player>().NumberPrefab.Count; i++)
        {
            GameObject charac = Instantiate(prefabCharacs[player.GetComponent<Player>().NumberPrefab[i]], new Vector3(0, 4f, 0), Quaternion.identity);

            InitialiseCharacter(charac, player, spawnCoordinates);

            NetworkServer.Spawn(charac);
            Vector3 realCoordinates = new Vector3Int(spawnCoordinates.x, spawnCoordinates.y, spawnCoordinates.z);
            RpcCreatePerso(charac, player, realCoordinates);

        }

    }

    [ClientRpc]
    public void RpcCreatePerso(GameObject charac, GameObject player, Vector3 spawnCoordinates)
    {
        Vector3Int realCoordinates = new Vector3Int((int)spawnCoordinates.x, (int)spawnCoordinates.y, (int)spawnCoordinates.z);
        InitialiseCharacter(charac, player, realCoordinates);
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


    // Update is called once per frame
    void Update()
    {


    }
}
