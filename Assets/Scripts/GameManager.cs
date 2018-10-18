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
    


    /// <summary>
    /// See https://www.gamedev.net/forums/topic/551455-length-of-a-generalized-quadratic-bezier-curve-in-3d/
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    public float LengthQuadraticBezier(Vector3 start, Vector3 end, Vector3 control)
    {
        Vector3[] C = { start, control, end };
        // ASSERT:  C[0], C[1], and C[2] are distinct points.

        // The position is the following vector-valued function for 0 <= t <= 1.
        //   P(t) = (1-t)^2*C[0] + 2*(1-t)*t*C[1] + t^2*C[2].
        // The derivative is
        //   P'(t) = -2*(1-t)*C[0] + 2*(1-2*t)*C[1] + 2*t*C[2]
        //         = 2*(C[0] - 2*C[1] + C[2])*t + 2*(C[1] - C[0])
        //         = 2*A[1]*t + 2*A[0]
        // The squared length of the derivative is
        //   f(t) = 4*Dot(A[1],A[1])*t^2 + 8*Dot(A[0],A[1])*t + 4*Dot(A[0],A[0])
        // The length of the curve is
        //   integral[0,1] sqrt(f(t)) dt

        Vector3[] A ={ C[1] - C[0],  // A[0] not zero by assumption
        C[0] - 2.0f*C[1] + C[2]
    };

        float length;

        if (A[1] != Vector3.zero)
        {
            // Coefficients of f(t) = c*t^2 + b*t + a.
            float c = 4.0f * Vector3.Dot(A[1], A[1]);  // c > 0 to be in this block of code
            float b = 8.0f * Vector3.Dot(A[0], A[1]);
            float a = 4.0f * Vector3.Dot(A[0], A[0]);  // a > 0 by assumption
            float q = 4.0f * a * c - b * b;  // = 16*|Cross(A0,A1)| >= 0

            // Antiderivative of sqrt(c*t^2 + b*t + a) is
            // F(t) = (2*c*t + b)*sqrt(c*t^2 + b*t + a)/(4*c)
            //   + (q/(8*c^{3/2}))*log(2*sqrt(c*(c*t^2 + b*t + a)) + 2*c*t + b)
            // Integral is F(1) - F(0).

            float twoCpB = 2.0f * c + b;
            float sumCBA = c + b + a;
            float mult0 = 0.25f / c;
            float mult1 = q / (8.0f * Mathf.Pow(c, 1.5f));
            length =
                mult0 * (twoCpB * Mathf.Sqrt(sumCBA) - b * Mathf.Sqrt(a)) +
                mult1 * (Mathf.Log(2.0f * Mathf.Sqrt(c * sumCBA) + twoCpB) - Mathf.Log(2.0f * Mathf.Sqrt(c * a) + b));
        }
        else
        {
            length = 2.0f * A[0].magnitude;
        }

        return length;
    }
    /// <summary>
    /// Unused Hacky way to approximate bezierr length
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    public float LengthBezierApprox(Vector3 start, Vector3 end, Vector3 control)
    {
        float chord = Vector3.Distance(end, start);
        float cont = Vector3.Distance(start, control) + Vector3.Distance(control, end);
        return (cont + chord) / 2;
    }
    /// <summary>
    /// Calculate distance of bezier, update isBezier and the needed controlPoint
    /// </summary>
    /// <param name="start"></param>
    /// <param name="nextNode"></param>
    /// <param name="isBezier"></param>
    /// <returns></returns>
    public float CalculateDistance(Vector3 start, Vector3 nextNode, ref bool isBezier, ref Vector3 controlPoint)
    {
        isBezier = start.y != nextNode.y;
        if (isBezier)// if difference of height
        {
            controlPoint = (nextNode + start + 2 * new Vector3(0, Mathf.Abs(nextNode.y - start.y), 0)) / 2;

            return LengthQuadraticBezier(start, nextNode, controlPoint);
        }
        else
        {

            return Vector3.Distance(start, nextNode);
        }

    }
    [ClientRpc]
    public void RpcMoveAlongBezier(Vector3[] path, NetworkInstanceId placeable, float speed)
    {
        GameObject plc = ClientScene.FindLocalObject(placeable);

        List<Vector3> pathe = new List<Vector3>(path);
        StartCoroutine(MoveAlongBezier(pathe, plc.GetComponent<Placeable>(), speed));
    }


    [Client]
    public IEnumerator MoveAlongBezier(List<Vector3> path, Placeable placeable, float speed)
    {
        float timeBezier = 0f;
        Vector3 delta = placeable.transform.position - path[path.Count - 1];
        Vector3 startPosition = path[path.Count - 1];
        Vector3 controlPoint = new Vector3();
        bool isBezier = true;
        //For visual rotation
        Vector3 targetDir = path[path.Count - 2] - placeable.transform.position;
        targetDir.y = 0;

        int i = path.Count - 2;

        float distance = CalculateDistance(startPosition, path[i], ref isBezier, ref controlPoint);
        float distanceParcourue = 0;
        while (timeBezier < 1)
        {
            distanceParcourue += (speed * Time.deltaTime);
            timeBezier = distanceParcourue / distance;

            while (timeBezier > 1 && i > 0) //on go through 
            {


                distanceParcourue -= distance;
                startPosition = path[i];
                i--;
                targetDir = path[i] - placeable.transform.position;//next one
                targetDir.y = 0;// don't move up 

                distance = CalculateDistance(startPosition, path[i], ref isBezier, ref controlPoint);//On calcule la distance au noeud suivant
                timeBezier = distanceParcourue / distance; //on recalcule

            }
            if (i == 0 && timeBezier > 1)
            {
                // arrived to the last node of path, in precedent loop
                placeable.transform.position = path[i] + delta;
                // exit yield loop
                break;
            }
            if (isBezier)
            {
                placeable.transform.position = delta + Mathf.Pow(1 - timeBezier, 2) * (startPosition) + 2 * (1 - timeBezier) * timeBezier * (controlPoint) + Mathf.Pow(timeBezier, 2) * (path[i]);

            }
            else
            {
                placeable.transform.position = Vector3.Lerp(startPosition + delta, path[i] + delta, timeBezier);

            }
            Vector3 vectorRotation = Vector3.RotateTowards(placeable.transform.forward, targetDir, 0.2f, 0);
            placeable.transform.rotation = Quaternion.LookRotation(vectorRotation);
            //change what we look at

            yield return null;


        }

    }


    /// <summary>
    /// moving character across the path
    /// déplace le characonnage le long du chemin
    /// </summary>
    /// <param name="path"></param>
    /// <param name="placeable"></param>
    /// <returns></returns>
    public IEnumerator ApplyMove(List<Vector3> path, Placeable placeable)
    {
        Vector3 delta = placeable.transform.position - path[path.Count - 1];
        float speed = 1;
        // distance traveled since starting position
        float travelDistance = 0;

        
        Vector3 startPosition = path[path.Count - 1] + delta;

        Vector3 targetDir = path[path.Count - 2] - placeable.transform.position;
        targetDir.y = 0;
        // loop on all points of the path
        for (int i = path.Count - 2, count = path.Count, lastIndex = 0; i >= 0; i--)
        {
            //targetDir = path[i] - placeable.transform.position;

            // distance between starting Position and arrival position (current node, following node)
            float distance = Vector3.Distance(startPosition, path[i] + delta);

            // direction vector between those two
            Vector3 direction = (path[i] + delta - startPosition).normalized;

            // looping while position of following node has not been passed
            while (travelDistance < distance)
            {
                // advance in function of speed and time past
                travelDistance += (speed * Time.deltaTime);

                // if arrival node position has been reached / passed
                if (travelDistance >= distance)
                {

                    // if we're still on our way 
                    if (i > lastIndex)
                    {
                        targetDir = path[i - 1] - placeable.transform.position;
                        targetDir.y = 0;
                        // positioning a few far away on the way
                        // between the two following nodes, depending on passed distance beyond current arrival node
                        float distanceNext = Vector3.Distance(path[i - 1], path[i]);

                        float ratio = (travelDistance - distance) / distanceNext;

                        // if ration > 1, 
                        // then passed distance >distance between the two following nodes 
                        // loop jump all nodes we're supposed to have gone through shifting with highspeed
                        while (ratio > 1)
                        {
                            i--;
                            if (i == lastIndex)
                            {
                                // arrived to the last node of the path
                                placeable.transform.position = path[i] + delta;
                                // ending loop
                                break;
                            }
                            else
                            {
                                travelDistance -= distance;
                                distance = distanceNext;
                                distanceNext = Vector3.Distance(path[i - 1], path[i]);
                                ratio = (travelDistance - distance) / distanceNext;
                            }
                        }

                        if (i == lastIndex)
                        {
                            // arrived to the last node of path during former while
                            break;
                        }
                        else
                        {

                            transform.position = Vector3.Lerp(path[i], path[i - 1], ratio);
                        }

                    }
                    else
                    {
                        // arrive to last node of the path
                        placeable.transform.position = path[i] + delta;

                        break;
                    }
                }
                else
                {
                    // going in direction of the arrival position
                    placeable.transform.position += direction * (speed * Time.deltaTime);
                }

                Vector3 vectorRotation = Vector3.RotateTowards(placeable.transform.forward, targetDir, 0.2f, 0);
                placeable.transform.rotation = Quaternion.LookRotation(vectorRotation);

                yield return null;
            }
            // substract distance to run between two former nodes
            travelDistance -= distance;

            // updating starting position for next iteration
            startPosition = path[i] + delta;
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

    [Server]
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
