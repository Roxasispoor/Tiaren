using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Represents a player
/// </summary>
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool acted;
    [SyncVar]
    private int score;
    public bool isReadyToPlay = false;
    public List<GameObject> characters = new List<GameObject>();
    public List<int> numberPrefab;
    private Vector3Int placeToGo;
    private bool isready;
    public bool isWinner = false;
    public Text winText;

    private Placeable shotPlaceable;
    public CameraScript cameraScript;
    public delegate float Axis();
    public delegate bool Condition();
    private Dictionary<string, Axis> dicoAxis;
    private Dictionary<string, Condition> dicoCondition;
    public Timer clock;

    /// <summary>
    /// Function to know if the player acted during this turn
    /// </summary>
    public bool Acted
    {
        get
        {
            return acted;
        }

        set
        {
            acted = value;
        }
    }

    /// <summary>
    /// Score of player
    /// </summary>
    public int Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
        }
    }

    public List<GameObject> Characters
    {
        get
        {
            return characters;
        }

        set
        {
            characters = value;
        }
    }

    public List<int> NumberPrefab
    {
        get
        {
            return numberPrefab;
        }

        set
        {
            numberPrefab = value;
        }
    }



    public Vector3Int PlaceToGo
    {
        get
        {
            return placeToGo;
        }

        set
        {
            placeToGo = value;
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



    public Dictionary<string, Axis> DicoAxis
    {
        get
        {
            return dicoAxis;
        }

        set
        {
            dicoAxis = value;
        }
    }

    public Dictionary<string, Condition> DicoCondition
    {
        get
        {
            return dicoCondition;
        }

        set
        {
            dicoCondition = value;
        }
    }

    public bool Isready
    {
        get
        {
            return isready;
        }

        set
        {
            isready = value;
        }
    }



    /// <summary>
    /// List of characters of the player
    /// </summary>

    private void Awake()
    {
        clock = GetComponent<Timer>();
        DicoAxis = new Dictionary<string, Axis>();
        DicoCondition = new Dictionary<string, Condition>();
        DicoAxis.Add("AxisXCamera", () => Input.GetAxis("Mouse X"));
        DicoAxis.Add("AxisYCamera", () => Input.GetAxis("Mouse Y"));
        DicoAxis.Add("AxisZoomCamera", () => Input.GetAxis("Mouse ScrollWheel"));
        DicoCondition.Add("BackToMovement", () => Input.GetButtonDown("BackToMovement"));
        DicoCondition.Add("OrbitCamera", () => Input.GetMouseButton(1));
        DicoCondition.Add("PanCamera", () => Input.GetMouseButton(2));
        Isready = false;
    }

    // Use this for initialization
    void Start()
    {
        if (GameManager.instance.player1 == null)
        {
            GameManager.instance.player1 = gameObject;
        }
        else
        {
            GameManager.instance.player2 = gameObject;
         }
        if (isLocalPlayer)
        {
            Debug.Log("STARTCLIENT!");
        }
    }


    public void displaySpawn()
    {
        gameObject.GetComponent<UIManager>().SpawnUI();
        for (int i = 0; i < Grid.instance.SpawnPlayer1.Count; i++)
        {
            Grid.instance.GridMatrix[Grid.instance.SpawnPlayer1[i].x, Grid.instance.SpawnPlayer1[i].y - 1,
            Grid.instance.SpawnPlayer1[i].z].GetComponent<MeshRenderer>().material = GameManager.instance.spawnMaterial;
            if (i < gameObject.GetComponent<UIManager>().CurrentCharacters.Count)
            {
                GameManager.instance.CreateCharacter(gameObject, Grid.instance.SpawnPlayer1[i], gameObject.GetComponent<UIManager>().CurrentCharacters[i]);
            }
            if (gameObject == GameManager.instance.player2)
            {
                Characters[i].SetActive(false);
            }
        }
        for (int i = 0; i < Grid.instance.SpawnPlayer2.Count; i++)
        {
            Grid.instance.GridMatrix[Grid.instance.SpawnPlayer1[i].x, Grid.instance.SpawnPlayer1[i].y - 1,
            Grid.instance.SpawnPlayer1[i].z].GetComponent<MeshRenderer>().material = GameManager.instance.spawnMaterial;
            if (i < gameObject.GetComponent<UIManager>().CurrentCharacters.Count)
            {
                GameManager.instance.CreateCharacter(gameObject, Grid.instance.SpawnPlayer1[i], gameObject.GetComponent<UIManager>().CurrentCharacters[i]);
            }
            if (gameObject == GameManager.instance.player2)
            {
                Characters[i].SetActive(false);
            }
        }
    }


    public void GameReady()
    {
        Isready = true;
        Vector3[] positions = new Vector3[Characters.Count];
        int[] ids = new int[Characters.Count];
        for(int i = 0; i < Characters.Count; i++)
        {
            positions[i] = Characters[i].transform.position;
            ids[i] = Characters[i].GetComponent<LivingPlaceable>().netId;
        }
        CmdGameReady(positions, ids);
    }


    [Command]
    public void CmdGameReady(Vector3[] positions, int[] ids)
    {
        Isready = true;
        if (GameManager.instance.player1.GetComponent<Player>().Isready && GameManager.instance.player2.GetComponent<Player>().Isready)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                Vector3Int groundSpace = new Vector3Int((int)positions[i].x, (int)positions[i].y - 1, (int)positions[i].z);
                Vector3Int space = new Vector3Int((int)positions[i].x, (int)positions[i].y, (int)positions[i].z);
                if(Grid.instance.SpawnPlayer1.Find(groundSpace.Equals) == null)
                {
                    Debug.Log("error in character " + i.ToString() + "of player" + this.ToString());
                }
                Grid.instance.MoveBlock(GameManager.instance.FindLocalObject(ids[i]), space, true);
            }
            GameManager.instance.IsGameStarted = true;
            GameManager.instance.BeginningOfTurn();
        }
    }

    private void Update()
    {
        if (isServer && GameManager.instance.player1 != null && GameManager.instance.player2 != null)
        {
            if (clock.IsFinished && GameManager.instance.playingPlaceable && GameManager.instance.playingPlaceable.Player==this)
            {
               RpcEndTurn(); //permet une resynchronisation au rythme server
               GameManager.instance.EndOFTurn();
            }
        }
    }
    [ClientRpc]
    public void RpcEndTurn()
    {
        Debug.Log("Oui chef, mon tour est fini!");
        GameManager.instance.EndOFTurn();
    }

    //launcher for end of turn
    public void EndTurn()
    {
        Debug.Log("EndTurn asked");
        CmdEndTurn();
    }

    [Command]
    private void CmdEndTurn()
    {
        GameManager.instance.EndOFTurn();
        RpcEndTurn();
    }

    // A useless player actually acts, but the timer is unactive and unlinked to nothing on the canvas
    [ClientRpc]
    public void RpcStartTimer(float time)
    {

        clock.StartTimer(time);
    }


    public void ShowSkillEffectTarget(LivingPlaceable playingPlaceable, Skill skill)
    {
        GameManager.instance.playingPlaceable.ResetAreaOfMovement();
        if (skill.SkillType==SkillType.BLOCK)
        {

            List<Vector3Int> vect= Grid.instance.HighlightTargetableBlocks(playingPlaceable.transform.position, skill.Minrange, skill.Maxrange);
            foreach(Vector3Int v3 in vect)
            {
                playingPlaceable.TargetArea.Add(Grid.instance.GridMatrix[v3.x,v3.y,v3.z]);
            }
            //playingPlaceable.TargetArea = 


            playingPlaceable.ResetAreaOfMovement();
            playingPlaceable.ChangeMaterialAreaOfTarget(GameManager.instance.targetMaterial);
        }
        else if (skill.SkillType == SkillType.LIVING)
        {
            playingPlaceable.TargetableUnits = Grid.instance.HighlightTargetableLiving(playingPlaceable.transform.position, skill.Minrange, skill.Maxrange);
        }
        else
        {
            playingPlaceable.GetComponent<Renderer>().material.color = Color.red;
        }

    }


    [Command]
    public void CmdMoveTo(Vector3[] path)
    {
        Debug.Log("CheckPath" + Grid.instance.CheckPath(path, GameManager.instance.playingPlaceable));
        if (GameManager.instance.PlayingPlaceable.player == this )// updating only if it's his turn to play, other checkings are done in GameManager
        {
            //Move  placeable
            Debug.Log("Start" + GameManager.instance.playingPlaceable.GetPosition());
            Grid.instance.GridMatrix[GameManager.instance.playingPlaceable.GetPosition().x, GameManager.instance.playingPlaceable.GetPosition().y,
                GameManager.instance.playingPlaceable.GetPosition().z] = null;
            Grid.instance.GridMatrix[(int)path[path.Length - 1].x, (int)path[path.Length - 1].y + 1,
                (int)path[path.Length - 1].z] = GameManager.instance.playingPlaceable;

            GameManager.instance.playingPlaceable.transform.position = path[path.Length - 1] + new Vector3(0, 1, 0);
            //Trigger effect the ones after the others
            foreach (Vector3 current in path)
            {
                //Grid.instance.GridMatrix[(int)current.x, (int)current.y, (int)current.z].OnWalk()
            }
            GameManager.instance.playingPlaceable.CurrentPM -= path.Length - 1;
            RpcMoveTo(path);



        }
    }
    [ClientRpc]
    public void RpcMoveTo(Vector3[] path)
    {

        //List<Vector3> bezierPath = new List<Vector3>(realPath);
        GameManager.instance.playingPlaceable.CurrentPM -= path.Length - 1;
        List<Vector3> bezierPath=new List<Vector3>(path);

        Debug.Log("CheckPath(): " + Grid.instance.CheckPath(path, GameManager.instance.playingPlaceable));
        Grid.instance.GridMatrix[GameManager.instance.playingPlaceable.GetPosition().x, GameManager.instance.playingPlaceable.GetPosition().y,
             GameManager.instance.playingPlaceable.GetPosition().z] = null;
        Grid.instance.GridMatrix[(int)path[path.Length - 1].x, (int)path[path.Length - 1].y + 1,
            (int)path[path.Length - 1].z] = GameManager.instance.playingPlaceable;

        StartCoroutine(Player.MoveAlongBezier(bezierPath, GameManager.instance.playingPlaceable, GameManager.instance.playingPlaceable.AnimationSpeed));
        GameManager.instance.MoveLogic(bezierPath);
        
        
    }
    

    [ClientRpc]
    public void RpcSetCamera(int mustPlay)
    {
        Placeable potential = GameManager.instance.FindLocalObject(mustPlay).GetComponent<Placeable>();

        this.cameraScript.target = potential.gameObject.transform;

    }
   
    
    public override void OnStartLocalPlayer()
    {
        transform.Find("Main Camera").gameObject.SetActive(true);
        transform.Find("TeamCanvas").gameObject.SetActive(true);
    }
    
    public void DispatchSkill(int skillID, LivingPlaceable caster, List<Placeable> targets)
    {
        Skill skill = caster.Skills[skillID];
        if (skill.Cooldown == 0 && GameManager.instance.PlayingPlaceable == caster)
        {
            GameManager.instance.UseSkill(skillID, caster, targets);
        }
    }


    //MOVING WITH BEZIER

    /// <summary>
    /// See https://www.gamedev.net/forums/topic/551455-length-of-a-generalized-quadratic-bezier-curve-in-3d/
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    public static float LengthQuadraticBezier(Vector3 start, Vector3 end, Vector3 control)
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
    /// Unused Hacky way to approximate bezier length
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
    public static float CalculateDistance(Vector3 start, Vector3 nextNode, ref bool isBezier, ref Vector3 controlPoint)
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

    [Client]
    public void StartMoveAlongBezier(List<Vector3> path, Placeable placeable, float speed)
    {
         if(path.Count>1)
        { 
        StartCoroutine(MoveAlongBezier(path, placeable, speed));
         }
    }

    [Client]
    
    // ONLY FOR CHARACTER
    public static IEnumerator MoveAlongBezier(List<Vector3> path, LivingPlaceable placeable, float speed)
    {
        if (path.Count < 2)
        {
            yield break;
        }
        float timeBezier = 0f;
        Vector3 delta = placeable.transform.position - path[0];
        Vector3 startPosition = path[0];
        Vector3 controlPoint = new Vector3();
        bool isBezier = true;


        Animator anim = placeable.gameObject.GetComponent<Animator>();
        bool isJumping = false;
        
        //For visual rotation
        Vector3 targetDir = path[1] - placeable.transform.position;
        targetDir.y = 0;

        int i = 1;
        int iref = 1;
        float distance = CalculateDistance(startPosition, path[i], ref isBezier, ref controlPoint);
        float distanceParcourue = 0;
        while (timeBezier < 1)
        {
            distanceParcourue += (speed * Time.deltaTime);
            timeBezier = distanceParcourue / distance;

            while (timeBezier > 1 && i < path.Count - 1) //on go through 
            {


                distanceParcourue -= distance;
                startPosition = path[i];
                i++; // changing movement
                targetDir = path[i] - placeable.transform.position;//next one
                targetDir.y = 0;// don't move up 

                distance = CalculateDistance(startPosition, path[i], ref isBezier, ref controlPoint);//On calcule la distance au noeud suivant
                timeBezier = distanceParcourue / distance; //on recalcule

            }
            if (i == path.Count - 1 && timeBezier > 1)
            {
                // arrived to the last node of path, in precedent loop
                placeable.transform.position = path[i] + delta;
                // exit yield loop
                break;
            }

            Vector3 vectorRotation = Vector3.RotateTowards(placeable.transform.forward, targetDir, 0.2f, 0);
            placeable.transform.rotation = Quaternion.LookRotation(vectorRotation);
            //change what we look at
            
            if (isBezier)
            {
                if (!isJumping)
                {
                    isJumping = true;
                    iref = i;
                    anim.SetTrigger("jump");
                    anim.ResetTrigger("walk");
                    yield return new WaitForSeconds(0.3f);

                }
                else
                {
                    if (i != iref)
                    {
                        iref = i;
                        anim.SetTrigger("landAndJump");
                        yield return new WaitForSeconds(0.3f);
                        
                    }
                }
                
                
                placeable.transform.position = delta + Mathf.Pow(1 - timeBezier, 2) * (startPosition) + 2 * (1 - timeBezier) * timeBezier * (controlPoint) + Mathf.Pow(timeBezier, 2) * (path[i]);

            }
            else
            {
                if (isJumping)
                {
                    isJumping = false;
                    anim.SetTrigger("land");
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    anim.SetTrigger("walk");
                }
                
                placeable.transform.position = Vector3.Lerp(startPosition + delta, path[i] + delta, timeBezier);

            }
            

            
            yield return null;


        }
        
        anim.SetTrigger("land");
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("idle");
        Debug.Log("End" + placeable.GetPosition());
        Debug.Log("End transform" + placeable.transform);
        anim.ResetTrigger("walk");
        anim.ResetTrigger("jump");
        anim.ResetTrigger("land");
        anim.ResetTrigger("landAndJump");
    }
    
    // ONLY FOR OTHER PLACEABLE
    public static IEnumerator MoveAlongBezier(List<Vector3> path, Placeable placeable, float speed)
    {
        if (path.Count < 2)
        {
            yield break;
        }
        float timeBezier = 0f;
        Vector3 delta = placeable.transform.position - path[0];
        Vector3 startPosition = path[0];
        Vector3 controlPoint = new Vector3();
        bool isBezier = true;


        //For visual rotation
        Vector3 targetDir = path[1] - placeable.transform.position;
        targetDir.y = 0;

        int i = 1;
        float distance = CalculateDistance(startPosition, path[i], ref isBezier, ref controlPoint);
        float distanceParcourue = 0;
        while (timeBezier < 1)
        {
            distanceParcourue += (speed * Time.deltaTime);
            timeBezier = distanceParcourue / distance;

            while (timeBezier > 1 && i < path.Count - 1) //on go through 
            {


                distanceParcourue -= distance;
                startPosition = path[i];
                i++; // changing movement
                targetDir = path[i] - placeable.transform.position;//next one
                targetDir.y = 0;// don't move up 

                distance = CalculateDistance(startPosition, path[i], ref isBezier, ref controlPoint);//On calcule la distance au noeud suivant
                timeBezier = distanceParcourue / distance; //on recalcule

            }
            if (i == path.Count - 1 && timeBezier > 1)
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
            

            
            yield return null;


        }

        GameManager.instance.OnEndAnimationEffectEnd();
        Debug.Log("End" + placeable.GetPosition());
        Debug.Log("End transform" + placeable.transform);
        
    }
    
    /// <summary>
    /// Check if use is possible and send rpc
    /// </summary>
    /// <param name="numSkill"></param>
    /// <param name="netidTarget"></param>
    [Command]
    public void CmdUseSkill(int numSkill, int netidTarget)
    {
        Placeable target = GameManager.instance.FindLocalObject(netidTarget);
        Skill skill = GameManager.instance.playingPlaceable.Skills[numSkill];
        if (this == GameManager.instance.playingPlaceable.Player) {
            if ((GameManager.instance.playingPlaceable.GetPosition() - target.GetPosition()).magnitude <= skill.Maxrange
                && (GameManager.instance.playingPlaceable.GetPosition() - target.GetPosition()).magnitude >= skill.Minrange)
            {
                skill.Use(GameManager.instance.playingPlaceable, new List<Placeable>() { target });
                RpcUseSkill(numSkill, netidTarget);
            }
            
        }
    }

    [ClientRpc]
    public void RpcUseSkill(int numSkill, int netidTarget)
    {
        Placeable target = GameManager.instance.FindLocalObject(netidTarget);
        Skill skill = GameManager.instance.playingPlaceable.Skills[numSkill];
        GameManager.instance.playingPlaceable.ResetAreaOfTarget();
        skill.Use(GameManager.instance.playingPlaceable, new List<Placeable>() { target });

    }

}
