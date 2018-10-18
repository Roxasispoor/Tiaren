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
    private bool isReadyToPlay = false;
    public List<GameObject> characters = new List<GameObject>();
    public List<int> numberPrefab;
    private Vector3Int placeToGo;



    private Placeable shotPlaceable;
    public CameraScript cameraScript;
    public delegate float Axis();
    public delegate bool Condition();
    private Dictionary<string, Axis> dicoAxis;
    private Dictionary<string, Condition> dicoCondition;
    public Timer clock;
    private Dictionary<GameObject, Color> changedColor;

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



    /// <summary>
    /// List of characters of the player
    /// </summary>

    private void Awake()
    {
        changedColor = new Dictionary<GameObject, Color>();
        clock = GetComponent<Timer>();
        DicoAxis = new Dictionary<string, Axis>();
        DicoCondition = new Dictionary<string, Condition>();
        DicoAxis.Add("AxisXCamera", () => Input.GetAxis("Mouse X"));
        DicoAxis.Add("AxisYCamera", () => Input.GetAxis("Mouse Y"));
        DicoAxis.Add("AxisZoomCamera", () => Input.GetAxis("Mouse ScrollWheel"));

        DicoCondition.Add("OrbitCamera", () => Input.GetMouseButton(1));
        DicoCondition.Add("PanCamera", () => Input.GetMouseButton(2));


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
        if (this.isServer)
        {

            this.acted = false;
            this.score = 0;

        }
        //Retrieve data 
    }

    private void Update()
    {
        if (clock.IsFinished)
        {
            EndTurn();
        }
    }


    // A useless player actually acts, but the timer is unactive and unlinked to nothing on the canvas
    [ClientRpc]
    public void RpcStartTimer(float time)
    {

        clock.StartTimer(time);
    }
    
    [ClientRpc]
    public void RpcMakeCubeBlue(NetworkInstanceId cube)
    {
        if (isLocalPlayer)
        {
            GameObject clientCube = ClientScene.FindLocalObject(cube);
            changedColor.Add(clientCube, clientCube.GetComponent<Renderer>().material.color);
            //clientCube.parent.Color = testa.gameObject.GetComponent<Renderer>().material.color;
            clientCube.GetComponent<Renderer>().material.color = Color.cyan;

        }
    }
    [ClientRpc]
    public void RpcChangeBackColor()
    {
        if (isLocalPlayer)
        {
            foreach (GameObject cube in changedColor.Keys)
            {
                if (cube.GetComponent<Renderer>() != null)
                {
                    cube.GetComponent<Renderer>().material.color = changedColor[cube];
                }

            }
            changedColor.Clear();
        }
    }

    //launcher for end of turn
    public void EndTurn()
    {
        CmdEndTurn();
        GameManager.instance.EndOFTurn();
    }

    [Command]
    private void CmdEndTurn()
    {
        GameManager.instance.EndOFTurn();
    }

    /// <summary>
    /// Updates where to go on server, and ask gameManager to do it
    /// </summary>
    /// <param name="toGo"></param>
    [Command]
    public void CmdMoveTo(NetworkInstanceId toGo)
    {

        Placeable potential = NetworkServer.FindLocalObject(toGo).GetComponent<Placeable>();
        if (GameManager.instance.PlayingPlaceable.player == this)// updating only if it's his turn to play, other checkings are done in GameManager
        {
            placeToGo = potential.GetPosition();
        }
    }
    [Command]
    public void CmdInputShot(GameObject shotPlaceable)
    {

        this.shotPlaceable = shotPlaceable.GetComponent<Placeable>();
        if (isServer)
        {


        }
    }
    [ClientRpc]
    public void RpcSetCamera(NetworkInstanceId mustPlay)
    {
        Placeable potential = ClientScene.FindLocalObject(mustPlay).GetComponent<Placeable>();

        this.cameraScript.target = potential.gameObject.transform;

    }

    override
    public void OnStartLocalPlayer()
    {
        transform.Find("Main Camera").gameObject.SetActive(true);
        transform.Find("Canvas").gameObject.SetActive(true);
        //activate camera of player, set ready
    }

    public void Translater(int skillID, LivingPlaceable caster, List<LivingPlaceable> targets)
    {

    }

    public void DispatchSkill(int skillID, LivingPlaceable caster, List<Placeable> targets)
    {
        Skill skill = caster.Skills[skillID];
        if (skill.Cooldown == 0 && GameManager.instance.PlayingPlaceable == caster)
        {
            GameManager.instance.UseSkill(skillID, caster, targets);
        }
    }




}
