using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Représente un joueur 
/// </summary>
public class Player : NetworkBehaviour {
    [SyncVar]
    private bool acted;
    [SyncVar]
    private int score;
     private bool isReadyToPlay = false;
    public List<GameObject> personnages=new List<GameObject>();
    public List<int> numeroPrefab;
    private Vector3Int placeToGo;

    
    private Placeable shotPlaceable;
    public CameraScript cameraScript;
    public delegate float Axis();
    public delegate bool Condition();
    private Dictionary<string, Axis> dicoAxis;
    private Dictionary<string, Condition> dicoCondition;
    private bool endPhase;
    public Timer clock;
    private Dictionary<GameObject,Color> changedColor; 

    

    //Un joueur inutile fait bien l'action, c'est juste que son chrono est non activé / relié a rien sur le canvas.
    [ClientRpc]
    public void RpcStartTimer(float temps)
    {
     
        clock.StartTimer(temps);
    }
       /// <summary>
       /// For reasons works that way
       /// </summary>
    public void LaunchCommandEndTurn()
    {
      //  Debug.Log("Bonjour je suis" + this);
        CmdEndTurn();
    }
    [ClientRpc]
    public void RpcMakeCubeBlue(NetworkInstanceId cube)
    {
       // Debug.Log("id client joueur recherché" + concernedJoueur);
        Debug.Log("et moi je suis" + netId);
        if (isLocalPlayer)
        {
            GameObject cuube = ClientScene.FindLocalObject(cube);
            changedColor.Add(cuube, cuube.GetComponent<Renderer>().material.color);
            //cuube.parent.Color = testa.gameObject.GetComponent<Renderer>().material.color;
            cuube.GetComponent<Renderer>().material.color = Color.cyan;

        }
    }
    [ClientRpc]
    public void RpcChangeBackColor()
    {
        if (isLocalPlayer)
        {
            foreach(GameObject  cube in changedColor.Keys)
            {
                if(cube.GetComponent<Renderer>()!=null)
                {
                    cube.GetComponent<Renderer>().material.color = changedColor[cube];
                }

            }
            changedColor.Clear();
        }
    }

    [Command]
    public void CmdEndTurn()
    {
        this.endPhase = true;
        Debug.Log("STP phase finie");
    }

    /// <summary>
    /// Updates where to go on server, and ask gameManager to do it
    /// </summary>
    /// <param name="toGo"></param>
    [Command]
    public void CmdMoveTo(NetworkInstanceId toGo)
    {
        
        Placeable potential= NetworkServer.FindLocalObject(toGo).GetComponent<Placeable>();
        if(GameManager.instance.PlayingPlaceable.player==this)//on update que si c'est à son tour de jouer, on fait les autres vérifs dans la Gamemanager
        {
            placeToGo = potential.GetPosition();
            Debug.Log(placeToGo);
        }         
    }
    [Command]
    public void CmdInputShot(GameObject shotPlaceable)
    {

        this.shotPlaceable = shotPlaceable.GetComponent<Placeable>();
        if (isServer)
        {
            Debug.Log("Coucou shot commanded");
            Debug.Log(shotPlaceable);

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
        //On active la camera du joueur et on se dit prêt


    }

    /// <summary>
    /// Permet de savoir si le joueur a agi ce tour ci
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
    /// Score du joueur
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

    public List<GameObject> Personnages
    {
        get
        {
            return personnages;
        }

        set
        {
            personnages = value;
        }
    }

    public List<int> NumeroPrefab
    {
        get
        {
            return numeroPrefab;
        }

        set
        {
            numeroPrefab = value;
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

    public bool EndPhase
    {
        get
        {
            return endPhase;
        }

        set
        {
            endPhase = value;
        }
    }



    /// <summary>
    /// Liste des personnages du joueur
    /// </summary>

    private void Awake()
    {
        changedColor = new Dictionary<GameObject,Color>(); 
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
    void Start () {
        
        if(GameManager.instance.player1==null)
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
	
	// Update is called once per frame
	void Update () {
       
		
	}
}
